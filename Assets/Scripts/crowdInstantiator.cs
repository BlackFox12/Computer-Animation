using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class crowdInstantiator : MonoBehaviour
{
    public int agentAmount = 10;
    private float zMin, xMin = -1;
    private float zMax, xMax = 1;
    public GameObject character;
    //public Agent agent;
    private ArrayList agents = new ArrayList();
    void Start()
    {
        float scaleX = transform.localScale.x * 5 -1;
        float scaleZ = transform.localScale.z * 5 -1;
        xMin = -scaleX;
        xMax = scaleX;
        zMin = -scaleZ;
        zMax = scaleZ;

        for (int i = 0; i < agentAmount; i++)
        {
            float randomPositionX = Random.Range(xMin, xMax);
            float randomPositionZ = Random.Range(zMin, zMax);

            Vector3 randomPosition = new Vector3(randomPositionX, 1f, randomPositionZ);
            Quaternion randomRotation = Quaternion.Euler(0,0,0);

            Object agent = Object.Instantiate(character, randomPosition, randomRotation, null);
            agents.Add(agent);
        }
        
    }

    public ArrayList getAgents() { return agents; }
}
