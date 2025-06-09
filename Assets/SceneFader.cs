using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

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
        StartCoroutine(LoadSceneAsync(buildNum));
    }

    IEnumerator LoadSceneAsync(int buildNum)
    {
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(buildNum);
    }
}