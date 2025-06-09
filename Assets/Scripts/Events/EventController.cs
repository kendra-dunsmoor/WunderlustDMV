using UnityEngine;

public class EventController : MonoBehaviour
{
    [SerializeField] SceneFader sceneFader;

    private AudioManager audioManager;
    void Awake()
    { 
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    void Start()
    {
        if (audioManager!= null) audioManager.PlayMusic(audioManager.breakRoomMusic);
        sceneFader.gameObject.SetActive(true);
    }

    public void NextShift() {
        if (audioManager != null) audioManager.PlaySFX(audioManager.openDoor);
        sceneFader.LoadScene(3);
    }
}
