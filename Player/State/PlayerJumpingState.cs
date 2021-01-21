using Player;
using Player.State;

public class PlayerJumpingState : PlayerBaseState
{
    public override void EnterState(PlayerController player)
    {
        player.CancelInvoke("PlayRandomFootstepAudio");
    }

    public override void OnCollisionEnter(PlayerController player)
    {
        if (player.IsGrounded() && !player.IsMoving)
        {
            player.TransitionToState(player.idleState);
        }
        if (player.IsGrounded() && player.IsMoving)
        {
            player.TransitionToState(player.movingState);
        }
    }

    public override void Update(PlayerController player)
    {
    }

}
