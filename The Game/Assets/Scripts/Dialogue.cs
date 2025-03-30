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
    [SerializeField] private bool canTalk = true;
    [SerializeField] private bool giveObjective = false;
    [TextArea(3, 5), SerializeField] private string[] dialogue = null;
    [TextArea(3, 5), SerializeField] private string objective = null;
    [SerializeField] private TextMeshProUGUI textBox, nameTextBox, interactTextBox, objectiveText;
    [SerializeField] private float interactionDistance = 1;
    [SerializeField] private float dialogueSpeed = 1;
    [SerializeField] private AudioClip dialogueSFX;

    private Queue<string> dialogueQueue;
    private Canvas pauseMenu;
    private GameObject player;
    private bool isTalking = false;

    private void Start()
    {
        dialogueQueue = new();
        pauseMenu = FindFirstObjectByType<PauseMenu>().GetComponent<Canvas>();
        player = GameObject.FindWithTag("Player");
        EndDialogue();
    }

    private void Update()
    {
        if (!canTalk)
        {
            return;
        }

        Vector3 playerPosition = player.transform.position - transform.position;
        bool isPlayerNear = playerPosition.magnitude < interactionDistance;
        textBox.enabled = isPlayerNear;
        interactTextBox.enabled = isPlayerNear;
        nameTextBox.enabled = isPlayerNear;

        if (isPlayerNear)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartDialogue();
            }

            if (!isTalking && Input.GetKeyDown(KeyCode.G) && !pauseMenu.enabled)
            {
                DisplayNextDialogue();
            }

            interactTextBox.text = textBox.text != "" ? "press [g] to continue" + "\n press [e] to restart conversation" : "press [e] to interact";
        }
    }

    private IEnumerator Write(string sentence)
    {
        isTalking = true;

        EndDialogue();
        nameTextBox.text = name;

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
            if (giveObjective)
            {
                objectiveText.text = $"Objective: {objective}";
            }
            return;
        }

        string nextDialogue = dialogueQueue.Dequeue();
        StopAllCoroutines();
        StartCoroutine(Write(nextDialogue));
    }

    private void EndDialogue()
    {
        textBox.text = "";
        nameTextBox.text = "";
        Debug.Log("End of conversation");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
