using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;

public class ElectricTower : MonoBehaviour
{
    public float rayDistance = 100f; // ������ �ִ� �Ÿ�
    public GameObject electricBolt;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // �����̽� Ű�� ������ �� ���� �߻�
        {
            RaycastHit hit; 
            
            Vector3 direction = transform.forward;

            // ����ĳ��Ʈ
            if (Physics.Raycast(transform.position, direction, out hit, rayDistance))
            {
                // ���� ó���� ���� ������Ʈ�� ��������
                GameObject hitObject = hit.collider.gameObject;
                GameObject electric = Instantiate(electricBolt, transform.position, Quaternion.identity);
                electric.GetComponent<LightningBoltScript>().StartObject = this.gameObject;
                electric.GetComponent<LightningBoltScript>().EndObject = hitObject;
                SecondStage.instance.CreateElectricBolt(electric);
                hitObject.GetComponent<TransmissionTower>().ElectricBolt();
                // ��Ʈ�� ������Ʈ�� ���� ���
                Debug.Log("Hit Object: " + hitObject.name);
            }
            else
            {
                Debug.Log("No object hit");
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.forward * rayDistance;
        Gizmos.DrawLine(transform.position, transform.position + direction);
    }
}
