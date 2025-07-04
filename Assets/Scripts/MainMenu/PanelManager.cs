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

    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    public void NewGameButton()
    {
        Debug.Log("New game");
        audioManager.PlaySFX(audioManager.buttonClick);
        MenuManager manager = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MenuManager>();
        manager.LoadScene(7);
    }
    public void CreditsButton()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        MenuManager manager = GameObject.FindGameObjectWithTag("MenuManager").GetComponent<MenuManager>();
        manager.CreditsMenu();
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
