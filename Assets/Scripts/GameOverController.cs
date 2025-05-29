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
        SceneManager.LoadSceneAsync(1); // just go to apartment for now
    }
}
