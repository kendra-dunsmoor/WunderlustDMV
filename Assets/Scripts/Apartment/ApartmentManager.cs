using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

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
        List<Certificate> playerCerts = gameManager.FetchCertificates();

        // Landlord takes rest of soul credits
        int rent = gameManager.FetchSoulCredits();
         if (playerCerts.Any(c => c.type == FINANCIAL_LITERACY))
		{
            if (rent>10) rent -= 10;
            else rent = 0;
		}

        gameManager.UpdateSoulCredits(-rent);

        gameManager.StartRun(); // TODO: Need to update to use rent 

        if (gameManager.InTutorial()) SceneManager.LoadSceneAsync(2);
        else SceneManager.LoadSceneAsync(3);
    }

    public void OpenComputer() {
        if (audioManager != null) audioManager.PlaySFX(audioManager.computerBootUp);
        GameObject menu = Instantiate(computerScreenPanel, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
    }
}
