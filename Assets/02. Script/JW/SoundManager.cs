using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    //아래의 변수는 inspector창에서 slider를 추가할 수 있는 창입니다
    //AudioMixer에는 구성한 오디오 믹서를 넣고 Slider에는 메인 사운드, bgm, sfx 사운드 slider를 넣으면
    //slider를 움직임에 따라 사운드 믹서를 조절할 수 있게 됩니다.
    [SerializeField] private AudioMixer m_AudioMixer;
    [SerializeField] private Slider m_MusicMasterSlider;
    [SerializeField] private Slider m_MusicBGMSlider;
    [SerializeField] private Slider m_MusicSFXSlider;
    private GameObject soundSet;
    

    public static SoundManager instance;

    float mastervalue;
    float bgmvalue;
    float sfxvalue;

    private void Awake()
    {
        FindSlider();
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }
        m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        m_MusicBGMSlider.onValueChanged.AddListener(SetMusicVolume);
        m_MusicSFXSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void FindSlider()
    {
        m_MusicMasterSlider = GameObject.Find("MainSound").GetComponent<Slider>();
        m_MusicBGMSlider = GameObject.Find("BGMSound").GetComponent<Slider>();
        m_MusicSFXSlider = GameObject.Find("SFXSound").GetComponent<Slider>();
        soundSet = GameObject.Find("SoundSet");
        m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        m_MusicBGMSlider.onValueChanged.AddListener(SetMusicVolume);
        m_MusicSFXSlider.onValueChanged.AddListener(SetSFXVolume);
        m_MusicMasterSlider.value = mastervalue;
        m_MusicBGMSlider.value = bgmvalue;
        m_MusicSFXSlider.value = sfxvalue;
        soundSet.SetActive(false);
    }

    public void UpdateSound()
    {
        mastervalue = m_MusicMasterSlider.value;
        bgmvalue = m_MusicBGMSlider.value;
        sfxvalue = m_MusicSFXSlider.value;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("master") == false)
        {
            m_MusicMasterSlider.value = .5f;
            mastervalue = .5f;
            m_AudioMixer.SetFloat("Master", Mathf.Log10(mastervalue) * 20);
            PlayerPrefs.SetFloat("master", mastervalue);
        }
        else
        {
            m_MusicMasterSlider.value = PlayerPrefs.GetFloat("master");
            mastervalue = m_MusicMasterSlider.value;
            m_AudioMixer.SetFloat("Master", Mathf.Log10(mastervalue) * 20);
        }

        if (PlayerPrefs.HasKey("bgm") == false)
        {
            m_MusicBGMSlider.value = .5f;
            bgmvalue = .5f;
            m_AudioMixer.SetFloat("BGM", Mathf.Log10(mastervalue) * 20);
            PlayerPrefs.SetFloat("bgm", bgmvalue);
        }
        else
        {
            m_MusicBGMSlider.value = PlayerPrefs.GetFloat("bgm");
            bgmvalue = m_MusicBGMSlider.value;
            m_AudioMixer.SetFloat("BGM", Mathf.Log10(bgmvalue) * 20);
        }
        if (PlayerPrefs.HasKey("sfx") == false)
        {
            m_MusicSFXSlider.value = .5f;
            sfxvalue = .5f;
            m_AudioMixer.SetFloat("SFX", Mathf.Log10(mastervalue) * 20);
            PlayerPrefs.SetFloat("sfx", sfxvalue);
        }
        else
        {
            m_MusicSFXSlider.value = PlayerPrefs.GetFloat("sfx");
            sfxvalue = m_MusicSFXSlider.value;
            m_AudioMixer.SetFloat("SFX", Mathf.Log10(bgmvalue) * 20);
        }
    }

    public void SetMasterVolume(float volume)
    {
        m_AudioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("master", volume);
    }

    public void SetMusicVolume(float volume)
    {
        m_AudioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("bgm", volume);
    }

    public void SetSFXVolume(float volume)
    {
        m_AudioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfx", volume);
    }
}
