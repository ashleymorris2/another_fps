using System;
using Enemy.Zombie;
using RootMotion.Dynamics;
using UnityEngine;

namespace ToExport.Scripts.Enemy.Ragdoll
{
    public class ZombieNavMeshPuppet : MonoBehaviour
    {
        public BehaviourPuppet puppet;
        public UnityEngine.AI.NavMeshAgent agent;
        private ZombieController zombie;

        private void Awake()
        {
            zombie = GetComponent<ZombieController>();
        }

        void Update()
        {
            // Keep the agent disabled while the puppet is unbalanced.
            agent.enabled = (puppet.state == BehaviourPuppet.State.Puppet);
            if (puppet.state != BehaviourPuppet.State.Puppet)
            {
                zombie.TransitionToState(zombie.idleState);
            }
            
            // // Update agent destination and Animator
            // if (agent.enabled)
            // {
            //     zombie.TransitionToState(zombie.wanderState);
            // }
            
           
     
        }
    }
}
