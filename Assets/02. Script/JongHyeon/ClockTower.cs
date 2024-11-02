using System.Collections;
using UnityEngine;

public class ClockTower : MonoBehaviour
{
    public float rotationSpeed = 30f; // 회전 속도 조절 변수

    void Start()
    {
        StartCoroutine(RotateClockTower());
    }

    private IEnumerator RotateClockTower()
    {
        while (true)
        {
            // y축을 기준으로 지정된 속도로 천천히 회전
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            yield return null; // 다음 프레임까지 대기
        }
    }
}
