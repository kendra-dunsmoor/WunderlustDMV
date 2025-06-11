using UnityEngine;

/*
* Dialogue Trigger
* ~~~~~~~~~~~~~
* Attach to GameObject that triggers dialogue on interaction
*/
public class DialogueTrigger : MonoBehaviour
{
    private EventSelector eventSelector;
    [SerializeField] private DialogueManager dialogueManager;


    void Awake()
    {
        eventSelector = FindFirstObjectByType<EventSelector>();
    }

    public void TriggerDialogue()
    {
        Dialogue dialogue = eventSelector.FetchRandomEvent();
        dialogueManager.StartDialogue(dialogue);
    }
}
