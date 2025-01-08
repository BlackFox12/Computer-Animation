using UnityEngine;

public class SteeringAgent : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float maxForce = 10f;

    public Vector3 velocity;

    public void ApplyForce(Vector3 force)
    {
        Vector3 truncatedForce = Truncate(force, maxForce);
        Vector3 acceleration = truncatedForce / GetComponent<Rigidbody>().mass;
        velocity += acceleration * Time.deltaTime;
        velocity = Truncate(velocity, maxSpeed);
    }

    public void Move()
    {
        transform.position += velocity * Time.deltaTime;
        if (velocity.magnitude > 0.1f)
        {
            transform.forward = velocity.normalized;
        }
    }

    private Vector3 Truncate(Vector3 v, float max)
    {
        float size = Mathf.Min(v.magnitude, max);
        return v.normalized * size;
    }

    public Vector3 Velocity => velocity;
}
