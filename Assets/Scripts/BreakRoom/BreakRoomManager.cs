using UnityEngine;
using UnityEngine.SceneManagement;

public class BreakRoomManager : MonoBehaviour
{
    private AudioManager audioManager;
    void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        audioManager.PlayMusic(audioManager.breakRoomMusic);
    }
    public void StartShift()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        SceneManager.LoadSceneAsync(3);
    }

}
