using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int stage;
    float mainVolume;
    float bgm;
    float sfx;
    bool[] gem;

    private void Start()
    {
        SoundManager.instance.FindSlider();
        gem = new bool[3];
    }

    public void NextStage()
    {
        stage++;
        SaveStage();
    }

    public void SaveStage()
    {
        PlayerPrefs.SetInt("Stage", stage);
    }

    public void LoadStage()
    {
        if (PlayerPrefs.HasKey("Stage"))
        {
            stage = PlayerPrefs.GetInt("Stage");
            for (int i = 0; i <= stage; i++)
            {
                gem[i] = true;
            }
            Debug.Log(stage);
        }
    }
    
    public void GameClear()
    {
        for (int i = 0; i < gem.Length; i++)
        {
            if (gem[i] == false)
            {
                Debug.Log("���� ������ �� ������ ���߽��ϴ�.");
                return;
            }
        }

        Debug.Log("���� Ŭ����");
    }

    public void TestingGem()
    {
        for (int i = 0; i < gem.Length; i++)
        {
            gem[i] = true;
        }
    }
    
}
