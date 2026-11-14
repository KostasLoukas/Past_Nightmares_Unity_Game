using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class OptionsScreen : MonoBehaviour
{
    public Toggle fullscreenToggle, vsyncToggle;
    

    public List<ResolutionItem> resolutions = new List<ResolutionItem>();
    private int selectedResolution;
    public TMP_Text resolutionLabel;

    public AudioMixer theMixer;
    public TMP_Text masterVolumeLabel, musicVolumeLabel, sfxVolumeLabel;
    public Slider masterVolumeSlider, musicVolumeSlider, sfxVolumeSlider;


    // Start is called before the first frame update
    void Start()
    {
        fullscreenToggle.isOn = Screen.fullScreen;

        //If VSync is off
        if (QualitySettings.vSyncCount == 0)
        {
            vsyncToggle.isOn = false;
        }
        else
        {
            vsyncToggle.isOn = true;
        }

        //Find the correct resolution to display in the settings (Resolution Label)
        bool foundResolution = false;

        for (int i = 0 ; i<resolutions.Count ; i++)
        {
            if (Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical)
            {
                foundResolution = true;

                selectedResolution = i;

                UpdateResolutionLabel();
            }
        }

        //If the player is not using one of the available resolutions, add his resolution to the available ones
        if (!foundResolution)
        {
            ResolutionItem newResolution = new ResolutionItem();
            newResolution.horizontal = Screen.width;
            newResolution.vertical = Screen.height;

            resolutions.Add(newResolution);
            selectedResolution = resolutions.Count - 1;

            UpdateResolutionLabel();
        }


        //We have to store and put the value again on the slider for it to display correctly (not on the defaults)
        float volume = 0f;
        theMixer.GetFloat("MasterVolume", out volume);
        masterVolumeSlider.value = volume;

        theMixer.GetFloat("MusicVolume", out volume);
        musicVolumeSlider.value = volume;

        theMixer.GetFloat("SFXVolume", out volume);
        sfxVolumeSlider.value = volume;

        masterVolumeLabel.text = Mathf.RoundToInt(masterVolumeSlider.value + 80).ToString();

        musicVolumeLabel.text = Mathf.RoundToInt(musicVolumeSlider.value + 80).ToString();

        sfxVolumeLabel.text = Mathf.RoundToInt(sfxVolumeSlider.value + 80).ToString();

        

    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
    

    public void ResolutionLeft()
    {
        selectedResolution--;
        if (selectedResolution < 0)  //if it goes too left, wrap around to the last item of the list
        {
            selectedResolution = resolutions.Count - 1;
        }

        UpdateResolutionLabel();
    }


    public void ResolutionRight()
    {
        selectedResolution++;
        if (selectedResolution > resolutions.Count - 1)  //if it goes too right, wrap around to the first item of the list
        {
            selectedResolution = 0;
        }

        UpdateResolutionLabel();
    }


    public void UpdateResolutionLabel()
    {
        resolutionLabel.text = resolutions[selectedResolution].horizontal.ToString() + "X" + resolutions[selectedResolution].vertical.ToString();
    }


    //Apply graphics settings according to the toggles in-game
    public void ApplyGraphics()
    {
        //Screen.fullScreen = fullscreenToggle.isOn;

        if (vsyncToggle.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }

        Screen.SetResolution(resolutions[selectedResolution].horizontal, resolutions[selectedResolution].vertical, fullscreenToggle.isOn);
    }


    public void SetMasterVolume()
    {
        masterVolumeLabel.text = Mathf.RoundToInt(masterVolumeSlider.value + 80).ToString();

        theMixer.SetFloat("MasterVolume", masterVolumeSlider.value);

        //Save this preference throughout scenes
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
    }

    public void SetMusicVolume()
    {
        musicVolumeLabel.text = Mathf.RoundToInt(musicVolumeSlider.value + 80).ToString();

        theMixer.SetFloat("MusicVolume", musicVolumeSlider.value);

        //Save this preference throughout scenes
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
    }

    public void SetSFXVolume()
    {
        sfxVolumeLabel.text = Mathf.RoundToInt(sfxVolumeSlider.value + 80).ToString();

        theMixer.SetFloat("SFXVolume", sfxVolumeSlider.value);

        //Save this preference throughout scenes
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
    }
}


[System.Serializable]
public class ResolutionItem
{
    public int horizontal, vertical;
}