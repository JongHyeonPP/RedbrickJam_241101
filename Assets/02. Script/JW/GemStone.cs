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
            GameManager.instance.Gem[gemNum - 1].SetActive(true);
            SoundManager.instance.PlaySoundEffect("item");
            if(GameManager.instance.gem1 == 1 && GameManager.instance.gem2 == 1 && GameManager.instance.gem3 == 1)
            {
                MessageManager.instance.OnTopMessage("골인 지점을 찾아 이동하세요!", 0);
            }
            else
            {
                MessageManager.instance.OnTopMessage("다음 보석을 찾아 이동하세요!", 5);
            }
        }
    }
}
