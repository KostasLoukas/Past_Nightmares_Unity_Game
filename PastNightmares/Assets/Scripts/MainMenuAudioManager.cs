using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


/*
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;

    [Range(0f, 0.5f)]
    public float randomizeVolume = 0f;
    [Range(0f, 0.5f)]
    public float randomizePitch = 0f;

    private AudioSource source;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
    }

    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-randomizeVolume/2f, randomizeVolume/2f));
        source.pitch = pitch * (1 + Random.Range(-randomizePitch/2f, randomizePitch/2f));
        source.Play();
    }
}
*/


public class MainMenuAudioManager : MonoBehaviour
{
    public AudioMixer theMixer;

    
    /*
    public static AudioManager instance;

    [SerializeField]
    Sound[] sounds;


    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one AudioManager exists in the scene!");
        }
        else
        {
            instance = this;
        }

    }
    */

    // Start is called before the first frame update
    void Start()
    {
        //Checking and loading the predetermined volume settings
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            theMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            theMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            theMixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume"));
        }


        /*
        for (int i = 0 ; i<sounds.Length ; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            //_go.transform.SetParent(this.Transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }
        */
        
    }

    
    /*
    public void PlaySound(string _name)
    {
        for (int i = 0 ; i<sounds.Length ; i++)
        {
            if (sounds[i].name == _name)  //found the correct sound to play (duplicated sound names will be skipped)
            {
                sounds[i].Play();
                return;
            }
        }

        //no sound with _name found
        Debug.LogWarning("AudioManager: Sound not found in the list! " + _name);
    }
    */
    
}
