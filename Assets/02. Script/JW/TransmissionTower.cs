using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;

public class TransmissionTower : MonoBehaviour
{
    public int towerNum;
    public float rayDistance = 100f;
    public GameObject electricBolt; // 전류

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SecondStage.instance.towerNum = towerNum;
            Debug.Log("들어옴");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SecondStage.instance.towerNum = -1;
        }
    }

    public void ElectricBolt()
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
            hitObject.GetComponent<TransmissionTower>().ElectricBolt();
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
        Vector3 direction = transform.forward * rayDistance;
        Gizmos.DrawLine(transform.position, transform.position + direction);
    }
}
