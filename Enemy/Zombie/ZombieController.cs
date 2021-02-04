using System.Collections;
using Enemy.Zombie.State;
using RootMotion.Dynamics;
using ToExport.Scripts.Enemy.Zombie.State;
using UnityEngine;
using UnityEngine.AI;

namespace ToExport.Scripts.Enemy.Zombie
{
    public class ZombieController : MonoBehaviour, IDamageable
    {
        [SerializeField] float fieldOfView = 90f;
        [SerializeField] private GameObject target;
        [SerializeField] BehaviourPuppet puppet;

        [SerializeField] private int attackDamage;
        [SerializeField] private float walkingSpeed = 1f;
        [SerializeField] private float runningSpeed = 8f;
        [SerializeField] private int maxHealth;

        private Animator _enemyAnimator;
        private BaseState<ZombieController> _currentState;
        
        public readonly ZombieIdleState idleState = new ZombieIdleState();
        public readonly ZombieChasingState chaseState = new ZombieChasingState();
        public readonly ZombieAttackState attackState = new ZombieAttackState();
        public readonly ZombieWanderState wanderState = new ZombieWanderState();
        public readonly ZombieFallenState fallenState = new ZombieFallenState();
        public readonly ZombieDeadState deadState = new ZombieDeadState();

        private int _currentHealth;

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

            _currentHealth = maxHealth;
        }

        void Update()
        {
            _currentState.DoState(this);

            if (_currentHealth == 0)
                TransitionToState(deadState);
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
            _enemyAnimator.CrossFadeInFixedTime(newState, transitionTime);
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

        public void TakeDamage(Collider hitCollider, int damageAmount)
        {
            if (hitCollider.name.ToLower() == "head")
                damageAmount *= 2;

            _currentHealth = Mathf.Clamp(_currentHealth -= damageAmount, 0, maxHealth);
        }

        public void Attack()
        {
            if (DistanceToPlayer() <= StoppingDistance)
            {
                if (target.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(null, attackDamage);
                }
            }
        }

        public void SinkBody(float sinkTime, float sinkHeight)
        {
            Invoke(nameof(RemoveColliders), sinkTime);

            StartCoroutine(DestroyGameObject(sinkHeight));
        }

        private void RemoveColliders()
        {
            var colliders = transform.root.GetComponentsInChildren<Collider>();
            foreach (var c in colliders)
            {
                Destroy(c);
            }
        }

        private IEnumerator DestroyGameObject(float terrainHeight)
        {
            while (transform.position.y > terrainHeight)
            {
                transform.Translate(0, -0.001f, 0f);

                yield return null;
            }

            if (transform.position.y < terrainHeight)
                Destroy(transform.root.gameObject);
        }
    }
}