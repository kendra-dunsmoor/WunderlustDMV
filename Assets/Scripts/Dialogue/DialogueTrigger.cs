using UnityEngine;

/*
* Dialogue Trigger
* ~~~~~~~~~~~~~
* Attach to GameObject that triggers dialogue on interaction
*/
public class DialogueTrigger : MonoBehaviour
{
	private EventSelector eventSelector;

    void Start()
    {
        eventSelector = 	FindFirstObjectByType<EventSelector>();
        Dialogue dialogue = eventSelector.FetchRandomEvent();
		FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue);
    }
}
