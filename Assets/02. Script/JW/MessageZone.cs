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
                MessageManager.instance.OnTopMessage("�ð� �������� ���� ���ſ� ���縦 �����鼭 ������ ȹ���ϼ���!", 0);
                MessageManager.instance.DialogueCategori = 2;
                MessageManager.instance.DialogueNumber = 2;
                MessageManager.instance.TutorialBottomMessage();
            }
            else if(stage == 2 && GameManager.instance.gem2 != 1)
            {
                MessageManager.instance.OnTopMessage("������ �����Ͽ� ������ �۵���Ű����!", 0);
                MessageManager.instance.DialogueCategori = 3;
                MessageManager.instance.DialogueNumber = 3;
                MessageManager.instance.ElectricBottomMessage();
            }
            else if (stage == 3 && GameManager.instance.gem3 != 1)
            {
                MessageManager.instance.OnTopMessage("Ÿ���� �����Ͽ� ������ ȹ���ϼ���!"+
                                "���ſ��� ����� �ǵ��ƿ��� Ÿ�� ��ġ�� �ʱ�ȭ�˴ϴ�!", 0);
                MessageManager.instance.DialogueCategori = 4;
                MessageManager.instance.DialogueNumber = 4;
                MessageManager.instance.StoneBottomMessage();
            }
        }
    }
}
