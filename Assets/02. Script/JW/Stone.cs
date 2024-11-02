using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public int stoneNum;

    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            FirstStage.instance.stoneNum = stoneNum;
            Debug.Log("µé¾î¿È");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            FirstStage.instance.stoneNum = -1;
        }
    }
}
