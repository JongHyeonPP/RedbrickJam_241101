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

    [SerializeField]AudioSource audioSource;

    [SerializeField] AudioClip buttonSound;
    [SerializeField] AudioClip landSound;
    [SerializeField] AudioClip itemSound;
    [SerializeField] AudioClip pushSound;
    [SerializeField] AudioClip[] walkSounds;
    private float lastRunSoundTime;
    private void Awake()
    {
        //FindSlider();
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }
    }
    public void FindSlider()
    {
        if(m_MusicMasterSlider == null)
        {
            m_MusicMasterSlider = GameObject.Find("MainSound").GetComponent<Slider>();
            m_MusicBGMSlider = GameObject.Find("BGMSound").GetComponent<Slider>();
            m_MusicSFXSlider = GameObject.Find("SFXSound").GetComponent<Slider>();
            soundSet = GameObject.Find("SoundSet");
        }
        m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        m_MusicBGMSlider.onValueChanged.AddListener(SetMusicVolume);
        m_MusicSFXSlider.onValueChanged.AddListener(SetSFXVolume);
        m_MusicMasterSlider.value = PlayerPrefs.GetFloat("master");
        m_MusicBGMSlider.value = PlayerPrefs.GetFloat("bgm");
        m_MusicSFXSlider.value = PlayerPrefs.GetFloat("sfx");
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
            if (!m_MusicMasterSlider)
            {
                m_MusicMasterSlider.value = .5f;
                mastervalue = .5f;
                m_AudioMixer.SetFloat("Master", Mathf.Log10(mastervalue) * 20);
                PlayerPrefs.SetFloat("master", mastervalue);
            }
        }
        else
        {
            if (m_MusicMasterSlider)
            {
                m_MusicMasterSlider.value = PlayerPrefs.GetFloat("master");
                mastervalue = m_MusicMasterSlider.value;
                m_AudioMixer.SetFloat("Master", Mathf.Log10(mastervalue) * 20);
            }
        }

        if (PlayerPrefs.HasKey("bgm") == false)
        {
            if (m_MusicBGMSlider)
            {
                m_MusicBGMSlider.value = .5f;
                bgmvalue = .5f;
                m_AudioMixer.SetFloat("BGM", Mathf.Log10(mastervalue) * 20);
                PlayerPrefs.SetFloat("bgm", bgmvalue);
            }
        }
        else
        {
            if (m_MusicBGMSlider)
            {
                m_MusicBGMSlider.value = PlayerPrefs.GetFloat("bgm");
                bgmvalue = m_MusicBGMSlider.value;
                m_AudioMixer.SetFloat("BGM", Mathf.Log10(bgmvalue) * 20);
            }
        }
        if (PlayerPrefs.HasKey("sfx") == false)
        {
            if (m_MusicSFXSlider)
            {
                m_MusicSFXSlider.value = .5f;
                sfxvalue = .5f;
                m_AudioMixer.SetFloat("SFX", Mathf.Log10(mastervalue) * 20);
                PlayerPrefs.SetFloat("sfx", sfxvalue);
            }
        }
        else
        {
            if (m_MusicSFXSlider)
            {
                m_MusicSFXSlider.value = PlayerPrefs.GetFloat("sfx");
                sfxvalue = m_MusicSFXSlider.value;
                m_AudioMixer.SetFloat("SFX", Mathf.Log10(bgmvalue) * 20);
            }
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
    public void PlaySoundEffect(string soundName, float value = 1)
    {
        switch (soundName)
        {  
            case "run":
                PlayRandomWalkSound();     
                break;
            case "land":
                audioSource.PlayOneShot(landSound, value);
                break;
            case "button":
                audioSource.PlayOneShot(buttonSound, value);
                break;
            case "push":
                audioSource.PlayOneShot(pushSound, value);
                break;
            case "item":
                audioSource.PlayOneShot(itemSound, value);
                break;
            default:
                Debug.LogWarning("Sound effect not found: " + soundName);
                break;
        }
    }
    private void PlayRandomWalkSound()
    {
        if (Time.time - lastRunSoundTime >= 0.5f)
        {
            int randomIndex = Random.Range(0, walkSounds.Length);
            audioSource.PlayOneShot(walkSounds[randomIndex]);
            lastRunSoundTime = Time.time;
        }
    }
}
