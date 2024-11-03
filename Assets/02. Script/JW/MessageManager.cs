using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageManager : MonoBehaviour
{
    public static MessageManager instance;
    public TextMeshProUGUI topText;
    public GameObject topMessageScreen;

    public TextMeshProUGUI bottomText;
    public GameObject bottomMessageScreen;
    public int DialogueNumber;
    public int DialogueCategori;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && DialogueNumber >= 1)
        {
            NextBottomMessage();
        }
    }

    public void OnTopMessage(string message, int time)
    {
        topMessageScreen.SetActive(true);
        topText.text = message;
        if(time >= 5)
        {
            Invoke("OffTopMessage", time);
        }
    }

    public void OffTopMessage()
    {
        topMessageScreen.SetActive(false);
    }

    public void OnBottomMessage(string message)
    {
        bottomMessageScreen.SetActive(true);
        bottomText.text = message;
    }

    public void OffBottomMessage()
    {
        bottomMessageScreen.SetActive(false);
    }
    public void NextBottomMessage()
    {
        DialogueNumber--;
        if(DialogueNumber == 0)
        {
            OffBottomMessage();
        }else if(DialogueCategori == 1)
        {
            StartBottomMessage();
        }else if (DialogueCategori == 2)
        {
            TutorialBottomMessage();
        }else if (DialogueCategori == 3)
        {
            ElectricBottomMessage();
        }else if (DialogueCategori == 4)
        {
            StoneBottomMessage();
        }
    }

    public void StartBottomMessage()
    {
        DialogueNumber = 1;
        OnBottomMessage("�̵�Ű : WASD\n" +
                                                "����: SPACE BAR\n" +
                                                "�ð� ��ȯ : ��ư\n" +
                                                "��ȣ �ۿ� : eŰ\n" +
                                                "Enter Ű�� ������ ������ϴ�.");
    }
    public void TutorialBottomMessage()
    {
        switch (DialogueNumber)
        {
            case 2:
                OnBottomMessage("���ſ��� �ٸ��� �־�����, ������ �ı��Ǿ� �� ���� �ǳ� �� �����ϴ�. (Enter)");
                break;
            case 1:
                OnBottomMessage("���ŷ� ���ư� �ʿ��� ��ġ�� ���ϸ� ������ ���� �ǳ� �� �ֽ��ϴ�. (Enter)");
                break;
        }
    }
    public void ElectricBottomMessage()
    {
        switch (DialogueNumber)
        {
            case 3:
                OnBottomMessage("������ ������ �߰�ž�� �ֱٿ� ������ �߰�ž�� �ֽ��ϴ�.(Enter)");
                break;
            case 2:
                OnBottomMessage("���ſ� ���縦 �ѳ���� �� ������ �߰�ž�� ȸ���Ͽ� ������ �����Ͽ� ������ ���� ��Ű����. (Enter)");
                break;
            case 1:
                OnBottomMessage("�߰�ž ȸ���� ��@��Ű�� �̿��Ͽ� ȸ���� �� �ֽ��ϴ�. (Enter)");
                break;
        }
    }
    public void StoneBottomMessage()
    {
        switch (DialogueNumber)
        {
            case 4:
                OnBottomMessage("���ſ��� �� �ٷ� ����Ǿ� �����Ǿ��� ���� �������� �ð��� �귯 �������������ϴ�. (Enter)");
                break;
            case 3:
                OnBottomMessage("�ð� �������� �̿��� ���������� ���� ������ ������ ������ ���·� ��ġ�� �ּ���. (Enter)");
                break;
            case 2:
                OnBottomMessage("���ſ��� ����� �ǵ��ƿ��� Ÿ�� ��ġ�� �ʱ�ȭ�˴ϴ�!(Enter)");
                break;
            case 1:
                OnBottomMessage("Ÿ�� �̵��� ��e��Ű�� �̿��Ͽ� �̵��� �� �ֽ��ϴ�. (Enter)");
                break;
        }
    }
}
