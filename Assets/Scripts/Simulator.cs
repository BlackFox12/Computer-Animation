using UnityEngine;
using System.Collections;

public class Simulator : MonoBehaviour
{
    public static Simulator _instance = null;
    public crowdInstantiator instantiator;
    private ArrayList agents;

    [Range(0.01f, 1f)]
    public float timestep;

    public static Simulator GetInstance()
    {
        if (_instance == null)
        {
            GameObject simulatorGameObject = new GameObject("Simulator");
            _instance = simulatorGameObject.AddComponent<Simulator>();
        }
        return _instance;
    }

    void Start()
    {
        if (instantiator == null) instantiator = FindObjectOfType<crowdInstantiator>();
        agents = instantiator.GetAgents();
        StartCoroutine(SimulationCoroutine());
    }

    IEnumerator SimulationCoroutine()
    {
        while (true)
        {
            UpdateSimulation(timestep);
            yield return new WaitForSeconds(timestep);
        }
    }

    void UpdateSimulation(float timestep)
    {
        foreach (Agent agent in agents)
        {
            PathManager pathManager = agent.GetComponent<PathManager>();
            if (pathManager.IsGoalReached(agent))
            {
                pathManager.AssignNewGoal(agent);
            }

            // Move agent towards its target
            Vector3 direction = (agent.targetPosition - agent.transform.position).normalized;
            agent.transform.position += direction * agent.velocity * timestep;
        }
    }
}
