using UnityEngine;

namespace ToExport.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "AmmoPickupStats")]
    public class AmmoPickupStats : ScriptableObject
    {
        [SerializeField] AudioClip ammoPickUpAudio;
        [SerializeField] int ammoAmount;

        public int AmmoAmount => ammoAmount;
    }
}
