using System.Diagnostics.CodeAnalysis;
using BehaviorDesigner.Runtime.Tasks;
using Enemy.Zombie.State;
using RootMotion.Dynamics;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Zombie
{
    public class ZombieController : MonoBehaviour
    {
        [SerializeField] float fieldOfView = 90f;
        [SerializeField] private GameObject target;
        [SerializeField] BehaviourPuppet puppet;

        [SerializeField] private float walkingSpeed = 1f;
        [SerializeField] private float runningSpeed = 8f;
        
        private string _currentAnimationState;
        private Animator _enemyAnimator;
        private BaseState<ZombieController> _currentState;

        public readonly ZombieIdleState idleState = new ZombieIdleState();
        public readonly ZombieChasingState chaseState = new ZombieChasingState();
        public readonly ZombieAttackState attackState = new ZombieAttackState();
        public readonly ZombieWanderState wanderState = new ZombieWanderState();
        public readonly ZombieFallenState fallenState = new ZombieFallenState();
        public readonly ZombieDeadState deadState = new ZombieDeadState();
        
        public GameObject Target => target;
        public BehaviourPuppet Puppet => puppet;
        public float WalkingSpeed => walkingSpeed;
        public float RunningSpeed => runningSpeed;
        
        public NavMeshAgent MeshAgent { get; private set; }
        public BaseState<ZombieController> PreviousState { get; private set; }
        public float StoppingDistance { get; private set; }
        
        void Start()
        {
            _enemyAnimator = GetComponent<Animator>();

            MeshAgent = GetComponent<NavMeshAgent>();

            TransitionToState(idleState);

            if (target == null)
            {
                target = GameObject.FindWithTag("Player");
            }
            
            StoppingDistance = MeshAgent.stoppingDistance;
        }

        void Update()
        {
            // if (puppet)
            //     MeshAgent.enabled = puppet.state == BehaviourPuppet.State.Puppet;
            
            _currentState.DoState(this);
        }

        public void TransitionToState(BaseState<ZombieController> newState)
        {
            if (newState == _currentState)
                return;

            _currentState?.OnExitState(this);

            PreviousState = _currentState;
            
            _currentState = newState;

            _currentState?.OnEnterState(this);
        }

        public void ChangeAnimationState(string newState, float transitionTime = 0f)
        {
            // if (newState == _currentAnimationState)
            //     return;

            _enemyAnimator.CrossFadeInFixedTime(newState, transitionTime);

            _currentAnimationState = newState;
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
            var targetDistanceToLastPosition = Vector3.Distance(target.transform.position, MeshAgent.nextPosition);
            return (targetDistanceToLastPosition >= 3f);
        }
    }
}