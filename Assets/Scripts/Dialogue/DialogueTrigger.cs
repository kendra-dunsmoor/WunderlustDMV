using UnityEngine;

/*
* Dialogue Trigger
* ~~~~~~~~~~~~~
* Attach to GameObject that triggers dialogue on interaction
*/
public class DialogueTrigger : MonoBehaviour
{
	[SerializeField] private Dialogue dialogue;
	public void TriggerDialogue ()
	{
		FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue);
	}
}
