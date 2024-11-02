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
            Debug.Log("�۽�ž ����");
            // �۽�ž ����
            TransmissionTower tower = other.GetComponent<TransmissionTower>();
            if (tower != null)
            {
                // �۽�ž �������� ���� �߻�
                Vector3 newDirection = tower.transform.forward;
                tower.FireCurrent(newDirection); // ���� �߻�
            }
            Destroy(gameObject); // ���� ���� ����
        }
    }
}
