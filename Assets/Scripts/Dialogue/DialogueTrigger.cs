using UnityEngine;

/*
* Dialogue Trigger
* ~~~~~~~~~~~~~
* Attach to GameObject that triggers dialogue on interaction
*/
public class DialogueTrigger : MonoBehaviour
{
	[SerializeField] private string character;
	[SerializeField] private Dialogue dialogue;

	// TODO: can add option to trigger by clicking too in update function
	public void TriggerDialogue ()
	{
		Debug.Log("Trying to trigger dialogue: " + character);
		FindFirstObjectByType<DialogueManager>().StartDialogue(character, dialogue.RootNode);
	}
}
