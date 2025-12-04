using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance;
    private Image fadeImage;
    private Coroutine currentFade;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        fadeImage = GetComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    public IEnumerator FadeOut(float duration)
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(Fade(0, 1, duration));
        yield return currentFade;
    }

    public IEnumerator FadeIn(float duration)
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(Fade(1, 0, duration));
        yield return currentFade;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float t = 0;
        Color c = fadeImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, t / duration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = endAlpha;
        fadeImage.color = c;
    }
}
