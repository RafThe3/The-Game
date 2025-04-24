using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.TextCore.Text;
using UnityEditor.ShaderGraph.Internal;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.AI;
using System.Net;
using TreeEditor;
public class Dialogue : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactTextBox;
    [SerializeField] private TextMeshProUGUI objectiveTextBox;
    [SerializeField] private TextMeshProUGUI dialogueTextBox;
    [SerializeField] private TextMeshProUGUI nameTextBox;

    [Header("Dialogue")]
    [SerializeField] private float dialogueSpeed = 1;
    [SerializeField] private AudioClip dialogueSFX;

    [Header("Interactables")]
    [SerializeField] private bool triggerInteractable = false;
    [SerializeField] private Interactable[] interactables;
    [SerializeField] private bool triggerDialogue = false;
    [SerializeField] private Dialogue[] npcs;

    private string[] dialogue;
    private string objectName;
    private bool giveObjective = false;
    private string objective = string.Empty;
    private Queue<string> dialogueQueue;
    private bool isTalking = false;
    private bool hasInteracted = false;

    private void Start()
    {
        dialogueQueue = new();
        dialogueTextBox.text = string.Empty;
        nameTextBox.text = string.Empty;
    }

    private IEnumerator Write(string sentence, TextMeshProUGUI textBox)
    {
        isTalking = true;

        textBox.text = string.Empty;
        if (textBox == dialogueTextBox) nameTextBox.text = objectName;

        foreach (char c in sentence.ToCharArray())
        {
            textBox.text += c;
            if (c.ToString() != " ") Camera.main.GetComponent<AudioSource>().PlayOneShot(dialogueSFX);
            yield return new WaitForSeconds(0.1f / dialogueSpeed);
        }

        isTalking = false;
    }
    
    public void StartDialogue()
    {
        Debug.Log("Start of conversation");

        hasInteracted = true;

        dialogueQueue.Clear();

        foreach (string sentence in dialogue) dialogueQueue.Enqueue(sentence);

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
            #region Triggers
            /*
            if (triggerDialogue)
            {
                TriggerDialogue();
            }
            if (triggerInteractable)
            {
                TriggerInteractable();
            }
            */
            #endregion
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
        Debug.Log("End of conversation");
    }

    /*
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
    */

    public bool IsTalking => isTalking;

    public bool HasInteracted => hasInteracted;
    public void TriggerObjective(bool shouldTrigger) => giveObjective = shouldTrigger;

    public void TriggerDialogue(bool shouldTrigger) => triggerDialogue = shouldTrigger;

    public void ToggleInteractBox(bool enable) => interactTextBox.enabled = enable;

    public void ToggleDialogueBox(bool enable) => interactTextBox.enabled = enable;

    public void ToggleNameBox(bool enable) => interactTextBox.enabled = enable;

    public void SetDialogue(string[] dialogue) => this.dialogue = dialogue;

    public void SetName(string name) => objectName = name;

    public void SetObjective(string objective) => this.objective = objective;
}
