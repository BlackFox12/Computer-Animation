using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject leaderPrefab;
    public GameObject followerPrefab;
    public Transform goalPrefab;
    public int followerCount = 5;

    private GameObject leader;
    private Transform goal;

    private void Start()
    {
        goal = Instantiate(goalPrefab, Vector3.zero, Quaternion.identity);
        leader = Instantiate(leaderPrefab, Vector3.zero, Quaternion.identity);
        leader.GetComponent<Leader>().goal = goal;

        for (int i = 0; i < followerCount; i++)
        {
            Vector3 spawnPos = leader.transform.position + Random.insideUnitSphere * 2f;
            spawnPos.y = 0;

            GameObject follower = Instantiate(followerPrefab, spawnPos, Quaternion.identity);
            follower.GetComponent<Follower>().leader = leader.transform;
        }
    }
}
