using UnityEngine;

/*
* Menu Manager
* ~~~~~~~~~~~~~
* Controls overall Main Menu logic for switching between panels 
*/
public class MenuManager : MonoBehaviour
{
    [Header("-------------Menu Panels-------------")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject instructionsMenu;
    
    [Header("-------------Audio-------------")]
    [SerializeField] private AudioManager audioManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Starting game main menu");
        GameObject menu = Instantiate(mainMenu, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
    }

    public void BackToMainMenu(string Tag)
    {
        Destroy(GameObject.FindGameObjectWithTag(Tag));
        GameObject menu = Instantiate(mainMenu, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
    }
    
    public void OptionsMenu()
    {
        Destroy(GameObject.FindGameObjectWithTag("MenuPanel"));
        GameObject menu = Instantiate(optionsMenu, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
    }
    
    public void InstructionsMenu()
    {
        Destroy(GameObject.FindGameObjectWithTag("MenuPanel"));
        GameObject menu = Instantiate(instructionsMenu, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
    }
}
