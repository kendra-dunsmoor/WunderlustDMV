using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
* Dialogue Manager
* ~~~~~~~~~~~~~~~~~
* Manages display of text in dialogue box
*/
public class DialogueManager : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI nameText;
	[SerializeField] private TextMeshProUGUI dialogueText;
	[SerializeField] private GameObject continueButton;
	[SerializeField] private TextMeshProUGUI continueText; // TODO: add updating option text for player response
	[SerializeField] private Image characterImage;

	private AudioManager audioManager;

	private Queue sentences;

	// Use this for initialization
	void Start () {
		sentences = new Queue();
		audioManager = 	FindFirstObjectByType<AudioManager>();
	}

	public void StartDialogue (Dialogue dialogue)
	{
		nameText.text = dialogue.name;

		sentences.Clear();

		foreach (string sentence in dialogue.sentences)
		{
			sentences.Enqueue(sentence);
		}

		DisplayNextSentence();
	}

	public void DisplayNextSentence ()
	{
		audioManager.PlaySFX(audioManager.buttonClick);
		if (sentences.Count == 1)
		{
			EndDialogue(); // TODO: add animation to remove box
		}

		string sentence = sentences.Dequeue().ToString();
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
	}

	IEnumerator TypeSentence (string sentence)
	{
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return null;
		}
	}

	void EndDialogue()
	{
		continueButton.SetActive(false);
	}
}
