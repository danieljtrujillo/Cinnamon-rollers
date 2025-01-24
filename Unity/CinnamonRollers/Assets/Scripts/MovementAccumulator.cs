using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

namespace Sngty
{
    public class MovementAccumulator : MonoBehaviour
    {
        [Tooltip("Time in seconds to accumulate data before stopping.")]
        public float listenDuration = 30f;

        [Tooltip("If average roll is less than this, do Action A, otherwise Action B.")]
        public float rollThreshold = 5f;

        [Header("Prefab References")]
        [Tooltip("Prefab to spawn if avgRoll < rollThreshold")]
        [SerializeField] private GameObject prefabActionA;

        [Tooltip("Prefab to spawn if avgRoll >= rollThreshold")]
        [SerializeField] private GameObject prefabActionB;

        // Internal accumulators
        private float totalRoll;
        private float totalPitch;
        private int messageCount;
        private bool isListening;

        [Header("Reference to SingularityManager")]
        [SerializeField]
        private SingularityManager singularityMgr;

        private void Start()
        {
            // Subscribe to the onMessageRecieved event so we get roll/pitch data
            singularityMgr.onMessageRecieved.AddListener(OnMessageReceived);

            // Start a coroutine that listens for 'listenDuration' seconds
            StartCoroutine(ListenAndStopAfterTime(listenDuration));
        }

        /// <summary>
        /// Runs for 'duration' seconds, then finalizes logic.
        /// </summary>
        private IEnumerator ListenAndStopAfterTime(float duration)
        {
            // Reset accumulated values
            totalRoll = 0f;
            totalPitch = 0f;
            messageCount = 0;
            isListening = true;

            // Wait the specified duration
            yield return new WaitForSeconds(duration);

            // Stop listening for new messages
            isListening = false;

            // Check if we received any data at all
            if (messageCount == 0)
            {
                Debug.Log("No movement data received in the allotted time.");
                yield break;
            }

            // Calculate averages
            float avgRoll = totalRoll / messageCount;
            float avgPitch = totalPitch / messageCount;

            Debug.Log($"After {duration} seconds, AvgRoll = {avgRoll:F2}, AvgPitch = {avgPitch:F2}, from {messageCount} messages.");

            // Decide which prefab to instantiate based on avgRoll vs rollThreshold
            if (avgRoll < rollThreshold)
            {
                Debug.Log("Action A triggered! (Roll below threshold)");
                Instantiate(prefabActionA, Vector3.zero, Quaternion.identity);
            }
            else
            {
                Debug.Log("Action B triggered! (Roll >= threshold)");
                Instantiate(prefabActionB, Vector3.zero, Quaternion.identity);
            }
        }

        /// <summary>
        /// Called every time SingularityManager receives a new WiFi/Bluetooth message.
        /// We parse 'roll' and 'pitch' from it, e.g. "roll = 0.0, pitch = -14.6".
        /// </summary>
        private void OnMessageReceived(string message)
        {
            // Ignore data if the listening window has closed
            if (!isListening)
                return;

            // Regex to extract numeric values after "roll =" and "pitch ="
            var match = Regex.Match(message, @"roll\s*=\s*([-\d\.]+),\s*pitch\s*=\s*([-\d\.]+)");
            if (match.Success)
            {
                if (float.TryParse(match.Groups[1].Value, out float rollValue))
                    totalRoll += rollValue;

                if (float.TryParse(match.Groups[2].Value, out float pitchValue))
                    totalPitch += pitchValue;

                messageCount++;
            }
            else
            {
                Debug.LogWarning($"Could not parse roll/pitch from message: {message}");
            }
        }
    }
}
