using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public int stoneNum;


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            FirstStage.instance.stoneNum = stoneNum;
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
