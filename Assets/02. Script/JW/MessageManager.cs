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
        OnBottomMessage("이동키 : WASD\n" +
                                                "점프: SPACE BAR\n" +
                                                "시간 전환 : 버튼\n" +
                                                "상호 작용 : e키\n" +
                                                "Enter 키를 누르면 사라집니다.");
    }
    public void TutorialBottomMessage()
    {
        switch (DialogueNumber)
        {
            case 2:
                OnBottomMessage("과거에는 다리가 있었으나, 지금은 파괴되어 이 강을 건널 수 없습니다. (Enter)");
                break;
            case 1:
                OnBottomMessage("과거로 돌아가 필요한 조치를 취하면 현재의 강을 건널 수 있습니다. (Enter)");
                break;
        }
    }
    public void ElectricBottomMessage()
    {
        switch (DialogueNumber)
        {
            case 3:
                OnBottomMessage("옛날에 지어진 중계탑과 최근에 지어진 중계탑이 있습니다.(Enter)");
                break;
            case 2:
                OnBottomMessage("과거와 현재를 넘나들어 두 종류의 중계탑을 회전하여 전류를 연결하여 센서에 감지 시키세요. (Enter)");
                break;
            case 1:
                OnBottomMessage("중계탑 회전은 ‘@’키를 이용하여 회전할 수 있습니다. (Enter)");
                break;
        }
    }
    public void StoneBottomMessage()
    {
        switch (DialogueNumber)
        {
            case 4:
                OnBottomMessage("과거에는 한 줄로 연결되어 보존되었던 석상 모형들이 시간이 흘러 어지럽혀졌습니다. (Enter)");
                break;
            case 3:
                OnBottomMessage("시간 정류장을 이용해 어지럽혀진 석상 모형을 과거의 정돈된 상태로 배치해 주세요. (Enter)");
                break;
            case 2:
                OnBottomMessage("과거에서 현재로 되돌아오면 타일 위치가 초기화됩니다!(Enter)");
                break;
            case 1:
                OnBottomMessage("타일 이동은 ‘e’키를 이용하여 이동할 수 있습니다. (Enter)");
                break;
        }
    }
}
