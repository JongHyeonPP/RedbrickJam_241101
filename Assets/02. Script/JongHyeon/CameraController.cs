using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing.MiniJSON;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    //PastVolume
    private Volume pastVolume;
    private WhiteBalance pastWhiteBalance;
    private Vignette pastVignette;
    // Renderer Feature
    private FullScreenPassRendererFeature fullScreenPass;
    private float fullScreenPassDuration = 2f; // FullScreenPass�� Ȱ��ȭ�Ǵ� �ð�
    public UniversalRendererData rendererData;
    public Material underWaterMat;
    private void Awake()
    {
        pastVolume = GetComponent<Volume>();

        // Get Bloom and Vignette components from the Volume profile
        if (pastVolume.profile.TryGet(out pastWhiteBalance) && pastVolume.profile.TryGet(out pastVignette))
        {
            // Successfully retrieved Bloom and Vignette
        }
        else
        {
            Debug.LogError("Bloom or Vignette not found in Volume profile.");
        }
        SetupFullScreenPass();
    }

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not assigned. Please assign a target for the camera to orbit.");
            return;
        }
        // Set initial rotation values
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        // Initialize y position and following flag
        fixedYPosition = target.position.y;
        isFollowingY = true;

        pastWhiteBalance.active =  pastVignette.active = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRightMouseDown = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isRightMouseDown = false;
            Cursor.lockState = CursorLockMode.None;
        }

        if (IsCharacterGrounded())
        {
            isFollowingY = true;
            fixedYPosition = target.position.y;
        }
        else
        {
            isFollowingY = false;
        }
    }

    void LateUpdate()
    {
        if (isRightMouseDown)
        {
            RotateAndFollowTarget();
        }
        else
        {
            targetPosition = CalculateCameraPosition();
            targetRotation = Quaternion.LookRotation(target.position - targetPosition);
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / smoothTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / smoothTime);
    }

    void RotateAndFollowTarget()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = new Vector3(0, 0, -distance);

        float targetY = isFollowingY ? target.position.y : fixedYPosition;
        targetPosition = new Vector3(target.position.x, targetY, target.position.z) + rotation * offset;
        targetRotation = Quaternion.LookRotation(new Vector3(target.position.x, targetY, target.position.z) - targetPosition);
    }

    Vector3 CalculateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = new Vector3(0, 0, -distance);

        float targetY = isFollowingY ? target.position.y : fixedYPosition;
        return new Vector3(target.position.x, targetY, target.position.z) + rotation * offset;
    }

    bool IsCharacterGrounded()
    {
        return Physics.Raycast(target.position, Vector3.down, 0.1f);
    }

    public void PastVolumeOnOff(bool _isOn)
    {
        StartCoroutine(ToggleFullScreenPassCoroutine()); // FullScreenPass ��� Ȱ��ȭ
        if (pastWhiteBalance != null)
            pastWhiteBalance.active = _isOn;

        if (pastVignette != null)
            pastVignette.active = _isOn;
    }
    private void SetupFullScreenPass()
    {
        // FullScreenPassRendererFeature ã��
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature is FullScreenPassRendererFeature)
            {
                fullScreenPass = (FullScreenPassRendererFeature)feature;
                fullScreenPass.SetActive(false);
                break;
            }
        }

        if (fullScreenPass == null)
        {
            Debug.LogError("FullScreenPassRendererFeature�� Renderer�� �����Ǿ� ���� �ʽ��ϴ�.");
        }
    }

    private IEnumerator ToggleFullScreenPassCoroutine()
    {
        if (fullScreenPass != null)
        {
            fullScreenPass.SetActive(true); // FullScreenPass Ȱ��ȭ
            float halfDuration = fullScreenPassDuration / 2f;
            float timer = 0f;

            // Blend ���� 0���� 0.1���� ����
            while (timer < halfDuration)
            {
                timer += Time.deltaTime;
                float blendValue = Mathf.Lerp(0f, 0.1f, timer / halfDuration);
                underWaterMat.SetFloat("_Blend", blendValue);
                yield return null;
            }

            // Blend ���� 0.1���� 0���� ����
            timer = 0f;
            while (timer < halfDuration)
            {
                timer += Time.deltaTime;
                float blendValue = Mathf.Lerp(0.1f, 0f, timer / halfDuration);
                underWaterMat.SetFloat("_Blend", blendValue);
                yield return null;
            }

            fullScreenPass.SetActive(false); // FullScreenPass ��Ȱ��ȭ
        }
        else
        {
            Debug.LogWarning("FullScreenPassRendererFeature�� ã�� �� �����ϴ�.");
        }
    }
}
