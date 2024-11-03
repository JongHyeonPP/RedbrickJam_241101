using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public GameObject[] GemStone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        GameManager.instance.LoadStage();
        GameManager.instance.LoadGem();
        GameManager.instance.haveGem = 0;
        if (GameManager.instance.gem1 != 1)
        {
            GemStone[0].SetActive(true);
            GameManager.instance.Gem[0].SetActive(false);
        }/*else if (GameManager.instance.gem2 != 1)
            {
                GemStone[1].SetActive(true);
            }else if (GameManager.instance.gem2 != 1)
            {
                GemStone[2].SetActive(true);
            }*/
    }
}
