using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    SceneFader sceneFader;
    private AudioManager audioManager;
    private GameManager gameManager;

    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Start()
    {
        sceneFader = FindFirstObjectByType<SceneFader>();
    }

    public void BackToMainMenuButton()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        gameManager.RestartGame();
        if (sceneFader != null) sceneFader.LoadScene(0);
        else SceneManager.LoadSceneAsync(0);
    }

    public void ResumeGame()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        Destroy(gameObject);
    }
}
