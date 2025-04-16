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
    [SerializeField] private TextMeshProUGUI coordinatesText;

    [SerializeField] private PlayerInteract playerInteract;
    //[SerializeField] private GameObject projectile;

    private FirstPersonController controller;
    private CharacterController character;
    private bool isMoving;
    private bool isOnSprintCooldown;
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

        playerInteract.interactTextBox.enabled = false;
        playerInteract.collectiblesTextBox.enabled = false;
        playerInteract.collectibles = GameObject.FindGameObjectsWithTag("Collectible");
    }

    private void Update()
    {
        UpdateCoordinates();
        ToggleInteract();

        /*
        if (Input.GetButton("Fire1"))
        {
            GameObject projectileClone = Instantiate(projectile, transform.position, Quaternion.identity);
            projectileClone.transform.Translate(10 * Time.deltaTime * Vector3.forward);
            Destroy(projectileClone, 5);
        }
        */

        isMoving = Mathf.Abs(character.velocity.x) > Mathf.Epsilon;

        if (!infiniteStamina)
        {
            Sprint();
        }
    }

    #region Player Interact

    private void ToggleInteract()
    {
        DetermineObject(out bool isInteractable, out bool isCollectible, out bool isNPC, out RaycastHit hit);

        playerInteract.interactTextBox.enabled = isInteractable || isNPC || isCollectible;

        if (isNPC)
        {
            Dialogue npc = hit.collider.GetComponent<Dialogue>();
            playerInteract.interactTextBox.text = npc.HasInteracted ? playerInteract.dialogueControls : playerInteract.interactText;
            if (npc.HasInteracted)
            {
                playerInteract.interactTextBox.text = npc.IsTalking ? "" : playerInteract.dialogueControls;
            }

            TriggerDialogue(npc);
        }

        if (isCollectible)
        {
            playerInteract.interactTextBox.text = playerInteract.collectText;
            if (Input.GetKeyDown(playerInteract.interactButton))
            {
                StopAllCoroutines();
                StartCoroutine(CollectItem(hit));
                //Debug.Log($"{hit.collider.name}s accumulated: {collectiblesCount} / {collectibles.Length}");
            }
        }

        if (isInteractable)
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            playerInteract.interactTextBox.text = playerInteract.interactText;
            if (Input.GetKeyDown(playerInteract.interactButton))
            {
                interactable.Interact();
            }
        }

        Debug.Log($"is npc: {isNPC}, {isInteractable}, {isCollectible}");
    }

    private void DetermineObject(out bool isInteractable, out bool isCollectible, out bool isNPC, out RaycastHit hit)
    {
        isInteractable = false;
        isCollectible = false;
        isNPC = false;
        Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out hit, playerInteract.raycastHitDistance))
        {
            isInteractable = hit.collider.CompareTag("Interactable");
            isNPC = hit.collider.CompareTag("NPC");
            isCollectible = hit.collider.CompareTag("Collectible");
        }
    }

    private IEnumerator CollectItem(RaycastHit collectible)
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(playerInteract.collectibleSFX);
        playerInteract.collectiblesCount++;
        Destroy(collectible.collider.gameObject);
        playerInteract.collectiblesTextBox.text = $"{collectible.collider.name} collected: \n{playerInteract.collectiblesCount} / {playerInteract.collectibles.Length}";
        playerInteract.collectiblesTextBox.enabled = true;

        yield return new WaitForSeconds(7.5f);

        playerInteract.collectiblesTextBox.enabled = false;
    }

    private void TriggerDialogue(Dialogue npc)
    {
        if (!npc.HasInteracted && Input.GetKeyDown(playerInteract.interactButton))
        {
            npc.StartDialogue();
        }

        if (!npc.IsTalking && Input.GetKeyDown(KeyCode.G) && !pauseMenu.enabled)
        {
            npc.DisplayNextDialogue();
        }
    }

    #endregion

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

    [System.Serializable]
    public class PlayerInteract
    {
        public TextMeshProUGUI interactTextBox, collectiblesTextBox;
        public readonly string interactText = "Press [e] to interact", collectText = "Press [e] to collect", 
                                dialogueControls = "Press [g] to continue";
        public readonly KeyCode interactButton = KeyCode.E;
        [HideInInspector] public GameObject[] collectibles;
        [HideInInspector] public float collectiblesCount = 0;
        public AudioClip collectibleSFX;
        public float raycastHitDistance = 1;
    }
}
