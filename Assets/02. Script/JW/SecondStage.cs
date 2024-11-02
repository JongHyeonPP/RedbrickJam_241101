using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondStage : MonoBehaviour
{
    public static SecondStage instance;
    public GameObject player;
    public GameObject[] tower;
    public int towerNum;

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
        
    }

    public void Rotation()
    {
        if(towerNum >= 0 && !isRotating)
        {
            StartCoroutine(RotateOverTime(2, towerNum));
        }
    }

    IEnumerator RotateOverTime(float duration, int towerNum)
    {
        isRotating = true;
        float elapsed = 0f;

        Quaternion startRotation = transform.rotation; // 현재 회전 값
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 90, 0); // Y축으로 90도 추가

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
}
