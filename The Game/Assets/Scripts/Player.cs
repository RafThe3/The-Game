using StarterAssets;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using NUnit.Framework.Constraints;

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
    [SerializeField] private TextMeshProUGUI coordinatesText, interactTextBox;
    [SerializeField] private float raycastHitDistance = 1;

    private FirstPersonController controller;
    private CharacterController character;
    private bool isMoving;
    private bool isOnSprintCooldown;
    private readonly string interactText = "Press [e] to interact";
    private float collectibles = 0;
    private Canvas pauseMenu;

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
        bool isInteractable = false, isCollectible = false, isNPC = false;

        if (interactTextBox.text == string.Empty)
        {
            interactTextBox.text = interactText;
        }

        Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastHitDistance))
        {
            isInteractable = hit.collider.CompareTag("Interactable");
            isNPC = hit.collider.CompareTag("NPC");
            isCollectible = hit.collider.CompareTag("Collectible");
        }

        interactTextBox.enabled = isInteractable || isNPC || isCollectible;

        if (isNPC)
        {
            Dialogue npc = hit.collider.GetComponent<Dialogue>();
            if (!npc.HasInteracted && Input.GetKeyDown(KeyCode.E))
            {
                npc.StartDialogue();
            }

            if (!npc.IsTalking && Input.GetKeyDown(KeyCode.G) && !pauseMenu.enabled)
            {
                npc.DisplayNextDialogue();
            }
        }

        Debug.Log($"is npc: {isNPC}, {isInteractable}, {isCollectible}");
    }

    private void UpdateCoordinates()
    {
        float x = Mathf.Round(transform.position.x);
        float y = Mathf.Round(transform.position.y);
        float z = Mathf.Round(transform.position.z);
        
        coordinatesText.text = $"x: {x} \ny: {y} \nz: {z}";
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
