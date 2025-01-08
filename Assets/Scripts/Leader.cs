using UnityEngine;

public class Leader : SteeringAgent
{
    public Transform goal;
    public float slowingDistance = 3f;

    [Header("Weights")]
    [Range(0, 1)] public float seekWeight = 0.5f;
    [Range(0, 1)] public float arriveWeight = 0.5f;
    [Range(0, 1)] public float obstacleAvoidanceWeight = 0.5f;

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
        Vector3 seekForce = steering.Seek(this, goal.position) * seekWeight;
        Vector3 arriveForce = steering.Arrive(this, goal.position, slowingDistance) * arriveWeight;
        Vector3 avoidanceForce = steering.ObstacleAvoidance(this, obstacles, avoidanceRadius, lookAheadDistance) * obstacleAvoidanceWeight;

        // Combine and normalize weights
        float totalWeight = seekWeight + arriveWeight + obstacleAvoidanceWeight;
        Vector3 steeringForce = (seekForce + arriveForce + avoidanceForce) / totalWeight;

        // Apply and move
        ApplyForce(steeringForce);
        Move();

        // Check goal reached
        if (Vector3.Distance(transform.position, goal.position) < 1f)
        {
            RandomizeGoal();
        }
    }

    private void RandomizeGoal()
    {
        goal.position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
    }
}
