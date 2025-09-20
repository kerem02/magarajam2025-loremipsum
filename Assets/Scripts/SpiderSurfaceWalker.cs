using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpiderSurfaceWalker : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2.5f;
    public float turnSpeed = 12f;
    public float hoverDistance = 0.05f;

    [Header("Raycasts")]
    public float rayLength = 0.4f;
    [Range(0.01f, 0.5f)] public float rayRingRadius = 0.15f;
    [Range(0f, 60f)] public float ringInwardAngle = 25f;

    [Header("Adhesion / Grounding")]
    public float stickForce = 35f;
    public LayerMask groundMask;

    [Header("Forward Transfer")]
    public float forwardProbeLength = 0.6f;   // önündeki başka dalı aramak için
    public float forwardProbeLift = 0.06f;    // mevcut yüzeyden ne kadar yukarıdan başlasın
    public float forwardProbeRadius = 0.06f;  // spherecast yarıçapı
    public float transitionSpeed = 4.0f;      // geçiş hızın
    public float arriveDistance = 0.04f;      // hedefe varmış sayılma mesafesi
    [Range(0, 60f)] public float minNormalDelta = 8f; // aynı collidera çarpıyorsa açı farkı şartı

    Rigidbody rb;

    Vector3 lastGroundNormal = Vector3.up;
    bool isGrounded;
    Collider lastGroundCollider;

    // transition state
    bool inTransition;
    Vector3 transTargetPos;
    Vector3 transTargetNormal;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        if (!inTransition)
        {
            // --- Normal Yürüme ---
            Vector3 avgNormal, groundPoint;
            Collider groundCol;
            isGrounded = ProbeGround(out avgNormal, out groundPoint, out groundCol);

            if (isGrounded)
            {
                lastGroundNormal = avgNormal;
                lastGroundCollider = groundCol;

                // yüzeye oturt
                Quaternion align = Quaternion.FromToRotation(transform.up, avgNormal) * transform.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, align, turnSpeed * Time.fixedDeltaTime);

                Vector3 desiredPos = groundPoint + avgNormal * hoverDistance;
                Vector3 toDesired = desiredPos - transform.position;
                rb.AddForce(toDesired / Time.fixedDeltaTime, ForceMode.Acceleration);
            }

            // input -> yüzeye projeksiyon
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector3 moveDir = GetSurfaceMoveDirection(input, lastGroundNormal);

            Vector3 targetVel = moveDir * moveSpeed;
            Vector3 velChange = targetVel - rb.velocity;
            rb.AddForce(velChange, ForceMode.VelocityChange);

            if (moveDir.sqrMagnitude > 0.0001f)
            {
                Quaternion look = Quaternion.LookRotation(moveDir, lastGroundNormal);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.fixedDeltaTime);
            }

            // yapışma
            rb.AddForce(-lastGroundNormal * stickForce, ForceMode.Acceleration);

            // --- ÖN TRANSFER TARAMASI ---
            TryStartForwardTransfer();
        }
        else
        {
            // --- Geçiş Modu ---
            Vector3 toTarget = transTargetPos - transform.position;

            // hedefe yönel ve ilerle
            Vector3 desiredVel = toTarget.normalized * transitionSpeed;
            rb.velocity = Vector3.Lerp(rb.velocity, desiredVel, 0.35f);

            // rotasyonu hedef normaline hizala
            Quaternion look = Quaternion.LookRotation(
                Vector3.ProjectOnPlane(transform.forward, transTargetNormal).normalized,
                transTargetNormal);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.fixedDeltaTime);

            // hafif yapışma
            rb.AddForce(-transTargetNormal * stickForce, ForceMode.Acceleration);

            if (toTarget.magnitude <= arriveDistance)
            {
                inTransition = false; // normale dön
                lastGroundNormal = transTargetNormal;
                rb.velocity = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// Ön yöne yüzeye paralel SphereCast. Farklı bir dala çarparsa geçiş başlatır.
    /// </summary>
    void TryStartForwardTransfer()
    {
        // yüzeye paralel ileri yön
        Vector3 forwardOnSurface = Vector3.ProjectOnPlane(transform.forward, lastGroundNormal).normalized;
        if (forwardOnSurface.sqrMagnitude < 0.0001f) return;

        // Mevcut yüzeyden biraz yukarıdan başlat, kendi dalını kaçırmak için
        Vector3 start = transform.position + lastGroundNormal * forwardProbeLift;

        bool hitSomething = Physics.SphereCast(
            start, forwardProbeRadius, forwardOnSurface,
            out RaycastHit hit, forwardProbeLength, groundMask, QueryTriggerInteraction.Ignore);

#if UNITY_EDITOR
        Debug.DrawRay(start, forwardOnSurface * forwardProbeLength, hitSomething ? Color.cyan : Color.yellow);
#endif

        if (!hitSomething) return;

        // Aynı collider ise: açı farkı yeterince büyükse yine izin ver
        bool differentCollider = (hit.collider != null && hit.collider != lastGroundCollider);
        float angleDelta = Vector3.Angle(hit.normal, lastGroundNormal);

        if (differentCollider || angleDelta >= minNormalDelta)
        {
            inTransition = true;
            transTargetNormal = hit.normal;
            transTargetPos = hit.point + hit.normal * hoverDistance;
        }
    }

    /// <summary>
    /// 1 ana ray + 4 halka ray ile ortalama zemin hesabı.
    /// </summary>
    bool ProbeGround(out Vector3 avgNormal, out Vector3 avgPoint, out Collider groundCol)
    {
        int hitCount = 0;
        Vector3 normalSum = Vector3.zero;
        Vector3 pointSum = Vector3.zero;
        groundCol = null;

        // ana ray
        if (Physics.Raycast(new Ray(transform.position, -transform.up),
            out RaycastHit mainHit, rayLength, groundMask, QueryTriggerInteraction.Ignore))
        {
            normalSum += mainHit.normal;
            pointSum += mainHit.point;
            hitCount++;
            groundCol = mainHit.collider;
        }

        // 4 yardımcı ray
        Vector3[] offsets =
        {
            transform.right *  rayRingRadius,
            -transform.right * rayRingRadius,
            transform.forward *  rayRingRadius,
            -transform.forward * rayRingRadius
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector3 origin = transform.position + offsets[i];
            Vector3 toCenter = (transform.position - origin).normalized;
            Vector3 dir = (-transform.up + toCenter).normalized;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, rayLength, groundMask, QueryTriggerInteraction.Ignore))
            {
                normalSum += hit.normal;
                pointSum += hit.point;
                hitCount++;
                if (groundCol == null) groundCol = hit.collider;
            }

#if UNITY_EDITOR
            Debug.DrawRay(origin, dir * rayLength, hit.collider ? Color.green : Color.red);
#endif
        }

#if UNITY_EDITOR
        Debug.DrawRay(transform.position, -transform.up * rayLength, mainHit.collider ? Color.green : Color.red);
#endif

        if (hitCount > 0)
        {
            avgNormal = (normalSum / hitCount).normalized;
            avgPoint = pointSum / hitCount;
            return true;
        }

        avgNormal = lastGroundNormal;
        avgPoint = transform.position - transform.up * (hoverDistance + 0.02f);
        return false;
    }

    Vector3 GetSurfaceMoveDirection(Vector2 input, Vector3 surfaceNormal)
    {
        if (input.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        Vector3 right = Vector3.ProjectOnPlane(transform.right, surfaceNormal).normalized;
        Vector3 fwd   = Vector3.ProjectOnPlane(transform.forward, surfaceNormal).normalized;
        return (right * input.x + fwd * input.y).normalized;
    }
}
