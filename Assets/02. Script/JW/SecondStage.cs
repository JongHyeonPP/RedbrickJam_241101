using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;

public class SecondStage : MonoBehaviour
{
    public static SecondStage instance;
    public GameObject player;
    public GameObject[] tower;
    public int towerNum;
    public string timeOfStuff;
    public GameObject gemStone;

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
        if (towerNum >= 0 && !isRotating && Input.GetKeyDown("e") && timeOfStuff == GameManager.instance.nowState)
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

        Quaternion startRotation = tower[towerNum].transform.rotation; // ���� ȸ�� ��
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, -90); // Z������ 90�� �߰�

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

    public void Clear()
    {
        gemStone.SetActive(true);
        Invoke("ClearCurrentInstances", 1f);
    }

    public void ClearCurrentInstances()
    {
        foreach (GameObject instance in electricInstances)
        {
            Destroy(instance); // �� �ν��Ͻ� ����
        }
        electricInstances.Clear(); // ����Ʈ ����
    }

    public bool CheckingLoop(GameObject hitObject)
    {
        foreach (GameObject instance in electricInstances)
        {
            if(instance.GetComponent<LightningBoltScript>().StartObject == hitObject)
            {
                // ���� ������
                return false;
            }
        }
        return true;
    }
}
