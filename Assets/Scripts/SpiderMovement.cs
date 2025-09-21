using UnityEngine;

public class SpiderSurfaceController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 8f;
    public float rotationSpeed = 8f;      
    public float rayDistance = 1.5f;      

    private Vector3 moveDirection;
    private Vector3 velocity;

    private Animator animator;

    void Update()
    {
        // Input al
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        moveDirection = new Vector3(h, 0, v).normalized;

        // Raycast ile yüzeye yapış
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, rayDistance))
        {
            // Normal'e göre rotasyonu düzelt
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Yüzey üzerinde ileri/sağ yönler
            Vector3 surfaceForward = Vector3.Cross(Camera.main.transform.right, hit.normal).normalized;
            Vector3 surfaceRight = Vector3.Cross(hit.normal, surfaceForward).normalized;

            // Hareket yönü
            Vector3 desiredMove = (surfaceForward * moveDirection.z + surfaceRight * moveDirection.x).normalized;

            // Hızlanma / yavaşlama
            if (moveDirection.magnitude > 0.1f)
            {
                velocity = Vector3.Lerp(velocity, desiredMove * moveSpeed, Time.deltaTime * acceleration);
            }
            else
            {
                velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * deceleration);
            }

            // Pozisyonu güncelle
            transform.position += velocity * Time.deltaTime;
        }
    }
}