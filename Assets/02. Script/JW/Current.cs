using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Current : MonoBehaviour
{
    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    void Update()
    {
        transform.position += direction * Time.deltaTime * 5;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TransmissionTower"))
        {
            Debug.Log("송신탑 도착");
            // 송신탑 도착
            TransmissionTower tower = other.GetComponent<TransmissionTower>();
            if (tower != null)
            {
                // 송신탑 방향으로 전류 발사
                Vector3 newDirection = tower.transform.forward;
                tower.FireCurrent(newDirection); // 전류 발사
            }
            Destroy(gameObject); // 현재 전류 제거
        }
    }
}
