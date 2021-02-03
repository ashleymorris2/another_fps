using UnityEngine;

namespace ToExport.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "HealthPickupStats")]
    public class HealthPickupStats : ScriptableObject
    {
        [SerializeField] AudioClip healthPickUpAudio;
        [SerializeField] int healAmount;

        public int HealAmount => healAmount;
    }
}
