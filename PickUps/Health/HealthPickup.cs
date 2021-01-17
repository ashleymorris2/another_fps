
using UnityEngine;

public class HealthPickup : MonoBehaviour, IPickupable
{

    [SerializeField] HealthPickupStats healthPickupStats;

    public void OnPickup()
    {
        if (!PlayerHealth.instance.IsAtMax())
        {
            PlayerHealth.instance.AddHealth(healthPickupStats.HealAmount);

            // healthPickupAudio.Play();

            // Debug.Log($"Current health: {Health.current}");
            Destroy(this.gameObject);

            Debug.Log("We have been picked up");
        }
    }
}
