using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{

    public Transform target;           // Target for the camera to orbit
    public float distance = 5.0f;      // Distance from the target
    public float rotationSpeed = 2.0f; // Rotation speed
    private bool isRightMouseDown = false;
    private float yaw;
    private float pitch;

    public float minPitch = -30f; // Minimum pitch angle
    public float maxPitch = 60f;  // Maximum pitch angle
    public float smoothTime = 0.1f; // Smooth time for damping

    private Vector3 targetPosition;    // Stores calculated target position
    private Quaternion targetRotation; // Stores calculated target rotation
    private float fixedYPosition;      // Fixed y position during jump
    private bool isFollowingY;         // Flag to control y-axis following
    private Volume volume;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not assigned. Please assign a target for the camera to orbit.");
            return;
        }
        volume = GetComponent<Volume>();
        // Set initial rotation values
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        // Initialize y position and following flag
        fixedYPosition = target.position.y;
        isFollowingY = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PostProcessOnOff(!volume.enabled);
        }

        // Check if right mouse button is pressed to enable rotation
        if (Input.GetMouseButtonDown(1))
        {
            isRightMouseDown = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Check if right mouse button is released to disable rotation
        if (Input.GetMouseButtonUp(1))
        {
            isRightMouseDown = false;
            Cursor.lockState = CursorLockMode.None;
        }

        // Update y-axis following state based on grounded status
        if (IsCharacterGrounded())
        {
            isFollowingY = true;              // Follow character's y position when grounded
            fixedYPosition = target.position.y; // Update fixed y position to current ground level
        }
        else
        {
            isFollowingY = false; // Stop following y position when in the air (jumping)
        }
    }

    void LateUpdate()
    {
        // Rotate and follow the target in a single function
        if (isRightMouseDown)
        {
            RotateAndFollowTarget();
        }
        else
        {
            // Always follow target position when not rotating
            targetPosition = CalculateCameraPosition();
            targetRotation = Quaternion.LookRotation(target.position - targetPosition);
        }

        // Smoothly interpolate position and rotation to avoid jittering
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / smoothTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / smoothTime);
    }

    void RotateAndFollowTarget()
    {
        // Get mouse input for rotation
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // Update yaw and pitch values
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch); // Clamp pitch to min and max values

        // Calculate the new rotation and offset position
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = new Vector3(0, 0, -distance);

        // Update target position based on y-following state
        float targetY = isFollowingY ? target.position.y : fixedYPosition; // Use fixed y during jump
        targetPosition = new Vector3(target.position.x, targetY, target.position.z) + rotation * offset;
        targetRotation = Quaternion.LookRotation(new Vector3(target.position.x, targetY, target.position.z) - targetPosition);
    }

    Vector3 CalculateCameraPosition()
    {
        // Calculate camera position based on current yaw and pitch
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = new Vector3(0, 0, -distance);

        // Update y based on following state
        float targetY = isFollowingY ? target.position.y : fixedYPosition;
        return new Vector3(target.position.x, targetY, target.position.z) + rotation * offset;
    }

    bool IsCharacterGrounded()
    {
        // Raycast to check if the character is grounded
        return Physics.Raycast(target.position, Vector3.down, 0.1f);
    }
    void PostProcessOnOff(bool _isOn)
    {
        volume.enabled = _isOn;
    }
}
