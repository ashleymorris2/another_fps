
using Player;
using Player.State;
using UnityEngine;

public class PlayerMovingState : PlayerBaseState
{
    public override void EnterState(PlayerController player)
    {
        player.HandleWalking();
    }

    public override void OnCollisionEnter(PlayerController player)
    {
    }

    public override void Update(PlayerController player)
    {
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGrounded())
        {
            player.Jump();
            player.TransitionToState(player.jumpingState);
        }

        if (!player.IsMoving)
        {
            player.TransitionToState(player.idleState);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            player.ChangeAnimationState("Walk With Rifle", 1f);
        }

    }
}

