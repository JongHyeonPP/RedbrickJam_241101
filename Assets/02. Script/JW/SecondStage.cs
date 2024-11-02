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

    private Quaternion startRotation;    // 시작 회전 값
    private Quaternion targetRotation;   // 목표 회전 값
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

        Quaternion startRotation = tower[towerNum].transform.rotation; // 현재 회전 값
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, -90); // Z축으로 90도 추가

        while (elapsed < duration)
        {
            // 경과 시간에 따라 회전 비율 계산
            tower[towerNum].transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsed / duration);

            elapsed += Time.deltaTime; // 경과 시간 누적
            yield return null;         // 다음 프레임까지 대기
        }

        transform.rotation = targetRotation; // 마지막으로 목표 각도로 설정
        isRotating = false; // 회전 종료 표시
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
            Destroy(instance); // 각 인스턴스 삭제
        }
        electricInstances.Clear(); // 리스트 비우기
    }

    public bool CheckingLoop(GameObject hitObject)
    {
        foreach (GameObject instance in electricInstances)
        {
            if(instance.GetComponent<LightningBoltScript>().StartObject == hitObject)
            {
                // 무한 루프면
                return false;
            }
        }
        return true;
    }
}
