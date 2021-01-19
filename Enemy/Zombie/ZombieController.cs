using System.Diagnostics.CodeAnalysis;
using Enemy.Zombie.State;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Zombie
{
    public class ZombieController : MonoBehaviour
    {
        [SerializeField] float fieldOfView = 90f;
        [SerializeField] private GameObject target;

        private Animator enemyAnimator;
        private string currentAnimationState;
        private BaseState<ZombieController> currentState;

        private readonly ZombieIdleState idleState = new ZombieIdleState();
        public readonly ZombieChasingState chaseState = new ZombieChasingState();
        public readonly ZombieAttackState attackState = new ZombieAttackState();
        public readonly ZombieWanderState wanderState = new ZombieWanderState();
        public readonly ZombieDeadState DeadState = new ZombieDeadState();

        public GameObject Target => target;
        public NavMeshAgent NavMeshAgent { get; private set; }
        
        void Start()
        {
            enemyAnimator = GetComponent<Animator>();
            NavMeshAgent = GetComponent<NavMeshAgent>();

            TransitionToState(idleState);
        }

        void Update()
        {
            currentState.DoState(this);
        }

        public void TransitionToState(BaseState<ZombieController> newState)
        {
            if (newState == currentState)
                return;

            currentState?.OnExitState(this);

            currentState = newState;

            currentState?.OnEnterState(this);
        }

        public void ChangeAnimationState(string newState, float transitionTime = 0f)
        {
            if (newState == currentAnimationState)
                return;

            enemyAnimator.CrossFadeInFixedTime(newState, transitionTime);

            currentAnimationState = newState;
        }

        public float DistanceToPlayer()
        {
            return Vector3.Distance(target.transform.position, transform.position);
        }

        public bool CanSeePlayer()
        {
            var myTransform = transform;
            var direction = target.transform.position - myTransform.position;
            var angle = Vector3.Angle(direction, myTransform.forward);

            if (angle < fieldOfView * 0.5)
            {
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out var hit, 10f))
                {
                    return (hit.collider.gameObject == target);
                }
            }

            return false;
        }

        public bool TargetHasMoved()
        {
            var targetDistanceToLastPosition = Vector3.Distance(target.transform.position, NavMeshAgent.nextPosition);
            return (targetDistanceToLastPosition >= 3f);
        }
    }
}