using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class Entry : MonoBehaviour
{
    [System.Serializable]
    public class FaderSettings
    {
        public Image image;
        public float fadeInStartTime = 0f;
        public float fadeInDuration = 1f;
        public float fadeOutStartTime = 5f;
        public float fadeOutDuration = 1f;
        public AudioClip audioClip;
    }

    [System.Serializable]
    public class ObjectActivationSettings
    {
        public GameObject objectToActivate;
        public float waitForTime = 0f;
        public bool shouldDeactivate = true;
        public float setInactiveAfterTime = 5f;
        public UnityEvent onObjectActivated; // This will show up as a dropdown in the inspector
    }

    [SerializeField] private List<FaderSettings> faders = new List<FaderSettings>();
    [SerializeField] private List<ObjectActivationSettings> objectsToActivate = new List<ObjectActivationSettings>();

    public UnityEvent OnEntryComplete; // Event triggered when the Entry sequence finishes

    private void Start()
    {
        StartCoroutine(HandleFading());
        StartCoroutine(HandleObjectActivation());
    }
    private IEnumerator HandleFading()
    {
        foreach (var fader in faders)
        {
            StartCoroutine(FadeImage(fader));
        }
        yield return null;

        CheckCompletion();
    }
    private IEnumerator FadeImage(FaderSettings fader)
    {
        Image image = fader.image;
        AudioSource audioSource = image.gameObject.AddComponent<AudioSource>();
        audioSource.clip = fader.audioClip;
        // Set initial opacity
        Color color = image.color;
        color.a = 0f;
        image.color = color;
        // Wait for fade in start time
        yield return new WaitForSeconds(fader.fadeInStartTime);
        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fader.fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fader.fadeInDuration);
            image.color = color;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            yield return null;
        }
        // Wait for fade out start time
        yield return new WaitForSeconds(fader.fadeOutStartTime - fader.fadeInStartTime - fader.fadeInDuration);
        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fader.fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsedTime / fader.fadeOutDuration);
            image.color = color;
            yield return null;
        }
        // Ensure the image is fully transparent and audio is stopped
        color.a = 0f;
        image.color = color;
        audioSource.Stop();
        image.gameObject.SetActive(false);
    }
    private IEnumerator HandleObjectActivation()
    {
        foreach (var objSetting in objectsToActivate)
        {
            StartCoroutine(ActivateAndDeactivateObject(objSetting));
        }
        yield return null;

        
        CheckCompletion();
    }
    private IEnumerator ActivateAndDeactivateObject(ObjectActivationSettings objSetting)
    {
        // Wait for the specified time before activating
        yield return new WaitForSeconds(objSetting.waitForTime);

        // Activate the object
        objSetting.objectToActivate.SetActive(true);

        // Invoke any methods assigned in the inspector
        objSetting.onObjectActivated?.Invoke();

        // Only deactivate if shouldDeactivate is true
        if (objSetting.shouldDeactivate)
        {
            // Wait for the specified time before deactivating
            yield return new WaitForSeconds(objSetting.setInactiveAfterTime);
            // Deactivate the object
            objSetting.objectToActivate.SetActive(false);
        }

        CheckCompletion();

    }

        private void CheckCompletion()
    {
        // Check if both coroutines have finished
        if (!IsInvoking("HandleFading") && !IsInvoking("HandleObjectActivation") && !IsInvoking("ActivateAndDeactivateObject"))
        {
            OnEntryComplete?.Invoke();
        }
    }
}