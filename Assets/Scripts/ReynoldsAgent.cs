using UnityEngine;
using System.Collections.Generic;

public class ReynoldsAgent : MonoBehaviour
{
    public bool isLeader = false;
    public float maxSpeed = 5f;
    public float maxForce = 10f;
    public float mass = 1f;

    [Header("Following Parameters")]
    public float followDistance = 3f;
    
    [Header("Flocking Parameters")]
    public float separationRadius = 2f;
    public float alignmentRadius = 5f;
    public float cohesionRadius = 5f;

    [Header("Debug Visualization")]
    public bool showDebugVisuals = true;
    public Color rayColor = Color.yellow;
    public Color cylinderColor = Color.cyan;
    public Color goalColor = Color.green;

    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public Vector3 targetPosition;
    [HideInInspector]
    public List<Vector3> debugRayPositions = new List<Vector3>();
    [HideInInspector]
    public List<Vector3> debugRayHits = new List<Vector3>();

    private void Start()
    {
        ReynoldsSimulator.Instance.RegisterAgent(this, isLeader);
        
        if (isLeader)
            AssignNewRandomGoal();
    }

    private void Update()
    {
        Vector3 steering = ReynoldsSimulator.Instance.CalculateSteering(this);
        
        // Apply steering force
        Vector3 acceleration = steering / mass;
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        
        // Move
        transform.position += velocity * Time.deltaTime;
        
        // Rotate to face movement direction
        if (velocity.magnitude > 0.1f)
            transform.forward = velocity.normalized;

        // Check if leader reached goal
        if (isLeader && Vector3.Distance(transform.position, targetPosition) < 1f)
            AssignNewRandomGoal();
    }

    private void AssignNewRandomGoal()
    {
        targetPosition = new Vector3(
            Random.Range(-20f, 20f),
            1f,
            Random.Range(-20f, 20f)
        );
    }

    private void OnDrawGizmos()
    {
        if (!showDebugVisuals) return;

        // Draw obstacle avoidance rays
        if (debugRayPositions.Count > 0)
        {
            Gizmos.color = rayColor;
            for (int i = 0; i < debugRayPositions.Count; i++)
            {
                Gizmos.DrawLine(debugRayPositions[i], debugRayHits[i]);
            }

            // Draw cylinder wireframe
            Gizmos.color = cylinderColor;
            Vector3 forward = velocity.magnitude > 0.1f ? velocity.normalized : transform.forward;
            DrawWireCylinder(transform.position, forward, 
                ReynoldsSimulator.Instance.cylinderRadius, 
                ReynoldsSimulator.Instance.lookAheadDistance);
        }

        // Draw goal for leader
        if (isLeader)
        {
            Gizmos.color = goalColor;
            Gizmos.DrawWireSphere(targetPosition, 0.5f);
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }

    private void DrawWireCylinder(Vector3 position, Vector3 direction, float radius, float length)
    {
        Vector3 forward = direction * length;
        Vector3 up = Vector3.up;
        Vector3 right = Vector3.Cross(up, forward).normalized * radius;
        Vector3 start = position;
        Vector3 end = position + forward;

        // Draw end caps
        DrawCircle(start, right, up, cylinderColor);
        DrawCircle(end, right, up, cylinderColor);

        // Draw lines connecting caps
        int segments = 8;
        float angleStep = 360f / segments;
        for (float angle = 0; angle < 360; angle += angleStep)
        {
            Vector3 point1 = start + Quaternion.Euler(0, angle, 0) * right;
            Vector3 point2 = end + Quaternion.Euler(0, angle, 0) * right;
            Gizmos.DrawLine(point1, point2);
        }
    }

    private void DrawCircle(Vector3 center, Vector3 right, Vector3 up, Color color)
    {
        Gizmos.color = color;
        int segments = 16;
        float angleStep = 360f / segments;
        
        for (float angle = 0; angle < 360; angle += angleStep)
        {
            Vector3 point1 = center + Quaternion.Euler(0, angle, 0) * right;
            Vector3 point2 = center + Quaternion.Euler(0, angle + angleStep, 0) * right;
            Gizmos.DrawLine(point1, point2);
        }
    }
} 