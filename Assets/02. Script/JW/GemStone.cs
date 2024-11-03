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
                MessageManager.instance.OnTopMessage("���� ������ ã�� �̵��ϼ���!", 0);
            }
            else
            {
                MessageManager.instance.OnTopMessage("���� ������ ã�� �̵��ϼ���!", 5);
            }
        }
    }
}
