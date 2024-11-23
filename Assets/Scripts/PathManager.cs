using UnityEngine;

public class PathManager : MonoBehaviour
{
    private float xMin, xMax, zMin, zMax;
    public float bufferRadius = 0.1f;

    public void Initialize(float xMin, float xMax, float zMin, float zMax)
    {
        this.xMin = xMin;
        this.xMax = xMax;
        this.zMin = zMin;
        this.zMax = zMax;
    }

    // Assign a new random target for the agent
    public void AssignNewGoal(Agent agent)
    {
        float randomX = Random.Range(xMin, xMax);
        float randomZ = Random.Range(zMin, zMax);
        agent.targetPosition = new Vector3(randomX, 0f, randomZ);
    }

    // Check if the agent reached its goal
    public bool IsGoalReached(Agent agent)
    {
        return Vector3.Distance(agent.transform.position, agent.targetPosition) < agent.radius + (bufferRadius*agent.velocity);
    }
}
