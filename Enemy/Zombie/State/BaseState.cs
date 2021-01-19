using UnityEngine;

namespace Enemy.Zombie.State
{
    public abstract class BaseState<T> where T : MonoBehaviour
    {
        public abstract void OnEnterState(T enemy);
        public abstract void DoState(T enemy);
        public abstract void OnExitState(T enemy);
    }
}
