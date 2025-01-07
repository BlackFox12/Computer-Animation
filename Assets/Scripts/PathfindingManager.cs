using UnityEngine;
using System.Collections;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance { get; private set; }

    [Header("References")]
    public GridManager gridManager;
    public AStarPathfinder pathfinder;
    public AStarSimulator simulator;

    public bool IsInitialized { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            StartCoroutine(InitializeComponents());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator InitializeComponents()
    {
        IsInitialized = false;

        // Find or wait for GridManager
        while (gridManager == null)
        {
            gridManager = FindObjectOfType<GridManager>();
            if (gridManager == null)
            {
                Debug.Log("Waiting for GridManager...");
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Wait for grid to be initialized
        while (gridManager.GetGrid() == null)
        {
            Debug.Log("Waiting for Grid to initialize...");
            yield return new WaitForSeconds(0.1f);
        }

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

        // Wait one more frame to ensure everything is properly set up
        yield return null;

        IsInitialized = true;
        Debug.Log("PathfindingManager initialization complete!");
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