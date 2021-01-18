
using System;
using UnityEngine;
using UnityEngine.AI;
public class ZombieController : MonoBehaviour
{

    [SerializeField] float fieldOfView = 90f;

    [SerializeField] private GameObject target;
    public GameObject Target { get => target; }

    private NavMeshAgent navMeshAgent;
    public NavMeshAgent NavMeshAgent { get => navMeshAgent; }

    private string currentAnimationState;
    private Animator enemyAnimator;

    private BaseState<ZombieController> currentState;
    public readonly ZombieIdleState IdleState = new ZombieIdleState();
    public readonly ZombieChasingState ChaseState = new ZombieChasingState();
    public readonly ZombieAttackState AttackState = new ZombieAttackState();
    public readonly ZombieWanderState WanderState = new ZombieWanderState();
    public readonly ZombieDeadState DeadState = new ZombieDeadState();


    void Start()
    {
        enemyAnimator = this.GetComponent<Animator>();
        navMeshAgent = this.GetComponent<NavMeshAgent>();

        TransitionToState(IdleState);
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
      Vector3 direction = target.transform.position - transform.position;
      float angle = Vector3.Angle(direction, transform.forward);

      if(angle < fieldOfView * 0.5)
      {
          if(Physics.Raycast(transform.position + transform.up, direction.normalized, out var hit, 10f))
          {
              return (hit.collider.gameObject == target);
          }
      }

      return false; 
    }

    public bool TargetHasMoved()
    {
        var targetDistanceToLastPosition  = Vector3.Distance(target.transform.position, navMeshAgent.nextPosition);
        return (targetDistanceToLastPosition >= 3f);
    }
}
