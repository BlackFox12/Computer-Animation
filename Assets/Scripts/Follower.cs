using UnityEngine;

public class Follower : SteeringAgent
{
    public Transform leader;
    public float followDistance = 3f;

    [Header("Weights")]
    [Range(0, 1)] public float seekWeight = 0.7f;
    [Range(0, 1)] public float obstacleAvoidanceWeight = 0.3f;

    [Header("Obstacle Avoidance")]
    public float avoidanceRadius = 2f;
    public float lookAheadDistance = 5f;
    public SteeringAgent agent;

    private SteeringBehaviors steering;
    private Collider[] obstacles;

    private void Start()
    {
        steering = GetComponent<SteeringBehaviors>();
        agent = GetComponent<SteeringAgent>();
    }

    private void Update()
    {
        // Compute forces
        Vector3 seekForce = steering.Seek(this, leader.position - leader.forward * followDistance) * seekWeight;
        Vector3 avoidanceForce = steering.ObstacleAvoidance(this, obstacles, avoidanceRadius, lookAheadDistance) * obstacleAvoidanceWeight;

        // Combine and normalize weights
        float totalWeight = seekWeight + obstacleAvoidanceWeight;
        Vector3 steeringForce = (seekForce + avoidanceForce) / totalWeight;

        // Apply and move
        ApplyForce(steeringForce);
        Move();
    }
}
