using ToExport.Scripts.PickUps;
using UnityEngine;

public class AmmoPickup : MonoBehaviour, IPickup
{

    [SerializeField] AmmoPickupStats ammoPickupStats;
    public void OnPickup()
    {

        // ammoPickupAudio.Play();

        // Ammo.count = Mathf.Clamp(Ammo.count + 10, 0, Ammo.max);
        // Debug.Log($"Current ammo: {Ammo.count}");
        // Destroy(other.gameObject);
          Destroy(this.gameObject);
    }
}
