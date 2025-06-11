using UnityEngine;

public class BreakRoomManager : MonoBehaviour
{
    [SerializeField] SceneFader sceneFader;
    [SerializeField] GameObject rewardsScreen;
    [SerializeField] GameObject coffeeMachine;
    [SerializeField] Item coffeeItem;
    [SerializeField] DialogueTrigger dialogueTrigger;
    private AudioManager audioManager;
    private GameManager gameManager;

    void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Start()
    {
        audioManager.PlayMusic(audioManager.breakRoomMusic);
        sceneFader.gameObject.SetActive(true);

        // Verrine will give coffee to you in opening scene:
        if (gameManager.InTutorial() && gameManager.FetchCurrentCalendarDay() == 0) coffeeMachine.SetActive(false);
        dialogueTrigger.TriggerDialogue();
    }
    public void StartShift()
    {
        if (audioManager != null) audioManager.PlaySFX(audioManager.openDoor);
        sceneFader.LoadScene(3);
    }

    public void GetCoffee()
    {
        // Verify didn't already get a coffee
        if (coffeeMachine.activeSelf)
        {
            if (audioManager != null) audioManager.PlaySFX(audioManager.coffeePour);
            gameManager.AddToInventory(coffeeItem.ID);
            GameObject screen = Instantiate(rewardsScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
            screen.GetComponent<PopUpRewardController>().AddRewardInfo(coffeeItem.Icon, coffeeItem.itemName, coffeeItem.flavorText);
            coffeeMachine.SetActive(false);
        }
    }
    
}
