using UnityEngine;

public class A_StarVampireAnimationController : MonoBehaviour
{
    private Animator animator;
    private AStarAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<AStarAgent>();

        if (animator == null)
            Debug.LogError("No Animator component found on vampire!");
        if (agent == null)
            Debug.LogError("No AStarAgent component found on vampire!");
    }

    void Update()
    {
        if (animator == null || agent == null) return;

        UpdateAnimationParameters();
    }

    private void UpdateAnimationParameters()
    {
        // Calculate movement vector and speed
        Vector3 targetPos = agent.GetTargetPosition();
        Vector3 movement = targetPos - transform.position;
        float speed = movement.magnitude / Time.deltaTime;

        // Convert movement direction to local space
        Vector3 localDirection = transform.InverseTransformDirection(movement.normalized);

        // Update animator parameters
        animator.SetFloat("Vel_x", localDirection.x);
        animator.SetFloat("Vel_z", localDirection.z);
        animator.SetFloat("Speed_multiplier", Mathf.Clamp01(speed / agent.velocity));
        animator.SetBool("isIdle", agent.IsPathComplete() || speed < 0.1f);

        // Rotate character to face movement direction
        if (speed > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(movement.x, 0f, movement.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
} 