using UnityEngine;
using UnityEngine.SceneManagement;

public class ApartmentManager : MonoBehaviour
{
    private AudioManager audioManager;

    [SerializeField] GameObject computerScreenPanel;
    void Start()
    {
        audioManager = 	FindFirstObjectByType<AudioManager>();
    }

    public void StartRun() {
        if (audioManager != null) audioManager.PlaySFX(audioManager.buttonClick);
        SceneManager.LoadSceneAsync(2);
    }

    public void OpenComputer() {
        GameObject menu = Instantiate(computerScreenPanel, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
    }
}
