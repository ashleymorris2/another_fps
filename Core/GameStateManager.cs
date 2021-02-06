using UnityEngine;

namespace ToExport.Scripts.Core
{
    public class GameStateManager : Singleton<GameStateManager>
    {
        public GameState CurrentGameState { get; private set; }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            CurrentGameState = GameState.InProgress;
        }

        public  void ChangeState(GameState newState)
        {
            CurrentGameState = newState;
        }
        
    }
}
