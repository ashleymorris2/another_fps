using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemy
{
    [RequireComponent(typeof(SphereCollider))]
    public class Spawn : MonoBehaviour
    {
        [SerializeField] private GameObject[] zombiePrefabs;
        [SerializeField] private int amountToSpawn;
        [SerializeField] private float spawnRadius;
        [SerializeField] private bool spawnOnTriggerEnter = false;

        private void Awake()
        {
            if (!spawnOnTriggerEnter)
            {
                DoSpawn();
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (spawnOnTriggerEnter && other.gameObject.CompareTag("Player"))
            {
                DoSpawn();
            }
        }
        
        private void DoSpawn()
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