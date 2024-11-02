using System.Collections;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public bool isPresent { get; private set; } = true;
    private bool isOnCooldown = false; // 쿨타임 상태를 저장하는 변수
    CameraController cameraController;

    // UI
    public GameObject presentButton;
    public GameObject pastButton;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    private void Start()
    {
        pastButton.SetActive(false);
        presentButton.SetActive(false);
    }

    public void GoPast()
    {
        if (isOnCooldown) return; // 쿨타임 중이면 함수를 실행하지 않음
        isPresent = false;
        cameraController.PastVolumeOnOff(true);
        pastButton.SetActive(false);
        presentButton.SetActive(true);

        StartCoroutine(StartCooldown()); // 쿨타임 시작
    }

    public void GoPresent()
    {
        if (isOnCooldown) return; // 쿨타임 중이면 함수를 실행하지 않음
        isPresent = true;
        cameraController.PastVolumeOnOff(false);
        pastButton.SetActive(true);
        presentButton.SetActive(false);

        StartCoroutine(StartCooldown()); // 쿨타임 시작
    }

    private IEnumerator StartCooldown()
    {
        isOnCooldown = true; // 쿨타임 시작
        yield return new WaitForSeconds(2f); // 2초 대기
        isOnCooldown = false; // 쿨타임 종료
    }
}
