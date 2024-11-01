using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;           // 카메라가 회전할 타겟
    public float distance = 5.0f;      // 타겟과의 거리
    public float rotationSpeed = 2.0f; // 회전 속도
    private bool isRightMouseDown = false;
    private float yaw;
    private float pitch;

    public float minPitch = -30f; // 카메라가 내려갈 수 있는 최소 각도
    public float maxPitch = 60f;  // 카메라가 올라갈 수 있는 최대 각도

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not assigned. Please assign a target for the camera to orbit.");
            return;
        }

        // 카메라의 초기 회전값 설정
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }
    private void Update()
    {

    }
    void LateUpdate()
    {
        // 항상 타겟을 따라가도록 설정
        FollowTarget();

        // 마우스 오른쪽 버튼을 누르면 회전 활성화
        if (Input.GetMouseButtonDown(1))
        {
            isRightMouseDown = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // 마우스 오른쪽 버튼을 떼면 회전 비활성화
        if (Input.GetMouseButtonUp(1))
        {
            isRightMouseDown = false;
            Cursor.lockState = CursorLockMode.None;
        }

        // 마우스 오른쪽 버튼이 눌린 동안 카메라 회전
        if (isRightMouseDown)
        {
            RotateCamera();
        }
    }

    void FollowTarget()
    {
        // 현재 카메라 회전값과 거리로 타겟을 따라가도록 위치 설정
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = new Vector3(0, 0, -distance);
        transform.position = target.position + rotation * offset;
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // yaw와 pitch 값을 업데이트하여 회전
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch); // pitch 각도 제한

        // 회전 적용 및 타겟을 바라보도록 설정
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = new Vector3(0, 0, -distance);
        transform.position = target.position + rotation * offset;
        transform.LookAt(target);
    }
}