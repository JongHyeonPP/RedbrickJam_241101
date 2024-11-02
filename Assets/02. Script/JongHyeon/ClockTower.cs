using System.Collections;
using UnityEngine;

public class ClockTower : MonoBehaviour
{
    public float rotationSpeed = 30f; // ȸ�� �ӵ� ���� ����

    void Start()
    {
        StartCoroutine(RotateClockTower());
    }

    private IEnumerator RotateClockTower()
    {
        while (true)
        {
            // y���� �������� ������ �ӵ��� õõ�� ȸ��
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            yield return null; // ���� �����ӱ��� ���
        }
    }
}
