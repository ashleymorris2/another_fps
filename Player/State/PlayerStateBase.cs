
namespace Player.State
{
    public abstract class PlayerBaseState 
    {
        public abstract void EnterState(PlayerController player);
        public abstract void Update(PlayerController player);
        public abstract  void OnCollisionEnter(PlayerController player);
    }
}
