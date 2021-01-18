using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDeadState : BaseState<ZombieController>
{
    public override void OnEnterState(ZombieController enemy)
    {
        enemy.ChangeAnimationState("DIE", 2f);
    }

    public override void DoState(ZombieController enemy)
    {
    }

    public override void OnExitState(ZombieController enemy)
    {
    }
}
