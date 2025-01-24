using System.Collections;
using UnityEngine;
using TMPro;

public class TypingEffect : MonoBehaviour
{
    public TMP_Text textMeshPro;
    public float typingSpeed = 0.1f;
    public bool loopText = false;
    public float loopDelay = 2f;

    private string fullText;

    private void Start()
    {
        fullText = textMeshPro.text;
        textMeshPro.text = string.Empty;
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        while (true)
        {
            textMeshPro.text = string.Empty;
            foreach (char letter in fullText)
            {
                textMeshPro.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            if (!loopText)
                break;

            yield return new WaitForSeconds(loopDelay);
        }
    }
}
