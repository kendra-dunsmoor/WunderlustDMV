using UnityEngine;
using UnityEngine.SceneManagement;

public class ApartmentManager : MonoBehaviour
{
    private AudioManager audioManager;
    private GameManager gameManager;

    [SerializeField] GameObject computerScreenPanel;
    void Start()
    {
        audioManager = 	FindFirstObjectByType<AudioManager>();
        // Don't restart music if it is already playing from main menu:
        if (audioManager != null && !audioManager.isMusicClipPlaying(audioManager.apartmentMusic))
            audioManager.PlayMusic(audioManager.apartmentMusic);
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public void StartRun() {
        if (audioManager != null) audioManager.PlaySFX(audioManager.openDoor);
        gameManager.StartRun();
        if (gameManager.InTutorial()) SceneManager.LoadSceneAsync(2);
        else SceneManager.LoadSceneAsync(3);
    }

    public void OpenComputer() {
        if (audioManager != null) audioManager.PlaySFX(audioManager.computerBootUp);
        GameObject menu = Instantiate(computerScreenPanel, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
    }
}
