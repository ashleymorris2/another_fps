using UnityEngine;


//Enters chase state if it can see or hear the player
//Can enter the wander state on a timer
public class ZombieIdleState : BaseState<ZombieController>
{
    public override void OnEnterState(ZombieController enemy)
    {
        enemy.ChangeAnimationState("IDLE", 2f);
    }

  
    public override void DoState(ZombieController enemy)
    {
        if(enemy.DistanceToPlayer() <= 10f)
        {
            if(enemy.CanSeePlayer())
            {
                enemy.TransitionToState(enemy.ChaseState);
            }
        }
        else{
            enemy.TransitionToState(enemy.WanderState);
        }
    }

    public override void OnExitState(ZombieController enemy)
    {
    }

}
