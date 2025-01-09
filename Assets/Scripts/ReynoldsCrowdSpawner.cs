using UnityEngine;

public class ReynoldsCrowdSpawner : MonoBehaviour
{
    public GameObject leaderPrefab;
    public GameObject followerPrefab;
    public int numberOfFollowers = 5;
    public float spawnRadius = 5f;

    void Start()
    {
        // Spawn leader
        GameObject leaderObj = Instantiate(leaderPrefab, Random.insideUnitSphere * spawnRadius, Quaternion.identity);
        ReynoldsAgent leaderAgent = leaderObj.GetComponent<ReynoldsAgent>();
        leaderAgent.isLeader = true;

        // Spawn followers
        for (int i = 0; i < numberOfFollowers; i++)
        {
            GameObject followerObj = Instantiate(followerPrefab, Random.insideUnitSphere * spawnRadius, Quaternion.identity);
            ReynoldsAgent followerAgent = followerObj.GetComponent<ReynoldsAgent>();
            followerAgent.isLeader = false;
        }
    }
} 