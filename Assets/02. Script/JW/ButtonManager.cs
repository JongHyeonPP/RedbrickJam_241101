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

    //�� �̵� ��ư
    public void MoveScene(string Scene)
    {
        //�� �̸� �ֱ�
        SceneManager.LoadScene(Scene);
    }

    //���� ���� ��ư
    public void ExitGame()
    {
        Application.Quit();
    }

    //â �ݱ� ��ư
    public void Quit(GameObject screen)
    {
        screen.SetActive(false);
        //���� ���� Ǯ�� ������
        Time.timeScale = 1;
    }

    //â ���� ��ư
    public void Open(GameObject screen)
    {
        screen.SetActive(true);
        //���� ���߰� �ϰ� ������
        Time.timeScale = 0;
    }
}
