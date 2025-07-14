using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingMenuScript : MonoBehaviour, IEventSystemHandler
{
    public AudioMixer AudioMixer;
    public AudioSource ClickSFX;
    public TMP_Dropdown ResolutionDropdown;
    public Toggle FullscreenToggle;

    public float FadeDuration = 1.0f; //handle value fade delay
    public float DelayTime = -0.05f; //Exit button click SFX

    /*resolution options filter*/
    private List<Vector2Int> _mainResolutions = new List<Vector2Int>()
    {
        new Vector2Int(2560, 1440),
        new Vector2Int(1920, 1080),
        new Vector2Int(1366, 768)
    };
    private List<Resolution> _filteredResolutions;
    private Resolution[] _resolutions;

    private Slider[] _volumeSliders;
    private string[] _volumeNames = { "Master", "Music", "SFX" };
    private bool _clickSFXUnlocked;

    // Start is called before the first frame update
    void Start()
    {
        /*graphic resolution settings*/
        _resolutions = Screen.resolutions; 
        Array.Reverse(_resolutions);

        _filteredResolutions = new List<Resolution>();

        ResolutionDropdown.ClearOptions();

        double currentRefreshRate = Math.Round(Screen.currentResolution.refreshRateRatio.value);

        foreach (Resolution resolution in _resolutions)
        {
            if (_mainResolutions.Contains(new Vector2Int(resolution.width, resolution.height)) &&
                Math.Round(resolution.refreshRateRatio.value) == currentRefreshRate)
            {
                _filteredResolutions.Add(resolution);
            }

        }

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < _filteredResolutions.Count; i++)
        {
            string option = _filteredResolutions[i].width + " x " + _filteredResolutions[i].height;
            options.Add(option);

            if (Screen.currentResolution.width == _filteredResolutions[i].width &&
                Screen.currentResolution.height == _filteredResolutions[i].height)
            {
                currentResolutionIndex = i;
            }
        }

        ResolutionDropdown.AddOptions(options);
        ResolutionDropdown.value = currentResolutionIndex;
        ResolutionDropdown.RefreshShownValue();



        /*volume siliders reference*/
        _volumeSliders = GetComponentsInChildren<Slider>();

        /*prevent indicator display problem*/
        foreach (var slider in _volumeSliders)
        {
            slider.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        }


        /*load player customized settings*/
        LoadKeys();

        _clickSFXUnlocked = true;

        /*Debug*/
        Debug.Log("Time Scale is:" + Time.timeScale);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnClickExit();
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        ApplyResolution(resolutionIndex);
        PlayClickSFX(_clickSFXUnlocked);
        PlayerPrefs.SetInt("Resolution", resolutionIndex);

    }

    public void ApplyResolution(int resolutionIndex)
    {

        Resolution resolution = _filteredResolutions[resolutionIndex];

        if (ResolutionDropdown.value != 0)
        {
            FullscreenToggle.isOn = false;
        }

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen = false);

    }

    public void SetFullscreen (bool isFullscreen)
    {
        if (ResolutionDropdown.value != 0 && isFullscreen)
        {
            ResolutionDropdown.value = 0;
        }

        FullscreenToggle.isOn = isFullscreen;
        Screen.fullScreen = isFullscreen;
        PlayClickSFX(_clickSFXUnlocked);
        PlayerPrefs.SetInt("Fullscreen", Convert.ToInt32(isFullscreen));

        if (isFullscreen)
        {
            Debug.Log("Activate Fullscreen ");

        }
        else
        {
            Debug.Log("Deactivate Fullscreen ");
        }

    }

    public void SetMasterVolume(float volume)
    {
        SetVolume("Master", volume);

    }

    public void SetMusicVolume(float volume)
    {
        SetVolume("Music", volume);

    }

    public void SetSFXVolume(float volume)
    {
        SetVolume("SFX", volume);
    }

    private void SetVolume(string volumeName, float volume)
    {
        //set the value change of the slider to be logarithmically as audio mixer;
        //also set the Min/Max value of slider to 0.0001/1;
        AudioMixer.SetFloat(volumeName + " Volume", Mathf.Log10(volume) * 20);

        if (volume != 0.0001f)
        {
            OnClickMute(volumeName, false);
        }
        else
        {
            OnClickMute(volumeName, true);
        }

        for (int i = 0; i < _volumeSliders.Length; i++)
        {
            if (volumeName == _volumeNames[i])
            {
                _volumeSliders[i].GetComponentInChildren<TextMeshProUGUI>().text = 
                    (Convert.ToInt32(Mathf.Lerp(0.0001f, 1f, volume) * 100f)).ToString();
                PlayerPrefs.SetFloat(volumeName + " Volume", volume);

                return;
            }
            
        }

        
    }

    public void OnDragMaster()
    {
        OnDrag("Master");
    }

    public void OnDragMusic()
    {
        OnDrag("Music");
    }

    public void OnDragSFX()
    {
        OnDrag("SFX");
    }

    public void OnDragEnd()
    {
        StartCoroutine(HandleValueFadeDuration());

    }

    private void OnDrag(string sliderName)
    {
        for (int i = 0; i < _volumeNames.Length; i++)
        {
            if (sliderName == _volumeNames[i])
            {
                _volumeSliders[i].GetComponentInChildren<TextMeshProUGUI>().enabled = true;

                return;
            }
        }
    }

    public void PlayClickSFX(bool clickSFXUnlocked)
    {
        if (clickSFXUnlocked)
        {
            ClickSFX.Play();
        }
        
    }

    public void OnClickMuteMaster(bool isMuted)
    {
        OnClickMute("Master", isMuted);
    }

    public void OnClickMuteMusic(bool isMuted)
    {
        OnClickMute("Music", isMuted);
    }

    public void OnClickMuteSFX(bool isMuted)
    {
        OnClickMute("SFX", isMuted);
    }

    private void OnClickMute(string toggleName, bool isMuted)
    {
        for (int i = 0; i < _volumeNames.Length; i++)
        {
            if(toggleName == _volumeNames[i])
            {
                GetComponentsInChildren<Toggle>()[i+1].isOn = isMuted; ////Toggle[0] is FullScreen;

                if (isMuted)
                {
                    AudioMixer.SetFloat(toggleName + " Volume", -80);
                    _volumeSliders[i].value = 0.0001f;
                }
                else if (!isMuted && _volumeSliders[i].value == 0.0001f)
                {
                    AudioMixer.SetFloat(toggleName + " Volume", -80);
                    _volumeSliders[i].value = 1.0f;
                }

                PlayClickSFX(_clickSFXUnlocked);
                PlayerPrefs.SetInt(toggleName + " Mute", Convert.ToInt32(isMuted));

                return;
            }
        }
    }
    public void OnClickExit()
    {
        if (_clickSFXUnlocked)
        {
            StartCoroutine(LoadSceneAfterSFX());

        }
    }

    public IEnumerator HandleValueFadeDuration()
    {
        yield return new WaitForSecondsRealtime(FadeDuration); //avoid yield return and timescale conflict

        foreach (var slider in _volumeSliders)
        {
            slider.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        }

    }

    private IEnumerator LoadSceneAfterSFX()
    {

        ClickSFX.Play();
        yield return new WaitForSecondsRealtime(ClickSFX.clip.length + DelayTime); //avoid yield return and timescale conflict

        gameObject.SetActive(false);
    }

    private void LoadKeys()
    {
        _clickSFXUnlocked = false;

        ApplyResolution(PlayerPrefs.GetInt("Resolution"));
        ResolutionDropdown.value = PlayerPrefs.GetInt("Resolution");

        bool isFullscreen = Convert.ToBoolean(PlayerPrefs.GetInt("Fullscreen"));
        SetFullscreen(isFullscreen);

        _volumeSliders[0].value = PlayerPrefs.GetFloat("Master Volume");
        _volumeSliders[1].value = PlayerPrefs.GetFloat("Music Volume");
        _volumeSliders[2].value = PlayerPrefs.GetFloat("SFX Volume");


        bool isMutedMaster = Convert.ToBoolean(PlayerPrefs.GetInt("Master Mute")); OnClickMuteMaster(isMutedMaster);
        bool isMutedMusic  = Convert.ToBoolean(PlayerPrefs.GetInt("Music Mute"));  OnClickMuteMusic (isMutedMusic);
        bool isMutedSFX    = Convert.ToBoolean(PlayerPrefs.GetInt("SFX Mute"));    OnClickMuteSFX   (isMutedSFX);
    }

    [ContextMenu("Reset Settings")]
    private void ResetKeys()
    {
        PlayerPrefs.SetInt("InitialStart", 0);

    }
}
