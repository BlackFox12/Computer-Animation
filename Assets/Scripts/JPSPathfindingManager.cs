using UnityEngine;
using System.Collections;

public class JPSPathfindingManager : MonoBehaviour
{
    public static JPSPathfindingManager Instance { get; private set; }

    [Header("References")]
    public JPSGridManager gridManager;
    public JPSPathfinder pathfinder;

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

        while (gridManager == null)
        {
            gridManager = FindObjectOfType<JPSGridManager>();
            if (gridManager == null)
            {
                Debug.Log("Waiting for GridManager...");
                yield return new WaitForSeconds(0.1f);
            }
        }

        while (gridManager.GetGrid() == null)
        {
            Debug.Log("Waiting for Grid to initialize...");
            yield return new WaitForSeconds(0.1f);
        }

        if (pathfinder == null)
        {
            GameObject pathfinderObj = new GameObject("JPSPathfinder");
            pathfinderObj.transform.parent = transform;
            pathfinder = pathfinderObj.AddComponent<JPSPathfinder>();
        }

        yield return null;

        IsInitialized = true;
        Debug.Log("JPSPathfindingManager initialization complete!");
    }

    public JPSPathfinder GetPathfinder()
    {
        return pathfinder;
    }
}