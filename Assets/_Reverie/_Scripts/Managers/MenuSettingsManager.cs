using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSettingsManager : MonoBehaviour
{
    public Slider volSliderMaster;
    public TextMeshProUGUI volDisplayMaster;
    public TMP_Dropdown resDropdown;
    public TMP_Dropdown qualityDropdown;
    public Toggle muteToggle;
    public AudioMixer masterMixer;
    public GameObject startPanel, pausePnl;
    public Button startSelectedButton;

    private Resolution[] resolutions;
    private float currentVolume, leanTime;
    private int currentQualityIndex = 0;
    private bool isMute = false;
    public String[] parameters, qualityLevels;

    private void Awake()
    {
        masterMixer = Resources.Load("MainMixer") as AudioMixer;
        resolutions = Screen.resolutions;
        qualityLevels = QualitySettings.names;
        currentQualityIndex = QualitySettings.GetQualityLevel();
        masterMixer.GetFloat("MasterVolume", out currentVolume);
    }

    void Start()
    {
        #region Resolutions
        if(resDropdown)
        {
            resDropdown.ClearOptions();

            List<string> options = new List<string>();

            int currentResIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResIndex = i;
                }
            }

            resDropdown.AddOptions(options);
            resDropdown.value = currentResIndex;
            resDropdown.RefreshShownValue();
        }
        #endregion

        #region Quality
        if(qualityDropdown)
        {
            qualityDropdown.ClearOptions();

            List<string> quality = new List<string>();

            for (int i = 0; i < qualityLevels.Length; i++)
            {
                quality.Add(qualityLevels[i]);
            }

            qualityDropdown.AddOptions(quality);
            qualityDropdown.value = currentQualityIndex;
            qualityDropdown.RefreshShownValue();
        }
        #endregion
    }

    public void SetVolume(float _volume)
    {
        masterMixer.SetFloat(parameters[0], _volume);

        float volValue = ((volSliderMaster.value) + 80);
        volValue = (volValue * 100) / 80;
        volDisplayMaster.text = volValue.ToString("F0");

        if (isMute)
        {
            muteToggle.isOn = false;
        }
    }

    public void SetQuality(int _qualityIndex)
    {
        QualitySettings.SetQualityLevel(_qualityIndex);
    }

    public void SetFullscreen(bool _isFullscreen)
    {
        Screen.fullScreen = _isFullscreen;
    }

    public void SetMute(bool _isMute)
    {
        if (_isMute)
        {
            masterMixer.SetFloat("MasterVolume", -80f);
            float volValue = 0;
            volValue = (volValue * 100) / 80;
            volDisplayMaster.text = volValue.ToString("F1");
            currentVolume = volSliderMaster.value;
            volSliderMaster.value = -80f;
        }
        else
        {
            masterMixer.SetFloat("MasterVolume", currentVolume);
            float volValue = ((currentVolume) + 80);
            volValue = (volValue * 100) / 80;
            volDisplayMaster.text = volValue.ToString("F0");
            volSliderMaster.value = currentVolume;
        }

        isMute = _isMute;
    }

    public void SetResolution(int _resIndex)
    {
        Resolution currentRes = resolutions[_resIndex];
        Screen.SetResolution(currentRes.width, currentRes.height, Screen.fullScreen);
    }
}
