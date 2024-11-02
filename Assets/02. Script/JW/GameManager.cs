using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int stage;
    public GameObject player;
    float mainVolume;
    float bgm;
    float sfx;
    public int gem1, gem2, gem3;
    Vector3 respawnPosition;
    public int haveGem;

    private void Start()
    {
        instance = this;
        SoundManager.instance.FindSlider();
        gem1 = 0;
        gem2 = 0;
        gem3 = 0;
    }

    public void SaveStage(Vector3 spawn)
    {
        PlayerPrefs.SetInt("Stage", stage);
        PlayerPrefs.SetFloat("X", spawn.x);
        PlayerPrefs.SetFloat("Y", spawn.y);
        PlayerPrefs.SetFloat("Z", spawn.z);
    }

    public void SaveGem()
    {
        PlayerPrefs.SetInt("Gem1", gem1);
        PlayerPrefs.SetInt("Gem2", gem2);
        PlayerPrefs.SetInt("Gem3", gem3);

    }
    
    public void LoadGem()
    {
        if (PlayerPrefs.HasKey("Gem1"))
        {
            gem1 = PlayerPrefs.GetInt("Gem1");
        }
        if (PlayerPrefs.HasKey("Gem2"))
        {
            gem2 = PlayerPrefs.GetInt("Gem2");
        }
        if (PlayerPrefs.HasKey("Gem3"))
        {
            gem3 = PlayerPrefs.GetInt("Gem3");
        }
    }

    public void LoadStage()
    {
        if (PlayerPrefs.HasKey("Stage"))
        {
            stage = PlayerPrefs.GetInt("Stage");
        }
        if (PlayerPrefs.HasKey("X"))
        {
            respawnPosition.x = PlayerPrefs.GetFloat("X");
        }
        if (PlayerPrefs.HasKey("Y"))
        {
            respawnPosition.y = PlayerPrefs.GetFloat("Y");
        }
        if (PlayerPrefs.HasKey("Z"))
        {
            respawnPosition.z = PlayerPrefs.GetFloat("Z");
        }
        player.transform.position = respawnPosition;
    }
    
    public void GameClear()
    {
        if(gem1 != 1 || gem2 != 1 || gem3 != 1)
        {
            Debug.Log("아직 보석이 다 모이지 않았습니다.");
            return;
        }

        Debug.Log("게임 클리어");
    }
    
}
