using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;

public class ElectricTower : MonoBehaviour
{
    public float rayDistance = 10f; // 레이의 최대 거리
    public GameObject electricBolt;
    Vector3 FirePosition;

    private void Start()
    {
        ConnectElectric();
    }

    void Update()
    {
        if (Input.GetKeyDown("t"))
        {
            ConnectElectric();
        }
    }

    public void ConnectElectric()
    {
        RaycastHit hit;

        Vector3 direction = transform.forward;

        // 레이캐스트
        if (Physics.Raycast(transform.position, direction, out hit, rayDistance))
        {
            // 가장 처음에 맞은 오브젝트를 가져오기
            GameObject hitObject = hit.collider.gameObject;
            GameObject electric = Instantiate(electricBolt, transform.position, Quaternion.identity);
            electric.GetComponent<LightningBoltScript>().StartObject = this.gameObject;
            electric.GetComponent<LightningBoltScript>().EndObject = hitObject;
            SecondStage.instance.CreateElectricBolt(electric);
            if (SecondStage.instance.CheckingLoop(hitObject))
            {
                hitObject.GetComponent<TransmissionTower>().ElectricBolt();
            }
            // 히트한 오브젝트의 정보 출력
            Debug.Log("Hit Object: " + hitObject.name);
        }
        else
        {
            Debug.Log("No object hit");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.up * -1 * rayDistance;
        Gizmos.DrawLine(transform.position, transform.position + direction);
    }
}
