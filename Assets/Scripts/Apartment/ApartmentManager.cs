using UnityEngine;
using UnityEngine.SceneManagement;

public class ApartmentManager : MonoBehaviour
{
    private AudioManager audioManager;
    void Start()
    {
        audioManager = 	FindFirstObjectByType<AudioManager>();
        // TODO: system to check which dialogue to trigger
        gameObject.GetComponent<DialogueTrigger>().TriggerDialogue();
    }

    public void StartRun() {
        audioManager.PlaySFX(audioManager.buttonClick);
        SceneManager.LoadSceneAsync(2);
    }
}
