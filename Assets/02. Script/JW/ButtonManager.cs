using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager instance;

    private void Start()
    {
        instance = this;
    }

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
        //게임 멈춤 풀고 싶으면
        Time.timeScale = 1;
    }

    //창 열기 버튼
    public void Open(GameObject screen)
    {
        screen.SetActive(true);
        //게임 멈추게 하고 싶으면
        Time.timeScale = 0;
    }
}
