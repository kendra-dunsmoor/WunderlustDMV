using UnityEngine;

public class ApartmentManager : MonoBehaviour
{
    private AudioManager audioManager;
    private GameManager gameManager;

    [SerializeField] GameObject computerScreenPanel;
    [SerializeField] SceneFader sceneFader;
    [SerializeField] DialogueTrigger dialogueTrigger;

    void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        sceneFader.gameObject.SetActive(true);
    }

    void Start()
    {
        // Don't restart music if it is already playing from main menu:
        if (audioManager != null && !audioManager.isMusicClipPlaying(audioManager.apartmentMusic))
            audioManager.PlayMusic(audioManager.apartmentMusic);
        dialogueTrigger.TriggerDialogue();
    }

    public void StartRun() {
        if (audioManager != null) audioManager.PlaySFX(audioManager.openDoor);
        gameManager.StartRun();
        // TODO: Change back from 5 to 2 when done testing performance review
        if (gameManager.InTutorial()) sceneFader.LoadScene(2);
        else sceneFader.LoadScene(3);
    }

    public void OpenComputer() {
        if (audioManager != null) audioManager.PlaySFX(audioManager.computerBootUp);
        GameObject menu = Instantiate(computerScreenPanel, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
    }
}
