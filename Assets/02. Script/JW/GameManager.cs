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

    private void Start()
    {
        SoundManager.instance.FindSlider();
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
            Debug.Log(stage);
        }
    }

}
