using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
    public Vector3 Seek(SteeringAgent agent, Vector3 target)
    {
        Vector3 desiredVelocity = (target - agent.transform.position).normalized * agent.maxSpeed;
        return desiredVelocity - agent.Velocity;
    }

    public Vector3 Flee(SteeringAgent agent, Vector3 target)
    {
        Vector3 desiredVelocity = (agent.transform.position - target).normalized * agent.maxSpeed;
        return desiredVelocity - agent.Velocity;
    }

    public Vector3 Arrive(SteeringAgent agent, Vector3 target, float slowingDistance)
    {
        Vector3 toTarget = target - agent.transform.position;
        float distance = toTarget.magnitude;

        if (distance < 0.1f) return Vector3.zero;

        float rampedSpeed = agent.maxSpeed * (distance / slowingDistance);
        float clippedSpeed = Mathf.Min(rampedSpeed, agent.maxSpeed);
        Vector3 desiredVelocity = (toTarget / distance) * clippedSpeed;
        return desiredVelocity - agent.Velocity;
    }

    public Vector3 ObstacleAvoidance(SteeringAgent agent, Collider[] obstacles, float avoidanceRadius, float lookAheadDistance)
    {
        Vector3 avoidanceForce = Vector3.zero;

        foreach (var obstacle in obstacles)
        {
            Vector3 directionToObstacle = obstacle.ClosestPoint(agent.transform.position) - agent.transform.position;
            float distance = directionToObstacle.magnitude;

            if (distance < lookAheadDistance && Vector3.Dot(agent.transform.forward, directionToObstacle.normalized) > 0)
            {
                Vector3 lateralAvoidance = Vector3.Cross(directionToObstacle, Vector3.up).normalized;
                avoidanceForce += lateralAvoidance / distance;
            }
        }

        return avoidanceForce.normalized * agent.maxSpeed;
    }
}
