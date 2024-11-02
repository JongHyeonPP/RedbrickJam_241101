using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondStage : MonoBehaviour
{
    public static SecondStage instance;
    public GameObject player;
    public GameObject[] tower;
    public int towerNum;

    private List<GameObject> electricInstances = new List<GameObject>();

    private Quaternion startRotation;    // ���� ȸ�� ��
    private Quaternion targetRotation;   // ��ǥ ȸ�� ��
    private bool isRotating = false;

    void Start()
    {
        instance = this;
        towerNum = -1;
    }

    void Update()
    {
        if (towerNum >= 0 && !isRotating && Input.GetKeyDown("e"))
        {
            StartCoroutine(RotateOverTime(2, towerNum));
        }
        if (Input.GetKeyDown("f"))
        {
            ClearCurrentInstances();
        }
    }
    
    IEnumerator RotateOverTime(float duration, int towerNum)
    {
        isRotating = true;
        float elapsed = 0f;

        Quaternion startRotation = transform.rotation; // ���� ȸ�� ��
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 90, 0); // Y������ 90�� �߰�

        while (elapsed < duration)
        {
            // ��� �ð��� ���� ȸ�� ���� ���
            tower[towerNum].transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsed / duration);

            elapsed += Time.deltaTime; // ��� �ð� ����
            yield return null;         // ���� �����ӱ��� ���
        }

        transform.rotation = targetRotation; // ���������� ��ǥ ������ ����
        isRotating = false; // ȸ�� ���� ǥ��
    }

    public void CreateElectricBolt(GameObject electiricBolt)
    {
        electricInstances.Add(electiricBolt);
    }

    public void ClearCurrentInstances()
    {
        foreach (GameObject instance in electricInstances)
        {
            Destroy(instance); // �� �ν��Ͻ� ����
        }
        electricInstances.Clear(); // ����Ʈ ����
    }
}
