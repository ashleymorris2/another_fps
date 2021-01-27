using Player;
using Player.State;
using ToExport.Scripts.Player;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public override void EnterState(PlayerController player)
    {
        player.CancelInvoke("PlayRandomFootstepAudio");
        if(player.RifleIsReady)
        {
            player.ChangeAnimationState("IDLE_SUB-GUN", 0.2f);
        }
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

        if(player.IsMoving)
        {
            player.TransitionToState(player.movingState);
        }
        
    }
}
