using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;           // ī�޶� ȸ���� Ÿ��
    public float distance = 5.0f;      // Ÿ�ٰ��� �Ÿ�
    public float rotationSpeed = 2.0f; // ȸ�� �ӵ�
    private bool isRightMouseDown = false;
    private float yaw;
    private float pitch;

    public float minPitch = -30f; // ī�޶� ������ �� �ִ� �ּ� ����
    public float maxPitch = 60f;  // ī�޶� �ö� �� �ִ� �ִ� ����

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not assigned. Please assign a target for the camera to orbit.");
            return;
        }

        // ī�޶��� �ʱ� ȸ���� ����
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }
    private void Update()
    {

    }
    void LateUpdate()
    {
        // �׻� Ÿ���� ���󰡵��� ����
        FollowTarget();

        // ���콺 ������ ��ư�� ������ ȸ�� Ȱ��ȭ
        if (Input.GetMouseButtonDown(1))
        {
            isRightMouseDown = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // ���콺 ������ ��ư�� ���� ȸ�� ��Ȱ��ȭ
        if (Input.GetMouseButtonUp(1))
        {
            isRightMouseDown = false;
            Cursor.lockState = CursorLockMode.None;
        }

        // ���콺 ������ ��ư�� ���� ���� ī�޶� ȸ��
        if (isRightMouseDown)
        {
            RotateCamera();
        }
    }

    void FollowTarget()
    {
        // ���� ī�޶� ȸ������ �Ÿ��� Ÿ���� ���󰡵��� ��ġ ����
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = new Vector3(0, 0, -distance);
        transform.position = target.position + rotation * offset;
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // yaw�� pitch ���� ������Ʈ�Ͽ� ȸ��
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch); // pitch ���� ����

        // ȸ�� ���� �� Ÿ���� �ٶ󺸵��� ����
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = new Vector3(0, 0, -distance);
        transform.position = target.position + rotation * offset;
        transform.LookAt(target);
    }
}