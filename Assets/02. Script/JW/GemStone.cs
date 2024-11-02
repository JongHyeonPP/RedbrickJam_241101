using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemStone : MonoBehaviour
{
    public int gemNum;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            GameManager.instance.haveGem = gemNum;
        }
    }
}
