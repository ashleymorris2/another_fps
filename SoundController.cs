using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] AudioSource shot;
    
    public void PlayShotSound()
    {
        shot.Play();
    }
}
