using UnityEngine;
using System.Collections;

public class MaterialFade : MonoBehaviour
{
    private Material material;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float delayBeforeFade = 0f;
    [SerializeField] private bool fadeInOnStart = true;
    private bool hasFaded = false;

    void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    void Start()
    {
        // Set initial alpha based on fade direction
        Color startColor = material.color;
        material.color = new Color(startColor.r, startColor.g, startColor.b, fadeInOnStart ? 0f : 1f);
        
        // Start the delayed fade coroutine
        StartCoroutine(DelayedFade());
    }

    private IEnumerator DelayedFade()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayBeforeFade);
        
        if (fadeInOnStart)
        {
            FadeIn();
        }
        else
        {
            FadeOut();
        }
        
        hasFaded = true;
    }

    public void FadeIn()
    {
        StartCoroutine(FadeRoutine(0f, 1f));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeRoutine(1f, 0f));
    }

    // Method to manually trigger fade with optional new delay
    public void TriggerFade(bool fadeIn, float delay = 0f)
    {
        if (!hasFaded || delay > 0f)
        {
            StopAllCoroutines();
            delayBeforeFade = delay;
            fadeInOnStart = fadeIn;
            StartCoroutine(DelayedFade());
        }
    }

    private IEnumerator FadeRoutine(float startValue, float endValue)
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, endValue, elapsedTime / fadeDuration);
            
            // Get the current color each frame
            Color currentColor = material.color;
            material.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            
            yield return null;
        }

        // Ensure we end up exactly at the target alpha
        Color finalColor = material.color;
        material.color = new Color(finalColor.r, finalColor.g, finalColor.b, endValue);
    }
}