using UnityEngine;
using System.Collections;

public class crowdInstantiator : MonoBehaviour
{
    public int agentAmount = 10;
    public GameObject characterPrefab;
    private float xMin, xMax, zMin, zMax;

    private ArrayList agents = new ArrayList();

    void Start()
    {
        // Calculate spawn boundaries based on the plane size
        float scaleX = transform.localScale.x * 5 - 1;
        float scaleZ = transform.localScale.z * 5 - 1;
        xMin = -scaleX;
        xMax = scaleX;
        zMin = -scaleZ;
        zMax = scaleZ;

        // Spawn agents
        for (int i = 0; i < agentAmount; i++)
        {
            float randomPositionX = Random.Range(xMin, xMax);
            float randomPositionZ = Random.Range(zMin, zMax);

            Vector3 randomPosition = new Vector3(randomPositionX, 0f, randomPositionZ);
            Quaternion randomRotation = Quaternion.identity;

            // Instantiate agent prefab
            GameObject agentObject = Instantiate(characterPrefab, randomPosition, randomRotation);

            // Retrieve existing Agent and PathManager components
            Agent agent = agentObject.GetComponent<Agent>();
            PathManager pathManager = agentObject.GetComponent<PathManager>();

            // Ensure PathManager is initialized and assign the first goal
            pathManager.Initialize(xMin, xMax, zMin, zMax);
            pathManager.AssignNewGoal(agent);

            // Add to agents list
            agents.Add(agent);
        }
    }

    public ArrayList GetAgents() { return agents; }
}
