using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieWanderState : BaseState<ZombieController>
{

    private float originalStoppingDistance = 0f;

    public override void OnEnterState(ZombieController enemy)
    {
        enemy.ChangeAnimationState("WALKING", 1f);
        enemy.NavMeshAgent.SetDestination(RandomDestination(enemy));
        originalStoppingDistance = enemy.NavMeshAgent.stoppingDistance;
        enemy.NavMeshAgent.stoppingDistance = 0;
    }

    public override void DoState(ZombieController enemy)
    {
        if (!enemy.NavMeshAgent.hasPath)
        {
            enemy.NavMeshAgent.SetDestination(RandomDestination(enemy));
        }
        
        if (enemy.DistanceToPlayer() <= 10f)
        {
            if (enemy.CanSeePlayer())
            {
                enemy.TransitionToState(enemy.ChaseState);
            }
        }
    }

    public override void OnExitState(ZombieController enemy)
    {
        enemy.NavMeshAgent.stoppingDistance = originalStoppingDistance;
    }

    private Vector3 RandomDestination(ZombieController enemy)
    {
        float newX = enemy.transform.position.x + Random.Range(-8, 8);
        float newZ = enemy.transform.position.z + Random.Range(-8, 8);
        float newY = Terrain.activeTerrain.SampleHeight(new Vector3(newX, 0, newZ));

        return new Vector3(newX, newY, newZ);
    }
}
