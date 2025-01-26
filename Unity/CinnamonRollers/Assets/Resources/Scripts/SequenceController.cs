using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit; // Assuming this namespace contains FindSpawnPositions

public class SequenceController : MonoBehaviour
{
    [System.Serializable]
    public class SequenceStep
    {
        public AudioClip audioClip; // Audio file to play
        public GameObject prefabToSpawn; // Prefab to spawn via FindSpawnPositions
        public string requiredSemanticLabel = "LAMP"; // Semantic label to check
        public AnimationClip animationClip; // Animation to trigger
        public float waitTimeAfterCompletion = 1.0f; // Delay before the next step
    }

    [SerializeField] private List<SequenceStep> sequenceSteps = new List<SequenceStep>();
    [SerializeField] private FindSpawnPositions findSpawnPositions; // Reference to FindSpawnPositions
    [SerializeField] private Entry entryScript; // Reference to the Entry script
  
    private int currentStepIndex = 0;
    private bool isSequenceActive = false;

    private void Start()
    {
        if (entryScript != null)
        {
            entryScript.OnEntryComplete.AddListener(StartSequence);
        }
    }

    public void StartSequence()
    {
        if (!isSequenceActive && sequenceSteps.Count > 0)
        {
            isSequenceActive = true;
            StartCoroutine(PlaySequence());
        }
    }

    private IEnumerator PlaySequence()
    {
        while (currentStepIndex < sequenceSteps.Count)
        {
            var step = sequenceSteps[currentStepIndex];

            // Step 1: Play audio
            if (step.audioClip != null)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = step.audioClip;
                audioSource.Play();
                yield return new WaitForSeconds(step.audioClip.length);
                Destroy(audioSource);
            }

            // Step 2: Wait for hand collision with semantic label
            bool isTriggered = false;
            while (!isTriggered)
            {
                yield return new WaitForFixedUpdate();

                Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f); // Adjust radius as needed
                foreach (var collider in colliders)
                {
                    MRUKAnchor anchor = collider.GetComponent<MRUKAnchor>();
                    if (anchor != null && anchor.HasAnyLabel(GetSceneLabel(step.requiredSemanticLabel)))
                    {
                        // Trigger animation
                        if (step.animationClip != null)
                        {
                            Animator animator = collider.GetComponent<Animator>();
                            if (animator != null)
                            {
                                animator.Play(step.animationClip.name);
                                yield return new WaitForSeconds(step.animationClip.length);
                            }
                        }
                        isTriggered = true;
                        break;
                    }
                }
            }

            // Step 3: Use FindSpawnPositions to spawn the prefab
            if (findSpawnPositions != null && step.prefabToSpawn != null)
            {
                findSpawnPositions.SpawnObject = step.prefabToSpawn; // Set the prefab to spawn
                findSpawnPositions.StartSpawn(); // Trigger spawning
            }

            // Step 4: Wait before the next step
            yield return new WaitForSeconds(step.waitTimeAfterCompletion);

            // Proceed to the next step
            currentStepIndex++;
        }

        isSequenceActive = false;
    }

    private MRUKAnchor.SceneLabels GetSceneLabel(string label)
    {
        return label switch
        {
            "FLOOR" => MRUKAnchor.SceneLabels.FLOOR,
            "CEILING" => MRUKAnchor.SceneLabels.CEILING,
            "LAMP" => MRUKAnchor.SceneLabels.LAMP,
            _ => MRUKAnchor.SceneLabels.OTHER
        };
    }
}
