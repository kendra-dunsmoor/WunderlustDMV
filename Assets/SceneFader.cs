using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private float fadeDuration = 1f;
    private GameManager gameManager;
    private string[] weekdays = {
        "Dreadday",
        "ReapDay",
        "DragDay",
        "LoopDay",
        "ReckonDay"
    };

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Start()
    {
        FadeIn();
    }

    private void FadeIn()
    {
        StartCoroutine(Fade(1, 0));
    }

    private void FadeOut()
    {
        StartCoroutine(Fade(0, 1));
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
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

    public void LoadScene(int buildNum)
    {
        FadeOut();
        string text = "";
        if (buildNum == 3)
        {
            // If transitioning to combat display what day it is
            int day = gameManager.FetchCurrentCalendarDay();
            text = "Day " + (day + 1) + ": " + weekdays[day];
        }
        StartCoroutine(LoadSceneAsync(buildNum, text));
    }

    IEnumerator LoadSceneAsync(int buildNum, string text)
    {
        yield return new WaitForSeconds(fadeDuration);
        // If text show text and wait
        if (text != "")
        {
            StartCoroutine(TypeLine(text));
            yield return new WaitForSeconds(fadeDuration * 2);
        }
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(buildNum);
    }

    IEnumerator TypeLine(string text) {
        // audioManager.PlayDialogue(PickFromSounds());
        textBox.text = "";
        foreach (char letter in text) {
            textBox.text += letter;
            yield return new WaitForSeconds(0.2f);
        }
        // Cut off long looping sound:
        // audioManager.StopDialogue();
    }
}