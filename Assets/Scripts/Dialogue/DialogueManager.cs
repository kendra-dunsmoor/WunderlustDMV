using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
 
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
 
    // UI references
    [SerializeField] private GameObject DialogueParent; // Main container for dialogue UI
    [SerializeField] private TextMeshProUGUI DialogueTitleText, DialogueBodyText; // Text components for title and body
    [SerializeField] private GameObject responseButtonPrefab; // Prefab for generating response buttons
    [SerializeField] private Transform responseButtonContainer; // Container to hold response buttons
 	[SerializeField] private Image characterImage;

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
 
    // Starts the dialogue with given title and dialogue node
    public void StartDialogue(string title, DialogueNode node)
    {
		Debug.Log("Title: " + title);
        // Display the dialogue UI
        ShowDialogue();
 
        // Set dialogue title and body text
        DialogueTitleText.text = title;
        DialogueBodyText.text = node.dialogueText;
 
        // Remove any existing response buttons
        foreach (Transform child in responseButtonContainer)
        {
            Destroy(child.gameObject);
        }
 
        // Create and setup response buttons based on current dialogue node
        foreach (DialogueResponse response in node.responses)
        {
            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = response.responseText;
 
            // Setup button to trigger SelectResponse when clicked
            buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response, title));
        }
    }
 
    // Handles response selection and triggers next dialogue node
    public void SelectResponse(DialogueResponse response, string title)
    {
        // Check if there's a follow-up node
        if (!response.nextNode.IsLastNode())
        {
            StartDialogue(title, response.nextNode); // Start next dialogue
        }
        else
        {
            // If no follow-up node, end the dialogue
            HideDialogue();
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
}