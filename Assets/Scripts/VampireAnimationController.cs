using UnityEngine;

public class VampireAnimationController : MonoBehaviour
{
    private Animator animator;
    private Agent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<Agent>();
    }

    void Update()
    {
        UpdateAnimationParameters();
    }

    private void UpdateAnimationParameters()
    {
        // Calculate movement vector and speed
        Vector3 movement = agent.targetPosition - transform.position;
        float speed = movement.magnitude / Time.deltaTime;

        // Convert movement direction to local space
        Vector3 localDirection = transform.InverseTransformDirection(movement.normalized);

        // Update animator parameters
        animator.SetFloat("Vel_x", localDirection.x);
        animator.SetFloat("Vel_z", localDirection.z);
        animator.SetFloat("Speed_multiplier", Mathf.Clamp01(speed / agent.velocity));
        animator.SetBool("isIdle", speed < 0.1f);

        // Rotate character to face movement direction
        if (speed > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(movement.x, 0f, movement.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
