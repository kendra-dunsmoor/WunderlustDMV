using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    private AudioManager audioManager;

    void Start()
    {
        audioManager = 	FindFirstObjectByType<AudioManager>();
    }

    public void Restart()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        SceneManager.LoadSceneAsync(1); // TODO: just go to apartment for now
    }

    public void ExitToMenu()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        SceneManager.LoadSceneAsync(0); // TODO: just go to main menu, need to add some reset logic later
    }
}
