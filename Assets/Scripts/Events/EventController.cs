using UnityEngine;
using UnityEngine.SceneManagement;

public class EventController : MonoBehaviour
{
    private AudioManager audioManager;
    void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        audioManager.PlayMusic(audioManager.breakRoomMusic);
    }

    public void NextShift() {
        if (audioManager != null) audioManager.PlaySFX(audioManager.openDoor);
        SceneManager.LoadSceneAsync(3);
    }
}
