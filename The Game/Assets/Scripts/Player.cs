using StarterAssets;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using TMPro;

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

    private FirstPersonController controller;
    private CharacterController character;
    private bool isMoving;
    private bool isOnSprintCooldown;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
        controller = GetComponent<FirstPersonController>();
    }

    private void Start()
    {
        staminaBar.maxValue = sprintDuration;
        staminaBar.value = sprintDuration;
    }

    private void Update()
    {
        float x = Mathf.Round(transform.position.x);
        float y = Mathf.Round(transform.position.y);
        float z = Mathf.Round(transform.position.z);
        coordinatesText.text = $"x: {x} \ny: {y} \nz: {z}";

        isMoving = Mathf.Abs(character.velocity.x) > Mathf.Epsilon;

        if (!infiniteStamina)
        {
            Sprint();
        }
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
