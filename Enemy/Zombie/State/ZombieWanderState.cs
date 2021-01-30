using RootMotion.Dynamics;
using UnityEngine;

namespace Enemy.Zombie.State
{
    public class ZombieWanderState : BaseState<ZombieController>
    {
        public override void OnEnterState(ZombieController enemy)
        {
            var agent = enemy.MeshAgent;
            agent.speed = enemy.WalkingSpeed;
            
            enemy.ChangeAnimationState("WALKING", 1f);
            enemy.MeshAgent.SetDestination(RandomDestination(enemy));
            
            agent.stoppingDistance = 0;
        }

        public override void DoState(ZombieController enemy)
        {
            if (enemy.Puppet && enemy.Puppet.state == BehaviourPuppet.State.Unpinned)
                enemy.TransitionToState(enemy.fallenState);
            
            var agent = enemy.MeshAgent;
            
            if (!agent.enabled) return;
            
            if (!agent.hasPath)
            {
                enemy.MeshAgent.SetDestination(RandomDestination(enemy));
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
            Debug.Log("Im not wandering any more");
            enemy.MeshAgent.stoppingDistance = enemy.StoppingDistance;
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
