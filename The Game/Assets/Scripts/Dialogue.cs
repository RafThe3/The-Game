using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.TextCore.Text;
using UnityEditor.ShaderGraph.Internal;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Dialogue : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField] private bool enableDialogue = true;
    [SerializeField] private bool giveObjective = false;
    [TextArea(3, 5), SerializeField] private string[] dialogue = null;
    [TextArea(3, 5), SerializeField] private string objective = string.Empty;
    [SerializeField] private TextMeshProUGUI interactTextBox, objectiveTextBox;
    public TextMeshProUGUI dialogueTextBox, nameTextBox;
    [SerializeField] private float interactionDistance = 1;
    [SerializeField] private float dialogueSpeed = 1;
    [SerializeField] private AudioClip dialogueSFX;

    [Header("Interactables")]
    [SerializeField] private bool triggerInteractable = false;
    [SerializeField] private Interactable[] interactables;
    [SerializeField] private bool triggerDialogue = false;
    [SerializeField] private Dialogue[] npcs;
    [SerializeField] private bool disableDialogue = false;

    private Queue<string> dialogueQueue;
    private Transform player;
    private bool isTalking = false;
    private bool hasInteracted = false;
    private readonly string dialogueControls = "Press [g] to continue";

    private void Start()
    {
        dialogueQueue = new();
        player = GameObject.FindWithTag("Player").transform;
        dialogueTextBox.text = string.Empty;
        nameTextBox.text = string.Empty;
    }

    private void Update()
    {
        tag = enableDialogue ? "NPC" : "Untagged";

        if (!enableDialogue)
        {
            return;
        }

        Vector3 playerPosition = player.position - transform.position;
        bool isPlayerNear = playerPosition.magnitude < interactionDistance;

        dialogueTextBox.enabled = isPlayerNear;
        nameTextBox.enabled = isPlayerNear;

        Debug.Log(isPlayerNear);

        /*
        if (isPlayerNear && dialogueTextBox.text != string.Empty)
        {
            interactTextBox.text = dialogueControls;
        }
        */
    }

    private IEnumerator Write(string sentence, TextMeshProUGUI textBox)
    {
        isTalking = true;

        textBox.text = string.Empty;
        if (textBox == dialogueTextBox)
        {
            nameTextBox.text = name;
        }

        foreach (char c in sentence.ToCharArray())
        {
            textBox.text += c;
            if (c.ToString() != " ")
            {
                Camera.main.GetComponent<AudioSource>().PlayOneShot(dialogueSFX);
            }
            yield return new WaitForSeconds(0.1f / dialogueSpeed);
        }

        isTalking = false;
    }
    
    public void StartDialogue()
    {
        Debug.Log("Start of conversation");

        hasInteracted = true;

        dialogueQueue.Clear();

        foreach (string sentence in dialogue)
        {
            dialogueQueue.Enqueue(sentence);
        }

        DisplayNextDialogue();
    }

    public void DisplayNextDialogue()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            if (hasInteracted && giveObjective && objective != string.Empty)
            {
                StartCoroutine(Write($"Objective: {objective}", objectiveTextBox));
                objective = string.Empty;
            }
            if (triggerDialogue)
            {
                TriggerDialogue();
            }
            if (triggerInteractable)
            {
                TriggerInteractable();
            }
            hasInteracted = false;
            return;
        }

        string nextDialogue = dialogueQueue.Dequeue();
        StopAllCoroutines();
        StartCoroutine(Write(nextDialogue, dialogueTextBox));
    }

    private void EndDialogue()
    {
        dialogueTextBox.text = string.Empty;
        nameTextBox.text = string.Empty;
        interactTextBox.text = string.Empty;
        if (disableDialogue)
        {
            enableDialogue = false;
        }
        Debug.Log("End of conversation");
    }

    public void ChangeDialogue(string[] dialogue)
    {
        this.dialogue = dialogue;
    }

    private void TriggerDialogue()
    {
        foreach (Dialogue n in npcs)
        {
            n.EnableDialogue();
        }
    }

    private void TriggerInteractable()
    {
        foreach (Interactable i in interactables)
        {
            i.EnableInteract();
        }
    }

    public void EnableDialogue() => enableDialogue = true;

    public bool DialogueEnabled => enableDialogue;

    public bool IsTalking => isTalking;

    public bool HasInteracted => hasInteracted;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
