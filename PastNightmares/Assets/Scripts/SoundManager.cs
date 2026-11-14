using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this class is a singleton class which means that only ONE instance of this class can exist each time
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioSource masterSource, musicSource, sfxSource, ambientSource;
    private bool isPlayingMusicSource = true;
    private float maxMusicVolume = 0.19f;



    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] //resolve gc handle fix
    static void Init()
    {
        instance = null;
    }


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //this instance will preserve even if we change scenes
        }
        else
        {
            Destroy(gameObject); //if an instance already exists, destroy it
        }
    }

    
    public void PlaySFXSound(AudioClip _clip)
    {
        sfxSource.PlayOneShot(_clip);
    }


    public void StopSFXSound(AudioClip _clip)
    {
        sfxSource.clip = _clip;
        sfxSource.Stop();
    }
    

    public void PlayMusicSound(AudioClip _clip)
    {
        musicSource.clip = _clip;
        musicSource.volume = maxMusicVolume;
        musicSource.loop = true; //The level's background music will loop forever until stopped.
        musicSource.Play();
    }


    public void PauseMusic()
    {
        musicSource.Pause();
    }


    public void ResumeMusic()
    {
        musicSource.Play();
    }


    public void StopMusicSound(AudioClip _clip)
    {
        musicSource.clip = _clip;
        musicSource.Stop();
    }


    public void SwapTrack(AudioClip clip)
    {
        StopCoroutine(FadeTrack(clip)); //in case it was running

        StartCoroutine(FadeTrack(clip));

        isPlayingMusicSource = !isPlayingMusicSource;
    }

    public IEnumerator FadeTrack (AudioClip newClip)
    {
        float timeToFade = 1f;
        float timeElapsed = 0;

        if (isPlayingMusicSource)
        {
            ambientSource.clip = newClip;
            ambientSource.volume = maxMusicVolume;
            ambientSource.loop = true;
            ambientSource.Play();
            
            while (timeElapsed < timeToFade) //fade audio between tracks
            {
                ambientSource.volume = Mathf.Lerp(0, maxMusicVolume, timeElapsed/timeToFade);
                musicSource.volume = Mathf.Lerp(maxMusicVolume, 0, timeElapsed/timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            musicSource.Pause();
        }
        else
        {
            musicSource.clip = newClip;
            musicSource.volume = maxMusicVolume;
            musicSource.loop = true;
            musicSource.Play();

            while (timeElapsed < timeToFade) //fade audio between tracks
            {
                musicSource.volume = Mathf.Lerp(0, maxMusicVolume, timeElapsed/timeToFade);
                ambientSource.volume = Mathf.Lerp(maxMusicVolume, 0, timeElapsed/timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            ambientSource.Pause();
        }

    }
}
