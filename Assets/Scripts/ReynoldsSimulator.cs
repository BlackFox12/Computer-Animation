using UnityEngine;
using System.Collections.Generic;

public class ReynoldsSimulator : MonoBehaviour
{
    private static ReynoldsSimulator _instance;
    public static ReynoldsSimulator Instance => _instance;

    [Header("Boids Weights")]
    [Range(0, 1)] public float separationWeight = 0.5f;
    [Range(0, 1)] public float alignmentWeight = 0.3f;
    [Range(0, 1)] public float cohesionWeight = 0.4f;

    [Header("Arrival Parameters")]
    public float slowingDistance = 5f;
    
    [Header("Obstacle Avoidance")]
    public float lookAheadDistance = 5f;
    public float cylinderRadius = 1f;

    private List<ReynoldsAgent> agents = new List<ReynoldsAgent>();
    private ReynoldsAgent leader;

    void Awake()
    {
        if (_instance == null) _instance = this;
    }

    public void RegisterAgent(ReynoldsAgent agent, bool isLeader = false)
    {
        if (isLeader)
            leader = agent;
        agents.Add(agent);
    }

    public Vector3 CalculateSteering(ReynoldsAgent agent)
    {
        Vector3 totalForce = Vector3.zero;

        // Calculate basic forces
        if (agent.isLeader)
        {
            totalForce += Seek(agent, agent.targetPosition);
            totalForce += ObstacleAvoidance(agent);
        }
        else
        {
            // Follower behavior
            Vector3 targetBehindLeader = leader.transform.position - leader.transform.forward * agent.followDistance;
            totalForce += Arrive(agent, targetBehindLeader);
            totalForce += ObstacleAvoidance(agent);
            
            // Boids behaviors for followers
            totalForce += Separation(agent) * separationWeight;
            totalForce += Alignment(agent) * alignmentWeight;
            totalForce += Cohesion(agent) * cohesionWeight;
        }

        return totalForce;
    }

    private Vector3 Seek(ReynoldsAgent agent, Vector3 target)
    {
        Vector3 desired = (target - agent.transform.position).normalized * agent.maxSpeed;
        return desired - agent.velocity;
    }

    private Vector3 Arrive(ReynoldsAgent agent, Vector3 target)
    {
        Vector3 toTarget = target - agent.transform.position;
        float distance = toTarget.magnitude;

        if (distance < 0.1f) return Vector3.zero;

        float targetSpeed = distance > slowingDistance ? 
            agent.maxSpeed : 
            agent.maxSpeed * (distance / slowingDistance);

        Vector3 desired = toTarget.normalized * targetSpeed;
        return desired - agent.velocity;
    }

    private Vector3 ObstacleAvoidance(ReynoldsAgent agent)
    {
        // Get movement direction
        Vector3 forward = agent.velocity.magnitude > 0.1f ? 
            agent.velocity.normalized : 
            agent.transform.forward;

        // Adjust cylinder length based on speed
        float cylinderLength = lookAheadDistance * (agent.velocity.magnitude / agent.maxSpeed + 0.5f);
        
        // Find all potential obstacles
        Collider[] obstacles = Physics.OverlapSphere(
            agent.transform.position + forward * cylinderLength * 0.5f, 
            cylinderLength);

        Vector3 mostThreateningObstacle = Vector3.zero;
        float nearestDistance = float.MaxValue;

        foreach (Collider obstacle in obstacles)
        {
            if (obstacle.CompareTag("Obstacle"))
            {
                // Get closest point on forward axis
                Vector3 directionToObstacle = obstacle.transform.position - agent.transform.position;
                Vector3 projectedPoint = Vector3.Project(directionToObstacle, forward);
                
                // Check if obstacle is in front
                if (Vector3.Dot(projectedPoint, forward) > 0)
                {
                    // Calculate distance from forward axis to obstacle center
                    Vector3 lateralOffset = directionToObstacle - projectedPoint;
                    float lateralDistance = lateralOffset.magnitude;

                    // Check if within cylinder radius
                    if (lateralDistance < cylinderRadius + obstacle.bounds.extents.x)
                    {
                        float distanceToObstacle = projectedPoint.magnitude;
                        
                        // Keep track of nearest threatening obstacle
                        if (distanceToObstacle < nearestDistance)
                        {
                            nearestDistance = distanceToObstacle;
                            mostThreateningObstacle = obstacle.transform.position;
                        }
                    }
                }
            }
        }

        // If we found a threatening obstacle, compute avoidance force
        if (mostThreateningObstacle != Vector3.zero)
        {
            // Project obstacle position onto lateral plane
            Vector3 directionToObstacle = mostThreateningObstacle - agent.transform.position;
            Vector3 lateralDirection = directionToObstacle - Vector3.Project(directionToObstacle, forward);
            
            // Calculate avoidance direction (negative lateral direction)
            Vector3 avoidanceDirection = -lateralDirection.normalized;
            
            // Scale force based on distance and speed
            float forceMagnitude = agent.maxForce * (1.0f - nearestDistance / cylinderLength);
            
            // Store debug info for visualization
            agent.debugAvoidanceForce = avoidanceDirection * forceMagnitude;
            agent.debugNearestObstacle = mostThreateningObstacle;
            agent.debugCylinderLength = cylinderLength;
            
            return avoidanceDirection * forceMagnitude;
        }

        agent.debugAvoidanceForce = Vector3.zero;
        agent.debugNearestObstacle = Vector3.zero;
        agent.debugCylinderLength = cylinderLength;
        
        return Vector3.zero;
    }

    private Vector3 Separation(ReynoldsAgent agent)
    {
        Vector3 steering = Vector3.zero;
        int count = 0;

        foreach (var other in agents)
        {
            if (other != agent)
            {
                float distance = Vector3.Distance(agent.transform.position, other.transform.position);
                if (distance < agent.separationRadius)
                {
                    Vector3 away = agent.transform.position - other.transform.position;
                    steering += away.normalized / distance;
                    count++;
                }
            }
        }

        if (count > 0)
            steering /= count;

        return steering;
    }

    private Vector3 Alignment(ReynoldsAgent agent)
    {
        Vector3 averageVelocity = Vector3.zero;
        int count = 0;

        foreach (var other in agents)
        {
            if (other != agent && Vector3.Distance(agent.transform.position, other.transform.position) < agent.alignmentRadius)
            {
                averageVelocity += other.velocity;
                count++;
            }
        }

        if (count > 0)
        {
            averageVelocity /= count;
            return averageVelocity - agent.velocity;
        }

        return Vector3.zero;
    }

    private Vector3 Cohesion(ReynoldsAgent agent)
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        foreach (var other in agents)
        {
            if (other != agent && Vector3.Distance(agent.transform.position, other.transform.position) < agent.cohesionRadius)
            {
                centerOfMass += other.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            centerOfMass /= count;
            return Seek(agent, centerOfMass);
        }

        return Vector3.zero;
    }
} 