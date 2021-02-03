using UnityEngine;

namespace ToExport.Scripts.Enemy
{
    public interface IDamageable
    {
        void TakeDamage(Collider hitCollider, int damageAmount);
    }
}