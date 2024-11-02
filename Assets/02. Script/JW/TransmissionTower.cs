using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmissionTower : MonoBehaviour
{
    public int towerNum;
    public GameObject currentPrefab; // Àü·ù

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SecondStage.instance.towerNum = towerNum;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SecondStage.instance.towerNum = -1;
        }
    }

    public void FireCurrent(Vector3 direction)
    {
        GameObject current = Instantiate(currentPrefab, transform.position, Quaternion.identity);
        current.GetComponent<Current>().SetDirection(direction.normalized);
    }
}
