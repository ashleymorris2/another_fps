using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class Spawn : MonoBehaviour
    {
        [SerializeField] private GameObject[] zombiePrefabs;
        [SerializeField] private int amountToSpawn;
        [SerializeField] private float spawnRadius;

        private void Start()
        {
            for (var i = 0; i < amountToSpawn; i++)
            {
                var randomSpawnPoint = transform.position + Random.insideUnitSphere * spawnRadius;
                if (NavMesh.SamplePosition(randomSpawnPoint, out var hit, spawnRadius, NavMesh.AllAreas))
                {
                    Instantiate(zombiePrefabs[Random.Range(0, zombiePrefabs.Length - 1)], hit.position, Quaternion.identity);
                }
                else
                {
                    i--;
                }
            }
        }
    }
}