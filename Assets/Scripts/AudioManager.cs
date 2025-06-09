
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
    [SerializeField] AudioSource dialogueSource;
    
    
    [Header("-------------Music Audio Clips-------------")]
    public AudioClip breakRoomMusic;
    public AudioClip combatMusic;
    public AudioClip apartmentMusic;

    [Header("-------------SFX Audio Clips-------------")]
    public AudioClip buttonClick;
    public AudioClip acceptButton;
    public AudioClip rejectButton;
    public AudioClip specialActionButton;
    public AudioClip noEnergy;
    public AudioClip buyUpgrade;
    public AudioClip shiftOver_Success;
    public AudioClip shiftOver_Fired;
    public AudioClip shiftOver_Reincarnated;
    public AudioClip computerBootUp;
    public AudioClip vendingMachineItem;
    public AudioClip paperRustle;
    public AudioClip drink;
    public AudioClip openDoor;

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
        if (apartmentMusic != null) {
            musicSource.clip = apartmentMusic;
            musicSource.Play();
        }
    }

    public bool isMusicClipPlaying(AudioClip clip)
    {
        return musicSource.clip == clip;
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
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
    public void PlayDialogue(AudioClip clip)
    {
        if (clip != null) {
            dialogueSource.clip = clip;
            dialogueSource.Play();
        }
    }

    public void StopDialogue()
    {
        dialogueSource.Stop();
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
