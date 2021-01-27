using RootMotion.Dynamics;
using UnityEngine;

namespace Enemy.Zombie.State
{
    public class ZombieFallenState : BaseState<ZombieController>
    {
        public override void OnEnterState(ZombieController enemy)
        {
            Debug.Log("I've fallen");
            enemy.MeshAgent.enabled = false;
        }

        public override void DoState(ZombieController enemy)
        {
            if(!enemy.Puppet)
                return;
            
            if(enemy.Puppet.state == BehaviourPuppet.State.Puppet)
                enemy.TransitionToState(enemy.PreviousState);
        }

        public override void OnExitState(ZombieController enemy)
        {
            enemy.MeshAgent.enabled = true;
            
            Debug.Log($"I'm transitioning to {enemy.PreviousState}");
            Debug.Log("I'm standing again.");
        }
    }
}