using TMPro;
using UnityEngine;
using static TutorialDialogueNode;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject tutorialParent;
    TutorialDialogue tutorial;
    private GameObject activeBox;
    private int currLine;

    [Header("------------- Tutorial Box Locations -------------")]
    [SerializeField] GameObject center;
    [SerializeField] GameObject basicButtons;
    [SerializeField] GameObject specialActions;
    [SerializeField] GameObject performance;
    [SerializeField] GameObject paperwork;
    [SerializeField] GameObject will;
    [SerializeField] GameObject customer;

    private AudioManager audioManager;

    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }
    public void StartTutorial(TutorialDialogue tutorial)
    {
        currLine = 0;
        activeBox = center;
        this.tutorial = tutorial;
        tutorialParent.SetActive(true);
        NextLine();
    }

    public void NextLine()
    {
        Debug.Log("Next tutorial line");
        audioManager.PlaySFX(audioManager.buttonClick);
        activeBox.SetActive(false);
        if (currLine >= tutorial.lines.Length)
        {
            EndTutorial();
            return;
        }
        TutorialDialogueNode tutorialLine = tutorial.lines[currLine];
        switch (tutorialLine.targetBox)
        {
            case TutorialBox.CENTER:
                activeBox = center;
                break;
            case TutorialBox.SPECIAL_ACTIONS:
                activeBox = specialActions;
                break;
            case TutorialBox.BASIC_BUTTONS:
                activeBox = basicButtons;
                break;
            case TutorialBox.PERFORMANCE:
                activeBox = performance;
                break;
            case TutorialBox.PAPERWORK:
                activeBox = paperwork;
                break;
            case TutorialBox.WILL:
                activeBox = will;
                break;
            case TutorialBox.CUSTOMER:
                activeBox = customer;
                break;
        }
        activeBox.GetComponentInChildren<TextMeshProUGUI>().text = tutorialLine.dialogueText;
        activeBox.SetActive(true);
        currLine++;
    }


    private void EndTutorial()
    {
        Debug.Log("Ending tutorial");
        activeBox.SetActive(false);
        tutorialParent.SetActive(false);
    }
}
