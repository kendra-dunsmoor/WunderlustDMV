using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

/*
* Dialogue Manager
* ~~~~~~~~~~~~~~~~~
* Manages display of text in dialogue box
*/
// TODO:
// Character picture/title needs to be optional since we have popups for environment objects too
// Check for rewards related to certain dialogue lines and update player items 
// Change box/UI used depending on if event dialogue or not? i.e. bottom of screen vs whole screen
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
	private AudioManager audioManager;
    private GameManager gameManager;

    [Header("------------- Temp Tutorials -------------")]
    [SerializeField] private TutorialManager tutorialManager; // temp
    [SerializeField] private TutorialDialogue tutorial; // temp

    [Header("------------- Dialogue -------------")]
    // UI references
    [SerializeField] private GameObject DialogueParent; // Main container for dialogue UI
    [SerializeField] private TextMeshProUGUI DialogueTitleText, DialogueBodyText; // Text components for title and body
    [SerializeField] private GameObject responseButtonPrefab; // Prefab for generating response buttons
    [SerializeField] private Transform responseButtonContainer; // Container to hold response buttons
 	[SerializeField] private Image characterImage;
    [SerializeField] private GameObject RewardScreen;

    private bool isTyping;
    private Dialogue currDialogue;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of DialogueManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
 
        // Initially hide the dialogue UI
        HideDialogue();
    }

    private void Start()
    {
        audioManager = 	FindFirstObjectByType<AudioManager>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    // Starts the dialogue with given title and dialogue node
    public void StartDialogue(Dialogue dialogue)
    {
		Debug.Log("Starting Dialogue: " + dialogue);
        // Display the dialogue UI
        ShowDialogue();
 
        // Set character name/iamge
        Debug.Log("Setting Character: " + dialogue.character.characterName);
        DialogueTitleText.text = dialogue.character.characterName;
        characterImage.sprite = dialogue.character.characterImage;
        
        currDialogue = dialogue;
        StartLine(dialogue.RootNode);
    }

    private void StartLine(DialogueNode currNode) {
        StartCoroutine(TypeLine(currNode.dialogueText));
 
        // Remove any existing response buttons
        foreach (Transform child in responseButtonContainer)
        {
            Destroy(child.gameObject);
        }
 
        // Create and setup response buttons based on current dialogue node
        foreach (DialogueResponse response in currNode.responses)
        {
            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = response.responseText;
 
            // Setup button to trigger SelectResponse when clicked
            buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response));
        }
    }
 
    // Handles response selection and triggers next dialogue node
    public void SelectResponse(DialogueResponse response)
    {
        if (isTyping) {
            StopAllCoroutines();
            isTyping = false;
        }
		// Button click audio
        audioManager.PlaySFX(audioManager.buttonClick);

        if (response.containsReward) AddRewards(response);

        // Fetch nextNode
        DialogueNode nextLine = currDialogue.nodes[response.nextNodeIndex];

        // Check if there's a follow-up node
        if (!nextLine.IsLastNode())
        {
            StartLine(nextLine); // Start next dialogue line
        }
        else
        {
            // If no follow-up node, end the dialogue
            HideDialogue();
            // this is a terrible way to do this but I need to temporarily to trigger tutorial after dialogue
            // TODO: add a better way to trigger this
            if (SceneManager.GetActiveScene().buildIndex == 3 && tutorialManager != null)
            {
                tutorialManager.StartTutorial(tutorial);
            }
        }
    }

    // Hide the dialogue UI
    public void HideDialogue()
    {
        DialogueParent.SetActive(false);
    }
 
    // Show the dialogue UI
    private void ShowDialogue()
    {
        DialogueParent.SetActive(true);
    }
 
    // Check if dialogue is currently active
    public bool IsDialogueActive()
    {
        return DialogueParent.activeSelf;
    }

    IEnumerator TypeLine(string text) {
        isTyping = true; 
        DialogueBodyText.text = "";
        foreach (char letter in text) {
            DialogueBodyText.text += letter;
            yield return new WaitForSeconds(currDialogue.character.typingSpeed);
        }
        isTyping = false; 
    }
    private void AddRewards(DialogueResponse currNode)
    {
        // Add any items/effects connected to node
        foreach (Item item in currNode.itemsRewards)
        {
            if (item is ArtifactItem)
                gameManager.AddArtifact(item.ID);
            else
                gameManager.AddToInventory(item.ID);
            // Add pop-up
            // For now just add individual screens per item, could combine multipl into one later if need be
            GameObject screen = Instantiate(RewardScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
            screen.GetComponent<PopUpRewardController>().AddRewardInfo(item.Icon, item.itemName, item.flavorText);
        }
        if (currNode.officeBucks != 0)
        {
            GameObject screen = Instantiate(RewardScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
            screen.GetComponent<PopUpRewardController>().AddRewardInfo(null, currNode.officeBucks + " officeBucks", "Use to purchase items from the vending machine!");
            gameManager.UpdateOfficeBucks(currNode.officeBucks);
            GameObject.FindGameObjectWithTag("Counter_OfficeBucks")
                .GetComponent<CurrencyCounter>()
                .RefreshCounter();
        }
        if (currNode.soulCredits != 0)
        {
            GameObject screen = Instantiate(RewardScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
            screen.GetComponent<PopUpRewardController>().AddRewardInfo(null, currNode.soulCredits + " soulCredits", "Use to pay rent & purchase apartment upgrades!");
            gameManager.UpdateSoulCredits(currNode.soulCredits);
            GameObject.FindGameObjectWithTag("Counter_SoulCredits")
                .GetComponent<CurrencyCounter>()
                .RefreshCounter();
        }
        if (currNode.performanceBoost != 0)
        {
            GameObject screen = Instantiate(RewardScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
            screen.GetComponent<PopUpRewardController>().AddRewardInfo(null, currNode.performanceBoost + " performance", "");
            float newPerformance = gameManager.FetchPerformance() + currNode.performanceBoost;
            gameManager.UpdatePerformance(newPerformance);
            GameObject.FindGameObjectWithTag("PerformanceMeter")
                .GetComponent<SliderCounter>()
                .UpdateBar(newPerformance);
        }
        if (currNode.willBoost != 0)
        {
            GameObject screen = Instantiate(RewardScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
            screen.GetComponent<PopUpRewardController>().AddRewardInfo(null, currNode.willBoost + " will", "");
            float newWill = gameManager.FetchWill() + currNode.willBoost;
            gameManager.UpdateWill(newWill);
            GameObject.FindGameObjectWithTag("WillMeter")
                .GetComponent<SliderCounter>()
                .UpdateBar(newWill);
        }
        if (currNode.chaos != 0)
        {
            GameObject screen = Instantiate(RewardScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
            screen.GetComponent<PopUpRewardController>().AddRewardInfo(null, currNode.chaos + " chaos", "");
        }
        if (currNode.specialRewardMessage != null && currNode.specialRewardMessage != "") {
            GameObject screen = Instantiate(RewardScreen, GameObject.FindGameObjectWithTag("Canvas").transform.position, GameObject.FindGameObjectWithTag("Canvas").transform.rotation, GameObject.FindGameObjectWithTag("Canvas").transform);
            screen.GetComponent<PopUpRewardController>().AddRewardInfo(null, currNode.specialRewardMessage, "");            
        }
        // TODO: add effects and special cases
    }
}