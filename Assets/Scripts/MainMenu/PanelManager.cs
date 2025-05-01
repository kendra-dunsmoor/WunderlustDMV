using UnityEngine;
using UnityEngine.SceneManagement;

/*
* Panel Manager
* ~~~~~~~~~~~~~
* Attached to individual UI panel prefabs.
* Responsible for connecting prefab panel buttons to main game logic  
*/
public class PanelManager : MonoBehaviour
{
    private AudioManager audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    public void NewGameButton()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        SceneManager.LoadSceneAsync(1);
    }

    public void OptionsButton()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        MenuManager manager = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MenuManager>();
        manager.OptionsMenu();
    }
    public void BackToMainMenuButton()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        MenuManager manager = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MenuManager>();
        manager.BackToMainMenu(gameObject.tag);
    }

    public void InstructionsButton()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        MenuManager manager = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MenuManager>();
        manager.InstructionsMenu();
    }
}
