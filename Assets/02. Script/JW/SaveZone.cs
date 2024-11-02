using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveZone : MonoBehaviour
{
    public Vector3 spawnPosition;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(GameManager.instance.haveGem == 1)
            {
                GameManager.instance.gem1 = 1;
            }else if (GameManager.instance.haveGem == 2)
            {
                GameManager.instance.gem2 = 1;
            }else if (GameManager.instance.haveGem == 3)
            {
                GameManager.instance.gem3 = 1;
            }
            GameManager.instance.SaveGem();
            GameManager.instance.SaveStage(spawnPosition);
            Debug.Log("∞‘¿” ¿˙¿Âµ ");
        }
    }
}
