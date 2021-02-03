using RootMotion.Dynamics;
using ToExport.Scripts.Enemy.Zombie;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Zombie.State
{
    public class ZombieChasingState : BaseState<ZombieController>
    {
        public override void OnEnterState(ZombieController enemy)
        {
            var agent = enemy.MeshAgent;

            if (!agent.enabled) return;

            enemy.ChangeAnimationState("RUNNING", 1f);
            
            agent.speed = enemy.RunningSpeed;
            agent.SetDestination(enemy.Target.transform.position);
        }

        public override void DoState(ZombieController enemy)
        {
            if (enemy.Puppet && enemy.Puppet.state == BehaviourPuppet.State.Unpinned)
                enemy.TransitionToState(enemy.fallenState);
            
            var agent = enemy.MeshAgent;
            
            if (!agent.enabled) return;
            
            if (enemy.TargetHasMoved())
            {
                agent.SetDestination(enemy.Target.transform.position);
            }
            else if (agent.remainingDistance <= agent.stoppingDistance)
            {
                enemy.TransitionToState(enemy.attackState);
            }
        }

        public override void OnExitState(ZombieController enemy)
        {

        }
    }
}