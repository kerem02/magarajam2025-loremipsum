using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpiderSurfaceController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 8f;
    public float rotationSpeed = 8f;      
    public float rayDistance = 1.5f;      

    private Rigidbody rb;
    private Vector3 moveDirection;
    private Vector3 velocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Input al
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        moveDirection = new Vector3(h, 0, v).normalized;
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, rayDistance))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);

            Vector3 surfaceForward = Vector3.Cross(Camera.main.transform.right, hit.normal).normalized;
            Vector3 surfaceRight = Vector3.Cross(hit.normal, surfaceForward).normalized;

            Vector3 desiredMove = (surfaceForward * moveDirection.z + surfaceRight * moveDirection.x).normalized;

            if (moveDirection.magnitude > 0.1f)
            {
                velocity = Vector3.Lerp(velocity, desiredMove * moveSpeed, Time.fixedDeltaTime * acceleration);
            }
            else
            {
                velocity = Vector3.Lerp(velocity, Vector3.zero, Time.fixedDeltaTime * deceleration);
            }

            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }
}
