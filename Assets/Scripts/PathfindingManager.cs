using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance { get; private set; }

    [Header("References")]
    public GridManager gridManager;
    public AStarPathfinder pathfinder;
    public AStarSimulator simulator;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeComponents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeComponents()
    {
        // Create AStarPathfinder if it doesn't exist
        if (pathfinder == null)
        {
            GameObject pathfinderObj = new GameObject("AStarPathfinder");
            pathfinderObj.transform.parent = transform;
            pathfinder = pathfinderObj.AddComponent<AStarPathfinder>();
        }

        // Create AStarSimulator if it doesn't exist
        if (simulator == null)
        {
            GameObject simulatorObj = new GameObject("AStarSimulator");
            simulatorObj.transform.parent = transform;
            simulator = simulatorObj.AddComponent<AStarSimulator>();
        }

        // Find GridManager if not assigned
        if (gridManager == null)
        {
            gridManager = FindObjectOfType<GridManager>();
            if (gridManager == null)
                Debug.LogError("GridManager not found in scene!");
        }
    }

    public AStarPathfinder GetPathfinder()
    {
        return pathfinder;
    }

    public AStarSimulator GetSimulator()
    {
        return simulator;
    }
} 