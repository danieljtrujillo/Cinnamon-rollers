using UnityEngine;
using TMPro;

public class MessageDisplay : MonoBehaviour
{
    // Reference to the text component in your Canvas
    [SerializeField] 
    private TextMeshProUGUI messageText;

    public void OnMessageReceived(string msg)
    {
        // Debug check
        Debug.Log($"OnMessageReceived called with: [{msg}]");
        
        // If msg is empty or whitespace, handle that
        if (string.IsNullOrWhiteSpace(msg))
        {
            Debug.LogWarning("Received empty or whitespace message—skipping display.");
            return;
        }

        // **Overwrite** the text rather than appending
        messageText.text = msg;
    }
}
