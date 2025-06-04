using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    private AudioManager audioManager;
    private GameManager gameManager;
    [SerializeField] TextMeshProUGUI resultText;

    void Start()
    {
        audioManager = 	FindFirstObjectByType<AudioManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null) {
            if (gameManager.FetchRunState() == GameState.RunStatus.REINCARNATED) {
                if (audioManager != null) audioManager.PlaySFX(audioManager.shiftOver_Reincarnated);
                resultText.text = "You did too well at work, you've been reincarnated";
            } else {
                if (audioManager != null) audioManager.PlaySFX(audioManager.shiftOver_Fired);
                resultText.text = "You've been fired for poor performance";
            }
        }
    }

    public void Restart()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        gameManager.RestartRun();
        SceneManager.LoadSceneAsync(1); // TODO: just go to apartment for now
    }

    public void ExitToMenu()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        SceneManager.LoadSceneAsync(0); // TODO: just go to main menu, need to add some reset logic later
    }
}
