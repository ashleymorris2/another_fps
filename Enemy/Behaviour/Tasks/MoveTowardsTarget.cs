using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Enemy.Zombie;
using UnityEngine;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

namespace Enemy.Behaviour.Tasks
{
    [Serializable]
    public class MoveTowardsTarget : Action
    {
        [SerializeField] float speed = 0;
        [SerializeField] private SharedTransform target;
        
        private NemesisController enemy;
        private Vector3 targetPosition;
        private float originalSpeed;

        public override void OnAwake()
        {
            enemy = gameObject.GetComponent<NemesisController>();
            originalSpeed = enemy.NavMeshAgent.speed;
        }

        public override void OnStart()
        {
            targetPosition = target.Value.position;
            enemy.ChangeAnimationState("RUNNING", 1f);
            enemy.NavMeshAgent.SetDestination(targetPosition);
            enemy.NavMeshAgent.speed = speed;
        }

        public override TaskStatus OnUpdate()
        {
            // Return a task status of success once we've reached the target
            if (enemy.NavMeshAgent.remainingDistance <= enemy.NavMeshAgent.stoppingDistance) {
                enemy.NavMeshAgent.speed = originalSpeed;
                enemy.ChangeAnimationState("IDLE", 1f);
                return TaskStatus.Success;
            }

            // We haven't reached the target yet so keep moving towards it
            targetPosition = target.Value.position;
            enemy.NavMeshAgent.SetDestination(targetPosition);

            return TaskStatus.Running;
        }
    }
}