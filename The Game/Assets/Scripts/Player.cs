using StarterAssets;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using NUnit.Framework.Constraints;
using System.Runtime.CompilerServices;
using System.Collections;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private bool infiniteStamina = false;
    [Min(0), SerializeField] private float sprintDuration = 1;
    [Min(0), SerializeField] private float sprintCooldownTime = 1;
    [Min(0), SerializeField] private float staminaRecoverySpeed = 1;

    [Header("UI")]
    [SerializeField] private Slider staminaBar;
    [SerializeField] private Image staminaBarFillArea;
    [SerializeField] private Animator staminaAnimator;
    [SerializeField] private TextMeshProUGUI coordinatesText, interactTextBox, collectiblesTextBox;
    [SerializeField] private float raycastHitDistance = 1;

    private FirstPersonController controller;
    private CharacterController character;
    private bool isMoving;
    private bool isOnSprintCooldown;
    private readonly string interactText = "Press [e] to interact";
    private readonly string collectText = "Press [e] to collect";
    private readonly string dialogueControls = "Press [g] to continue";
    private float collectiblesCount = 0;
    private GameObject[] collectibles;
    private Canvas pauseMenu;
    private readonly KeyCode interactButton = KeyCode.E;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
        controller = GetComponent<FirstPersonController>();
        pauseMenu = FindFirstObjectByType<PauseMenu>().GetComponent<Canvas>();
    }

    private void Start()
    {
        staminaBar.maxValue = sprintDuration;
        staminaBar.value = sprintDuration;
        interactTextBox.enabled = false;
        collectiblesTextBox.enabled = false;
        collectibles = GameObject.FindGameObjectsWithTag("Collectible");
    }

    private void Update()
    {
        UpdateCoordinates();
        ToggleInteract();

        isMoving = Mathf.Abs(character.velocity.x) > Mathf.Epsilon;

        if (!infiniteStamina)
        {
            Sprint();
        }
    }

    private void ToggleInteract()
    {
        DetermineObject(out bool isInteractable, out bool isCollectible, out bool isNPC, out RaycastHit hit);

        interactTextBox.enabled = isInteractable || isNPC || isCollectible;

        if (isNPC)
        {
            Dialogue npc = hit.collider.GetComponent<Dialogue>();
            interactTextBox.text = npc.HasInteracted ? dialogueControls : interactText;
            if (npc.HasInteracted)
            {
                interactTextBox.text = npc.IsTalking ? "" : dialogueControls;
            }

            TriggerDialogue(npc);
        }

        if (isCollectible)
        {
            interactTextBox.text = collectText;
            if (Input.GetKeyDown(interactButton))
            {
                StopAllCoroutines();
                StartCoroutine(CollectItem(hit));
                //Debug.Log($"{hit.collider.name}s accumulated: {collectiblesCount} / {collectibles.Length}");
            }
        }

        if (isInteractable)
        {
            interactTextBox.text = interactText;
        }

        Debug.Log($"is npc: {isNPC}, {isInteractable}, {isCollectible}");
    }

    private void DetermineObject(out bool isInteractable, out bool isCollectible, out bool isNPC, out RaycastHit hit)
    {
        isInteractable = false;
        isCollectible = false;
        isNPC = false;
        Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out hit, raycastHitDistance))
        {
            isInteractable = hit.collider.CompareTag("Interactable");
            isNPC = hit.collider.CompareTag("NPC");
            isCollectible = hit.collider.CompareTag("Collectible");
        }
    }

    private IEnumerator CollectItem(RaycastHit collectible)
    {
        collectiblesCount++;
        Destroy(collectible.collider.gameObject);
        collectiblesTextBox.text = $"{collectible.collider.name} collected: \n{collectiblesCount} / {collectibles.Length}";
        collectiblesTextBox.enabled = true;

        yield return new WaitForSeconds(7.5f);

        collectiblesTextBox.enabled = false;
    }

    private void TriggerDialogue(Dialogue npc)
    {
        if (!npc.HasInteracted && Input.GetKeyDown(interactButton))
        {
            npc.StartDialogue();
        }

        if (!npc.IsTalking && Input.GetKeyDown(KeyCode.G) && !pauseMenu.enabled)
        {
            npc.DisplayNextDialogue();
        }
    }

    private void UpdateCoordinates()
    {
        float x = Mathf.Round(transform.position.x);
        float y = Mathf.Round(transform.position.y);
        float z = Mathf.Round(transform.position.z);
        
        coordinatesText.text = $"x: {x} / y: {y} / z: {z}";
    }

    private void Sprint()
    {
        if (staminaBar.value <= 0)
        {
            isOnSprintCooldown = true;
            controller.SprintSpeed = controller.MoveSpeed;
            staminaBarFillArea.color = Color.red;
        }

        if (isOnSprintCooldown && staminaBar.value >= sprintCooldownTime)
        {
            isOnSprintCooldown = false;
            controller.SprintSpeed = controller._tempSprintSpeed;
            staminaBarFillArea.color = Color.yellow;
        }

        if (controller._isSprinting && !isOnSprintCooldown && isMoving)
        {
            staminaBar.value -= Time.deltaTime;
            staminaAnimator.SetBool("isSprinting", true);
        }
        else if (staminaBar.value < staminaBar.maxValue)
        {
            staminaBar.value += Time.deltaTime * staminaRecoverySpeed;
        }

        if (staminaBar.value == staminaBar.maxValue)
        {
            staminaAnimator.SetBool("isSprinting", false);
        }
    }
}
