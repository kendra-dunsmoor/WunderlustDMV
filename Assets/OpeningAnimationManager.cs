using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class OpeningAnimationManager : MonoBehaviour
{
    [SerializeField] GameObject textBox;
    [SerializeField] TextMeshProUGUI openingText;
    [SerializeField] GameObject continueButton;
    [SerializeField] string[] dialogueLines;
    private bool isTyping;


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
        openingText.text = "";
        foreach (char letter in text)
        {
            openingText.text += letter;
            yield return new WaitForSeconds(0.01f);
        }
        isTyping = false;
        yield return new WaitForSeconds(0.5f);
        continueButton.SetActive(true);
    }

    public void GoToApartment()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
