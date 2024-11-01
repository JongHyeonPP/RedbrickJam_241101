using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    //씬 이동 버튼
    public void MoveScene(string Scene)
    {
        //씬 이름 넣기
        SceneManager.LoadScene(Scene);
    }

    //게임 종료 버튼
    public void ExitGame()
    {
        Application.Quit();
    }

    //창 닫기 버튼
    public void Quit(GameObject screen)
    {
        screen.SetActive(false);
    }
    
    //창 열기 버튼
    public void Open(GameObject screen)
    {
        screen.SetActive(true);
    }
}
