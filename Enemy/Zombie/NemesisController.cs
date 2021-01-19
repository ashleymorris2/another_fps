using Enemy.Zombie.State;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Zombie
{
    public class NemesisController : MonoBehaviour
    {
        private NavMeshAgent navMeshAgent;
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        

        private string currentAnimationState;
        private Animator enemyAnimator;
        
        void Start()
        {
            enemyAnimator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        
        public void ChangeAnimationState(string newState, float transitionTime = 0f)
        {
            if (newState == currentAnimationState)
                return;

            enemyAnimator.CrossFadeInFixedTime(newState, transitionTime);

            currentAnimationState = newState;
        }
    }
}