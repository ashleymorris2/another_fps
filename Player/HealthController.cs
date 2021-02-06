using System;
using System.Collections;
using UnityEngine;

namespace ToExport.Scripts.Player
{
    public class HealthController : MonoBehaviour
    {
        [SerializeField] private int maxHealth;

        private int _currentHealth;
        public int CurrentHealth
        {
            get => _currentHealth;
            private set
            {
                _currentHealth = value;
                if (_currentHealth <= 0)
                    OnDeath?.Invoke();
            }
        }

        private event Action OnDeath;
        
        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void Init(Action onDeath)
        {
            OnDeath = onDeath;
        }
        
        public bool IsAtMax() => CurrentHealth == maxHealth;
    
        public void AddHealth(int healAmount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + healAmount, 0, maxHealth);
        }

        public void RemoveHealth(int damageAmount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth - damageAmount, 0, maxHealth);
        }

    }
}
