using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class OpeningAnimationManager : MonoBehaviour
{
    AudioManager audioManager;
    [SerializeField] SceneFader sceneFader;
    [SerializeField] GameObject textBox;
    [SerializeField] TextMeshProUGUI openingText;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject tombstone;
    [SerializeField] string[] dialogueLines;
    [SerializeField] AudioClip textSound;
    [SerializeField] AudioClip bellSound;
    private float fadeDuration = 1.5f;

    void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    void Start()
    {
        sceneFader.gameObject.SetActive(true);
        textBox.SetActive(false);
        continueButton.SetActive(false);

        // Trigger opening text for now
        StartCoroutine(DelayedAnimation());
        // Can expand on this scene later
    }

    private void FadeIn(Image image)
    {
        StartCoroutine(Fade(1, 0, image));
    }

    private void FadeOut(Image image)
    {
        StartCoroutine(Fade(0, 1, image));
    }

    IEnumerator Fade(float startAlpha, float endAlpha, Image fadeImage)
    {
        float time = 0;
        Color color = fadeImage.color;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }
    }

    IEnumerator DelayedAnimation()
    {
        audioManager.PlayClipTwice(bellSound);
        yield return new WaitForSeconds(1.5f);
        Image image = tombstone.GetComponent<Image>();
        yield return StartCoroutine(Fade(1, 0, image));
        yield return new WaitForSeconds(1f);
        textBox.SetActive(true);
        StartCoroutine(TypeLine(dialogueLines[0]));
    }

    IEnumerator TypeLine(string text)
    {
        audioManager.PlayDialogue(textSound);
        openingText.text = "";
        foreach (char letter in text)
        {
            openingText.text += letter;
            yield return new WaitForSeconds(0.01f);
        }
        audioManager.StopDialogue();
        yield return new WaitForSeconds(0.75f);
        continueButton.SetActive(true);
    }

    public void GoToApartment()
    {
        audioManager.PlayDialogue(audioManager.buttonClick);
        sceneFader.LoadScene(1);
    }
}
