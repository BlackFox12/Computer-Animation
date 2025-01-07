using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarSimulator : MonoBehaviour
{
    public static AStarSimulator Instance { get; private set; }
    public AStarcrowdInstantiator instantiator;
    private List<AStarAgent> agents = new List<AStarAgent>();
    private bool isInitialized = false;

    [Range(0.01f, 1f)]
    public float timestep = 0.016f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        // Wait for PathfindingManager to be initialized
        while (PathfindingManager.Instance == null || !PathfindingManager.Instance.IsInitialized)
        {
            Debug.Log("AStarSimulator waiting for PathfindingManager...");
            yield return new WaitForSeconds(0.1f);
        }

        if (instantiator == null)
            instantiator = FindObjectOfType<AStarcrowdInstantiator>();

        isInitialized = true;
        Debug.Log("AStarSimulator initialized successfully");
        StartCoroutine(SimulationCoroutine());
    }

    public void RegisterAgent(AStarAgent agent)
    {
        if (!agents.Contains(agent))
        {
            agents.Add(agent);
            Debug.Log($"Agent registered. Total agents: {agents.Count}");
        }
    }

    IEnumerator SimulationCoroutine()
    {
        while (true)
        {
            if (isInitialized)
            {
                UpdateSimulation(timestep);
            }
            yield return new WaitForSeconds(timestep);
        }
    }

    void UpdateSimulation(float deltaTime)
    {
        foreach (AStarAgent agent in agents)
        {
            if (agent == null) continue;

            AStarGoalManager goalManager = agent.GetComponent<AStarGoalManager>();
            if (goalManager == null) continue;

            if (goalManager.IsGoalReached(agent))
            {
                goalManager.AssignNewGoal(agent);
            }

            agent.UpdatePosition(deltaTime);
        }
    }
} 