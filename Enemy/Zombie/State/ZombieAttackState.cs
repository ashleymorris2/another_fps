
public class ZombieAttackState : BaseState<ZombieController>
{
    public override void OnEnterState(ZombieController enemy)
    {
        enemy.ChangeAnimationState("ATTACKING", 1f);
    }

    public override void DoState(ZombieController enemy)
    {
        var adjusted = enemy.Target.transform.position;
        // adjusted.z = 0;
        adjusted.y = 0;
        enemy.transform.LookAt(adjusted);

        var agent = enemy.NavMeshAgent;
        if (enemy.TargetHasMoved())
        {
            enemy.TransitionToState(enemy.ChaseState);
        }
    }

    public override void OnExitState(ZombieController enemy)
    {
    }


}
