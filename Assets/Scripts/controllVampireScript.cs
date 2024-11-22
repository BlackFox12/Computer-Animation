using UnityEngine;

public class DragController : MonoBehaviour
{
    private Animator animator; // Reference to the Animator component
    private float speed = 5f; // Movement speed
    private float acceleration = 2f; // Acceleration rate
    private float deceleration = 3f; // Deceleration rate
    private float currentSpeedZ = 0f; // Current speed in the Z direction
    private float speedMultiplier = 1f;

    [Range(1.2f, 3.5f)]
    public float maxSpeed; // Maximum speed multiplier

    public bool canRotate = true; // Toggleable variable for rotation behavior

    private void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    private void Update()
    {
        HandleInput();
        UpdateAnimatorParameters();
    }

    private void HandleInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        bool shouldSpeedUp = Input.GetKey(KeyCode.LeftShift);

        speedMultiplier = shouldSpeedUp ? maxSpeed : 1f;

        HandleMovement(moveHorizontal, moveVertical);
        HandleRotation(moveHorizontal, moveVertical);
    }

    private void HandleMovement(float moveHorizontal, float moveVertical)
    {
        float targetSpeedZ = moveVertical * speed * speedMultiplier;

        // Accelerate or decelerate towards the target speed
        currentSpeedZ = Mathf.MoveTowards(
            currentSpeedZ,
            targetSpeedZ,
            (moveVertical != 0 ? acceleration : deceleration) * Time.deltaTime
        );

        currentSpeedZ = Mathf.Clamp(currentSpeedZ, -1f, 1f);
    }

    private void HandleRotation(float moveHorizontal, float moveVertical)
    {
        if (canRotate && (Mathf.Abs(moveHorizontal) > 0.1f || Mathf.Abs(moveVertical) > 0.1f))
        {
            // Rotate character based on horizontal input
            if (Mathf.Abs(moveHorizontal) > 0.1f)
            {
                float rotationAdjustment = moveHorizontal * 45f * Time.deltaTime;
                transform.Rotate(0f, rotationAdjustment, 0f);
            }
        }
    }

    private void UpdateAnimatorParameters()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        bool isInputActive = Mathf.Abs(moveHorizontal) > 0.1f || Mathf.Abs(moveVertical) > 0.1f;

        if (canRotate)
        {
            // Disable strafing and allow forward movement
            animator.SetFloat("Vel_x", 0f);
            animator.SetFloat("Vel_z", moveVertical > 0.1f ? 1f : 0f);
        }
        else
        {
            // Normal behavior with strafe and forward movement
            animator.SetFloat("Vel_x", moveHorizontal);
            animator.SetFloat("Vel_z", currentSpeedZ);
        }

        // Set idle animation state
        animator.SetBool("isIdle", !isInputActive);

        // Set speed multiplier in the animator
        animator.SetFloat("Speed_multiplier", speedMultiplier);
    }
}
