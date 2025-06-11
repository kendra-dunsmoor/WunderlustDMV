using UnityEngine;

public class EventController : MonoBehaviour
{
    [SerializeField] SceneFader sceneFader;
    [SerializeField] DialogueTrigger dialogueTrigger;

    private AudioManager audioManager;
    void Awake()
    { 
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    void Start()
    {
        if (audioManager != null) audioManager.PlayMusic(audioManager.breakRoomMusic);
        sceneFader.gameObject.SetActive(true);
        dialogueTrigger.TriggerDialogue();
    }

    public void NextShift() {
        if (audioManager != null) audioManager.PlaySFX(audioManager.openDoor);
        sceneFader.LoadScene(3);
    }
}
