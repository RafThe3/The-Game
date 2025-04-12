using UnityEngine;
using TMPro;
using UnityEditor.Embree;
using System.Collections;

public class Interactable : MonoBehaviour
{
    [Header("Interactable")]
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private TextMeshProUGUI interactTextBox, objectiveTextBox;
    [SerializeField] private bool giveObjective = true;
    [TextArea(2, 1), SerializeField] private string objective = string.Empty;
    [SerializeField] private float objectiveTextSpeed = 1;
    [SerializeField] private AudioClip objectiveTextSFX;
    [SerializeField] private float interactionDistance = 1;

    [Header("NPCs")]
    [SerializeField] private Dialogue npc;
    [SerializeField] private bool triggerDialogue = false;
    [TextArea(3, 5), SerializeField] private string[] dialogue = null;

    private GameObject player;
    private readonly KeyCode interactButton = KeyCode.E;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Start()
    {
        //interactTextBox.enabled = false;
    }

    private void Update()
    {
        tag = isInteractable ? "Interactable" : "Untagged";

        if (!isInteractable)
        {
            return;
        }

        Vector3 playerPosition = player.transform.position - transform.position;
        bool isPlayerNear = playerPosition.magnitude < interactionDistance;

        if (isPlayerNear && Input.GetKeyDown(interactButton) && giveObjective)
        {
            Interact();
        }
    }

    public void Interact()
    {
        StartCoroutine(Write($"Objective: {objective}", objectiveTextBox));
        npc.ChangeDialogue(dialogue);
        npc.EnableDialogue();
        isInteractable = false;
    }

    private IEnumerator Write(string sentence, TextMeshProUGUI textBox)
    {
        textBox.text = string.Empty;

        foreach (char c in sentence.ToCharArray())
        {
            textBox.text += c;
            Camera.main.GetComponent<AudioSource>().PlayOneShot(objectiveTextSFX);
            yield return new WaitForSecondsRealtime(0.1f / objectiveTextSpeed);
        }
    }

    public void EnableInteract() => isInteractable = true;

    public bool IsInteractable => isInteractable;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
