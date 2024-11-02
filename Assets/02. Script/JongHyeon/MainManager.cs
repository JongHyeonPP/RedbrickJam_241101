using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public bool isPresent { get; private set; } = true;
    private bool isOnCooldown = false; // ��Ÿ�� ���¸� �����ϴ� ����
    CameraController cameraController;

    // UI
    public GameObject presentButton;
    public GameObject pastButton;
    //PushTrick
    [SerializeField] Transform placedParent;
    Transform[,] placed = new Transform[5, 5];//�� ��ġ
    bool[,] isPlaced = new bool[5, 5];//��ġ �ƴ���
    [SerializeField] List<PushObject> pushObjects = new();


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
        if (isOnCooldown) return; // ��Ÿ�� ���̸� �Լ��� �������� ����
        isPresent = false;
        cameraController.PastVolumeOnOff(true);
        pastButton.SetActive(false);
        presentButton.SetActive(true);

        StartCoroutine(StartCooldown()); // ��Ÿ�� ����
    }

    public void GoPresent()
    {
        if (isOnCooldown) return; // ��Ÿ�� ���̸� �Լ��� �������� ����
        isPresent = true;
        cameraController.PastVolumeOnOff(false);
        pastButton.SetActive(true);
        presentButton.SetActive(false);

        StartCoroutine(StartCooldown()); // ��Ÿ�� ����
    }

    private IEnumerator StartCooldown()
    {
        isOnCooldown = true; // ��Ÿ�� ����
        yield return new WaitForSeconds(2f); // 2�� ���
        isOnCooldown = false; // ��Ÿ�� ����
    }
}
