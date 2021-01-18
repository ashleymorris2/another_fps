using UnityEngine;

public class ZombieChasingState : BaseState<ZombieController>
{
    private float originalSpeed;
    public override void OnEnterState(ZombieController enemy)
    {
        enemy.ChangeAnimationState("RUNNING", 1f);

        var agent = enemy.NavMeshAgent;
        agent.SetDestination(enemy.Target.transform.position);
        originalSpeed = agent.speed;
        agent.speed += 4f;
    }

    public override void DoState(ZombieController enemy)
    {
        var agent = enemy.NavMeshAgent;
        var moved = enemy.TargetHasMoved();

        if (enemy.TargetHasMoved())
        {
            agent.SetDestination(enemy.Target.transform.position);
        }
        else if (agent.remainingDistance <= agent.stoppingDistance)
        {
            enemy.TransitionToState(enemy.AttackState);
        }
    }

    public override void OnExitState(ZombieController enemy)
    {
        enemy.NavMeshAgent.speed = originalSpeed;
    }
}
