using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBridgeCollider : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            FirstStage.instance.isBuild = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            FirstStage.instance.isBuild = false;
        }
    }
}
