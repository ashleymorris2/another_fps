using System.Dynamic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using Enemy.Zombie.State;
using RootMotion.Dynamics;
using UnityEngine;

namespace ToExport.Scripts.Enemy.Zombie.State
{
    public class ZombieDeadState : BaseState<ZombieController>
    {
        public override void OnEnterState(ZombieController enemy)
        {
            Debug.Log("I'm dead now!");
            
            // enemy.Puppet.SetState(BehaviourPuppet.State.Unpinned);
            enemy.ChangeAnimationState("DIE", 2f);

            // enemy.Puppet.unpinnedMuscleWeightMlp = 0f;
            enemy.Puppet.canGetUp = false;

            enemy.MeshAgent.enabled = false;
                
            var sinkHeight = Terrain.activeTerrain.SampleHeight(enemy.transform.position) - 2;
            enemy.SinkBody(8f, sinkHeight);
           
        }

        public override void DoState(ZombieController enemy)
        {
           
        }

        public override void OnExitState(ZombieController enemy)
        {
        }

     
    }
}
