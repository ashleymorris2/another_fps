using UnityEngine;

namespace Enemy.Zombie.State
{
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
                    enemy.TransitionToState(enemy.chaseState);
                }
            }
        }

        public override void OnExitState(ZombieController enemy)
        {
            enemy.NavMeshAgent.stoppingDistance = originalStoppingDistance;
        }

        private Vector3 RandomDestination(ZombieController enemy)
        {
            var position = enemy.transform.position;
            var newX = position.x + Random.Range(-8, 8);
            var newZ = position.z + Random.Range(-8, 8);
            var newY = Terrain.activeTerrain.SampleHeight(new Vector3(newX, 0, newZ));

            return new Vector3(newX, newY, newZ);
        }
    }
}
