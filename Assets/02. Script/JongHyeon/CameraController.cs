using System.Collections;
using UnityEngine;
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

    // Past Volume
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

        // Get WhiteBalance and Vignette components from the Volume profile
        if (pastVolume.profile.TryGet(out pastWhiteBalance) && pastVolume.profile.TryGet(out pastVignette))
        {
            // Successfully retrieved WhiteBalance and Vignette
        }
        else
        {
            Debug.LogError("WhiteBalance or Vignette not found in Volume profile.");
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
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
        fixedYPosition = target.position.y;
        isFollowingY = true;

        // �ʱ� ���¿��� WhiteBalance�� Vignette ȿ���� ��Ȱ��ȭ
        pastWhiteBalance.temperature.value = 0f;
        pastVignette.intensity.value = 0f;
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

    public void PastVolumeOnOff(bool isGoingToPast)
    {
        StartCoroutine(ToggleFullScreenPassCoroutine(isGoingToPast));
    }

    private void SetupFullScreenPass()
    {
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

    private IEnumerator ToggleFullScreenPassCoroutine(bool isGoingToPast)
    {
        if (fullScreenPass != null)
        {
            fullScreenPass.SetActive(true); // FullScreenPass Ȱ��ȭ
            float halfDuration = fullScreenPassDuration / 2f;
            float timer = 0f;

            // ��ǥ �� ����
            float targetWhiteBalanceTemp = isGoingToPast ? 40f : 0f;
            float targetVignetteIntensity = isGoingToPast ? 0.4f : 0f;

            // ���������� ���� �����Ͽ� ��ǥ ���� ����
            while (timer < halfDuration)
            {
                timer += Time.deltaTime;
                float t = timer / halfDuration;

                // UnderWaterMat�� Blend �� ����
                float blendValue = Mathf.Lerp(0f, 0.1f, t);
                underWaterMat.SetFloat("_Blend", blendValue);

                // WhiteBalance�� Vignette �� ������ ��ȭ
                if (pastWhiteBalance != null)
                    pastWhiteBalance.temperature.value = Mathf.Lerp(pastWhiteBalance.temperature.value, targetWhiteBalanceTemp, t);
                if (pastVignette != null)
                    pastVignette.intensity.value = Mathf.Lerp(pastVignette.intensity.value, targetVignetteIntensity, t);

                yield return null;
            }

            // ��ǥ ���� ����
            if (pastWhiteBalance != null)
                pastWhiteBalance.temperature.value = targetWhiteBalanceTemp;
            if (pastVignette != null)
                pastVignette.intensity.value = targetVignetteIntensity;

            // Blend ���� 0���� �ǵ���
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
    private void OnApplicationQuit()
    {
        fullScreenPass.SetActive(false);
    }
}
