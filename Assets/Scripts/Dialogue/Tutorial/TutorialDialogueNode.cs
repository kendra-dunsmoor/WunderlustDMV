using UnityEngine;

[System.Serializable]
public class TutorialDialogueNode
{
    [TextArea(3, 10)]
    public string dialogueText;
    public enum TutorialBox
    {
        CENTER,
        BASIC_BUTTONS,
        SPECIAL_ACTIONS,
        PERFORMANCE,
        WILL,
        CUSTOMER,
        PAPERWORK
    }
    public TutorialBox targetBox;
}
