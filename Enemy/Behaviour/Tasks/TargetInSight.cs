using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Enemy.Behaviour.Tasks
{
    
    [Serializable]
    public class TargetInSight : Conditional
    {
        [SerializeField] private float fieldOfView;
        [SerializeField] private GameObject target;
        [SerializeField] private SharedTransform targetInSight;


        public override TaskStatus OnUpdate()
        {
            if (TargetWithinSight())
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        private bool TargetWithinSight()
        {
            var direction = target.transform.position - transform.position;
            var angle = Vector3.Angle(direction, transform.forward);

            if (angle < fieldOfView * 0.5)
            {
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out var hit, 10f))
                {
                    var hitObject = hit.collider.gameObject;
                    targetInSight.Value = hitObject.transform;
                    return (hitObject == target);
                }
            }
            return false;
        }
    }
}