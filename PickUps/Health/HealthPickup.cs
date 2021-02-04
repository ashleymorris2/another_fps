using ToExport.Scripts.PickUps;
using ToExport.Scripts.Player;
using ToExport.Scripts.ScriptableObjects;
using UnityEngine;

public class HealthPickup : MonoBehaviour, IPickup
{

    [SerializeField] HealthPickupStats healthPickupStats;

    public void OnPickup()
    {
        // if (!HealthController.instance.IsAtMax())
        // {
        //     HealthController.instance.AddHealth(healthPickupStats.HealAmount);
        //
        //     // healthPickupAudio.Play();
        //
        //     // Debug.Log($"Current health: {Health.current}");
        //     Destroy(this.gameObject);
        //
        //     Debug.Log("We have been picked up");
        // }
    }
}
