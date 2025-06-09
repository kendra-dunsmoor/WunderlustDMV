using UnityEngine;

public class BreakRoomManager : MonoBehaviour
{
    [SerializeField] SceneFader sceneFader;
    private AudioManager audioManager;

    void Awake()
    { 
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    void Start()
    {
        audioManager.PlayMusic(audioManager.breakRoomMusic);
        sceneFader.gameObject.SetActive(true);
    }
    public void StartShift()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.openDoor);
       sceneFader.LoadScene(3);
    }
}
