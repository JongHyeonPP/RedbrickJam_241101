using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricTower : MonoBehaviour
{
    public GameObject currentPrefab; // 전류

    public void FireCurrent()
    {
        Vector3 direction = new Vector3(0,0,-3);
        GameObject current = Instantiate(currentPrefab, transform.position, Quaternion.identity);
        current.GetComponent<Current>().SetDirection(direction.normalized);
        Debug.Log("발사");
        Debug.Log(current.transform.position);
    }
}
