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
        SceneManager.LoadSceneAsync(3);
    }

}
