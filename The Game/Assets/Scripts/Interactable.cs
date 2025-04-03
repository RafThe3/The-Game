using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private TextMeshProUGUI interactTextBox, objectiveTextBox;
    [SerializeField] private bool giveObjective = true;
    [TextArea(2, 1), SerializeField] private string objective = string.Empty;
    [TextArea(3, 5), SerializeField] private string[] dialogue = null;
    [SerializeField] private float interactionDistance = 1;
    [SerializeField] private NPC npc;

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
        if (!isInteractable)
        {
            return;
        }

        Vector3 playerPosition = player.transform.position - transform.position;
        bool isPlayerNear = playerPosition.magnitude < interactionDistance;

        if (isPlayerNear)
        {
            if (Input.GetKeyDown(interactButton) && giveObjective)
            {
                objectiveTextBox.text = $"Objective: {objective}";
                npc.ChangeDialogue(dialogue);
            }
            isInteractable = false;
        }
    }

    public void EnableInteract()
    {
        isInteractable = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
