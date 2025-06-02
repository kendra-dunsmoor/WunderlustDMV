using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/*
* Audio Manager
* ~~~~~~~~~~~~~~
* Controls output for music and SFX
*/
public class AudioManager : MonoBehaviour
{
    [Header("-------------Audio Source-------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    
    
    [Header("-------------Audio Clip-------------")]
    public AudioClip breakRoomMusic;
    public AudioClip combatMusic;
    public AudioClip apartmentMusic;

    public AudioClip buttonClick;

    public static AudioManager instance;
    [SerializeField] private AudioMixer mixer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (breakRoomMusic != null) {
            musicSource.clip = breakRoomMusic;
            musicSource.Play();
        }
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null) {
            SFXSource.PlayOneShot(clip);
        }
    }
    
    public void PlayMusic(AudioClip clip)
    {
        if (clip != null) {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void StopSFX()
    {
        SFXSource.Stop();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
