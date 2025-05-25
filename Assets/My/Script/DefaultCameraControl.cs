using UnityEngine;

public class DefaultCameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float fastSpeed = 15f;
    public float mouseSensitivity = 2f;
    public float scrollSensitivity = 2f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    public float panSpeed = 0.5f; // Sensitivity of the pan
    private Vector3 lastMousePosition;
    private bool isPanning = false;


    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotationX = rot.y;
        rotationY = rot.x;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {

        // Right Mouse Button Look
        if ( Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            rotationX += mouseX;
            rotationY -= mouseY;
            rotationY = Mathf.Clamp(rotationY, -90f, 90f);

            transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
        }
        // Middle Mouse Button Pan
        else if (Input.GetMouseButtonDown(2))
        {
            isPanning = true;
            lastMousePosition = Input.mousePosition;
        }

        else if (Input.GetMouseButtonUp(2))
        {
            isPanning = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }


        if (isPanning)
        {
            // dynamic panSpeed
            panSpeed = Mathf.Clamp(Vector3.Distance(transform.position, Vector3.zero) / 2f, 1f, 100f);

            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 move = new Vector3(-delta.x, -delta.y, 0) * panSpeed * Time.deltaTime;

            // Move the camera in its own local space (e.g., for orthographic top-down view)
            transform.Translate(move, Space.Self);

            lastMousePosition = Input.mousePosition;
        }
        else 
        {
            fastSpeed = Mathf.Clamp(Vector3.Distance(transform.position, Vector3.zero) / 5f, 15f, 100f);
            // Movement
            float speed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed : fastSpeed;

            Vector3 move = new Vector3(
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical")
            );

            if (Input.GetKey(KeyCode.E)) move.y += 1;
            if (Input.GetKey(KeyCode.C)) move.y -= 1;

            transform.Translate(move * speed * Time.deltaTime, Space.Self);

            // Scroll forward/backward
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                transform.Translate(Vector3.forward * scroll * scrollSensitivity, Space.Self);
            }
        }
    }
}