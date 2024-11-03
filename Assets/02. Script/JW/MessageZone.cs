using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageZone : MonoBehaviour
{
    public int stage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(stage == 1 && GameManager.instance.gem1 != 1)
            {
                MessageManager.instance.OnTopMessage("시간 정류장을 통해 과거와 현재를 오가면서 보석을 획득하세요!", 0);
                MessageManager.instance.DialogueCategori = 2;
                MessageManager.instance.DialogueNumber = 2;
                MessageManager.instance.TutorialBottomMessage();
            }
            else if(stage == 2 && GameManager.instance.gem2 != 1)
            {
                MessageManager.instance.OnTopMessage("전류를 연결하여 센서를 작동시키세요!", 0);
                MessageManager.instance.DialogueCategori = 3;
                MessageManager.instance.DialogueNumber = 3;
                MessageManager.instance.ElectricBottomMessage();
            }
            else if (stage == 3 && GameManager.instance.gem3 != 1)
            {
                MessageManager.instance.OnTopMessage("타일을 연결하여 보석을 획득하세요!"+
                                "과거에서 현재로 되돌아오면 타일 위치가 초기화됩니다!", 0);
                MessageManager.instance.DialogueCategori = 4;
                MessageManager.instance.DialogueNumber = 4;
                MessageManager.instance.StoneBottomMessage();
            }
        }
    }
}
