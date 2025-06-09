using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class OpeningAnimationManager : MonoBehaviour
{
    AudioManager audioManager;
    [SerializeField] GameObject textBox;
    [SerializeField] TextMeshProUGUI openingText;
    [SerializeField] GameObject continueButton;
    [SerializeField] string[] dialogueLines;
    [SerializeField] AudioClip textSound;

    private bool isTyping;

    void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textBox.SetActive(false);
        continueButton.SetActive(false);

        // Trigger opening text for now
        StartCoroutine(Delay(1));
        // Can expand on this scene later
    }

    IEnumerator Delay(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        textBox.SetActive(true);
        StartCoroutine(TypeLine(dialogueLines[0]));
    }

    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        audioManager.PlayDialogue(textSound);
        openingText.text = "";
        foreach (char letter in text)
        {
            openingText.text += letter;
            yield return new WaitForSeconds(0.01f);
        }
        isTyping = false;
        audioManager.StopDialogue();
        yield return new WaitForSeconds(0.75f);
        continueButton.SetActive(true);
    }

    public void GoToApartment()
    {
        audioManager.PlayDialogue(textSound);
        SceneManager.LoadSceneAsync(1);
    }
}
