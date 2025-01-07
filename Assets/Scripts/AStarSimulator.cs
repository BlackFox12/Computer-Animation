using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarSimulator : MonoBehaviour
{
    public static AStarSimulator Instance { get; private set; }
    public AStarcrowdInstantiator instantiator;
    private List<AStarAgent> agents = new List<AStarAgent>();

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
        if (instantiator == null) 
            instantiator = FindObjectOfType<AStarcrowdInstantiator>();
        
        StartCoroutine(SimulationCoroutine());
    }

    public void RegisterAgent(AStarAgent agent)
    {
        if (!agents.Contains(agent))
        {
            agents.Add(agent);
        }
    }

    IEnumerator SimulationCoroutine()
    {
        while (true)
        {
            UpdateSimulation(timestep);
            yield return new WaitForSeconds(timestep);
        }
    }

    void UpdateSimulation(float deltaTime)
    {
        foreach (AStarAgent agent in agents)
        {
            AStarGoalManager goalManager = agent.GetComponent<AStarGoalManager>();
            if (goalManager.IsGoalReached(agent))
            {
                goalManager.AssignNewGoal(agent);
            }

            agent.UpdatePosition(deltaTime);
        }
    }
} 