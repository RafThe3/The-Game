using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private bool enableDialogue = true;
    [SerializeField] private bool enableTrigger = false;
    [SerializeField] private bool endlessDialogue = false;
    [SerializeField] private bool giveObjective = false;
    [TextArea(3, 5), SerializeField] private string[] dialogue;
    [TextArea(3, 5), SerializeField] private string objective;

    private GameObject barrier;
    private Dialogue dialogueManager;
    public bool HasInteracted = false;
    public bool IsTalking = false;

    private void Awake()
    {
        dialogueManager = FindFirstObjectByType<Dialogue>();
        barrier = GameObject.FindWithTag("Barrier");
    }

    private void Update()
    {
        tag = enableDialogue ? "NPC" : "Untagged";

        if (!enableDialogue)
        {
            return;
        }

        if (!CompareTag("Player"))
        {
            HasInteracted = dialogueManager.HasInteracted;
            IsTalking = dialogueManager.IsTalking;
            barrier.SetActive(HasInteracted);
        }
    }

    public void StartDialogue()
    {
        barrier.transform.position = transform.position;
        dialogueManager.SetDialogue(dialogue);
        dialogueManager.SetName(name);
        dialogueManager.StartDialogue();
        dialogueManager.TriggerObjective(giveObjective);
        if (giveObjective) dialogueManager.SetObjective(objective);
    }

    public void DisplayNextDialogue()
    {
        dialogueManager.DisplayNextDialogue();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enableTrigger) return;
    }
}
