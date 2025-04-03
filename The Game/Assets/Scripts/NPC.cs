using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.TextCore.Text;
using UnityEditor.ShaderGraph.Internal;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;

public class NPC : MonoBehaviour
{
    [SerializeField] private bool canTalk = true;
    [SerializeField] private bool giveObjective = false;
    [TextArea(3, 5), SerializeField] private string[] dialogue = null;
    [TextArea(3, 5), SerializeField] private string objective = string.Empty;
    [SerializeField] private TextMeshProUGUI dialogueTextBox, nameTextBox, interactTextBox, objectiveTextBox;
    [SerializeField] private float interactionDistance = 1;
    [SerializeField] private float dialogueSpeed = 1;
    [SerializeField] private AudioClip dialogueSFX;
    [SerializeField] private Interactable interactable;

    private Queue<string> dialogueQueue;
    private Canvas pauseMenu;
    private GameObject player;
    private bool isTalking = false;
    private bool hasInteracted = false;
    private readonly string dialogueControls = "Press [g] to continue" + "\n Press [e] to restart conversation";

    private void Start()
    {
        dialogueQueue = new();
        pauseMenu = FindFirstObjectByType<PauseMenu>().GetComponent<Canvas>();
        player = GameObject.FindWithTag("Player");
        dialogueTextBox.text = string.Empty;
        nameTextBox.text = string.Empty;
    }

    private void Update()
    {
        if (!canTalk)
        {
            return;
        }

        Vector3 playerPosition = player.transform.position - transform.position;
        bool isPlayerNear = playerPosition.magnitude < interactionDistance;
        dialogueTextBox.enabled = isPlayerNear;
        nameTextBox.enabled = isPlayerNear;
        if (hasInteracted)
        {
            interactTextBox.enabled = isPlayerNear;
        }

        if (isPlayerNear)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                hasInteracted = true;
                StartDialogue();
            }

            if (!isTalking && Input.GetKeyDown(KeyCode.G) && !pauseMenu.enabled)
            {
                DisplayNextDialogue();
            }

            if (dialogueTextBox.text != string.Empty)
            {
                interactTextBox.text = dialogueControls;
            }
        }
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
            Camera.main.GetComponent<AudioSource>().PlayOneShot(dialogueSFX);
            yield return new WaitForSecondsRealtime(dialogueSpeed);
        }

        isTalking = false;
    }
    
    private void StartDialogue()
    {
        Debug.Log("Start of conversation");

        dialogueQueue.Clear();

        foreach (string sentence in dialogue)
        {
            dialogueQueue.Enqueue(sentence);
        }

        DisplayNextDialogue();
    }

    private void DisplayNextDialogue()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            if (hasInteracted && giveObjective && objective != string.Empty)
            {
                StartCoroutine(Write($"Objective: {objective}", objectiveTextBox));
                objective = string.Empty;
                interactable.EnableInteract();
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
        Debug.Log("End of conversation");
    }

    public void ChangeDialogue(string[] dialogue)
    {
        this.dialogue = dialogue;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
