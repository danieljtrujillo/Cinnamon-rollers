using System.Collections;
using UnityEngine;
using TMPro;

public class TypingEffect : MonoBehaviour
{
    public TMP_Text textMeshPro;
    public float typingSpeed = 0.1f;
    public bool loopText = false;
    public float loopDelay = 2f;
    public float fadeDuration = 1f; // New parameter for fade duration

    private string fullText;

    private void Start()
    {
        fullText = textMeshPro.text;
        textMeshPro.text = string.Empty;
        // Set initial alpha to 1
        textMeshPro.alpha = 1f;
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        while (true)
        {
            // Reset text and fade in
            textMeshPro.text = string.Empty;
            yield return StartCoroutine(FadeText(0f, 1f));

            // Type the text
            foreach (char letter in fullText)
            {
                textMeshPro.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            // Wait a moment before fading out
            yield return new WaitForSeconds(0.5f);

            // Fade out
            yield return StartCoroutine(FadeText(1f, 0f));

            if (!loopText)
                break;

            yield return new WaitForSeconds(loopDelay);
        }
    }

    IEnumerator FadeText(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        textMeshPro.alpha = startAlpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            textMeshPro.alpha = currentAlpha;
            yield return null;
        }

        textMeshPro.alpha = endAlpha; // Ensure we end up exactly at the target alpha
    }
}