using UnityEngine;

public class AStarcrowdInstantiator : MonoBehaviour
{
    public int agentAmount = 1;
    public GameObject characterPrefab;
    private GridManager gridManager;
    private AStarSimulator simulator;

    void Start()
    {
        // Find references before Start
        gridManager = FindObjectOfType<GridManager>();
        simulator = FindObjectOfType<AStarSimulator>();

        if (gridManager == null)
            Debug.LogError("GridManager not found in the scene!");
        if (simulator == null)
            Debug.LogError("AStarSimulator not found in the scene!");
        if (characterPrefab == null)
            Debug.LogError("Character Prefab not assigned!");
        // Wait for one frame to ensure grid is initialized
        Invoke("SpawnAgents", 0.1f);
    }

    public void SpawnAgents()
    {
        if (characterPrefab == null || gridManager == null || simulator == null || gridManager.GetGrid() == null)
        {
            Debug.LogError("Missing required components for spawning agents!");
            return;
        }

        for (int i = 0; i < agentAmount; i++)
        {
            // Find a random non-occupied cell for spawning
            GridCell spawnCell = null;
            for (int attempts = 0; attempts < 10; attempts++)
            {
                int randomIndex = Random.Range(0, gridManager.GetGrid().getNumNodes());
                GridCell cell = gridManager.GetGrid().getNode(randomIndex);
                if (cell != null && !cell.IsOccupied)
                {
                    spawnCell = cell;
                    break;
                }
            }

            if (spawnCell != null)
            {
                // Instantiate agent at the cell position
                GameObject agentObject = Instantiate(characterPrefab, spawnCell.center, Quaternion.identity);
                
                // Add and setup components
                AStarAgent aStarAgent = agentObject.AddComponent<AStarAgent>();
                AStarGoalManager goalManager = agentObject.AddComponent<AStarGoalManager>();
                A_StarVampireAnimationController animController = agentObject.AddComponent<A_StarVampireAnimationController>();
                
                // Register with simulator
                simulator.RegisterAgent(aStarAgent);
                
                // Assign initial goal
                goalManager.AssignNewGoal(aStarAgent);
            }
        }
    }
}
