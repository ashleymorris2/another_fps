using Player;
using ToExport.Scripts.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ToExport.Scripts.Player.Combat
{
    public class Gun : MonoBehaviour
    {
        [Header("Gun Stats")] 
        [SerializeField] private int damage;
        [SerializeField] private float timeBetweenShooting;
        [Tooltip("Random spread added to each bullet")][SerializeField] private float bulletSpread;
        [Tooltip("How many seconds it takes to fully reload the weapon")][SerializeField] private float reloadTime;
        [SerializeField] private float timeBetweenShots;
        [Tooltip("How many bullets the gun has per magazine")][SerializeField] private int magazineSize;
        [Tooltip("How many bullets the gun fires per burst")][SerializeField] private int bulletsPerBurst;
        [Tooltip("The speed of the projectile once its left the barrel")][SerializeField] private float muzzleVelocity;
        [Tooltip("Can the trigger be held down to fire?")][SerializeField] private bool isFullAuto;

        [Header("Animator References")] 
        [SerializeField] private Animator gunAnimator;
        
        [Header("Location References")]
        [SerializeField] private Transform barrelLocation;
        
        [Header("Bullet Settings")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject muzzleFlashPrefab; 
        
        [Header("Casing Settings (Optional)")]
        [Tooltip("Casing Ejection Speed")][SerializeField] private float ejectPower = 150f;
        [Tooltip("Specify time to destroy the casing object")][SerializeField] private float casingDestroyTimer = 2f;
        [Tooltip("Casing prefab object")][SerializeField] private GameObject casingPrefab;
        [Tooltip("The location that the casing is to exit from")][SerializeField] private Transform casingExitLocation;

        [Header("Audio Settings")] 
        [Tooltip("Where the audio for this weapon originates from")][SerializeField] private AudioSource gunAudioSource;
        [Tooltip("The sound that the gun makes when shooting")][SerializeField] private AudioClip shootingClip;
        [Tooltip("Randomises the shooting pitch")][SerializeField][Range(0, 1f)] private float pitchRange;
        
        private int _bulletsInMagazine;
        private int _bulletsShotThisBurst;

        private bool _isShooting;
        private bool _isReadyToShoot;
        private bool _isReloading;
        
        private ParticleSystem _muzzleFlash;
        private LayerMask _layersToIgnore;

        private ProjectileFactory _projectileFactory;

        private static readonly int Fire = Animator.StringToHash("Fire");
        
        private void Awake()
        {
            _bulletsInMagazine = magazineSize;
            _isReadyToShoot = true;

            _layersToIgnore = bulletPrefab.GetComponent<Projectile>().GetLayersToIgnore();

            _projectileFactory = new ProjectileFactory(bulletPrefab, damage, muzzleVelocity);
        }

        private void Start()
        {
            if (muzzleFlashPrefab)
                _muzzleFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation, barrelLocation).GetComponent<ParticleSystem>();
            
            _muzzleFlash.Stop();
        }

        private void Update()
        {
            ShootingInput();
        }

        private void ShootingInput()
        {
            _isShooting = isFullAuto ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0);

            if (Input.GetKeyDown(KeyCode.R) && _bulletsInMagazine < magazineSize && !_isReloading)
                Reload();

            if (_isReadyToShoot && _isShooting && !_isReloading && _bulletsInMagazine > 0)
            {
                _bulletsShotThisBurst = bulletsPerBurst;
                Shoot();
            }
        }

        private void Reload()
        {
            _isReloading = true;
            Invoke(nameof(ReloadFinished), reloadTime);
        }

        private void Shoot()
        {
            _isReadyToShoot = false;
            
            gunAnimator.SetTrigger(Fire);
            
            _muzzleFlash.Emit(1);
            
            var pitch = gunAudioSource.pitch;
            gunAudioSource.clip = shootingClip;
            gunAudioSource.pitch = Random.Range(pitch - pitchRange, pitch + pitchRange);
            gunAudioSource.Play();
            
            var cameraTransform = PlayerCamera.Instance.transform;
            
            //Check if we hit something at the middle of the screen
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hit, 50f, _layersToIgnore))
            {
                if (Vector3.Distance(cameraTransform.position, hit.point) > 1f)
                {
                    barrelLocation.LookAt(hit.point);
                }
            }
            else
            {
                barrelLocation.LookAt(cameraTransform.position + (cameraTransform.forward * 80f));
            }
            
            //Spread
            var randomX = Random.Range(-bulletSpread, bulletSpread);
            var randomY = Random.Range(-bulletSpread, bulletSpread);
            
            //Do Shot
            Vector3 shotDirection = barrelLocation.eulerAngles + new Vector3(randomX, randomY, 0);
            _projectileFactory.Create(barrelLocation.position, shotDirection);
            
            CasingRelease();
            
            _bulletsInMagazine--;
            _bulletsShotThisBurst--;
            
            Invoke(nameof(ResetShot), timeBetweenShooting);

            if (_bulletsShotThisBurst > 0 && _bulletsInMagazine > 0)
            {
                Invoke(nameof(Shoot), timeBetweenShots);
            }
        }

        private void CasingRelease()
        {
            //Cancels function if ejection slot hasn't been set or there's no casing
            if (!casingExitLocation || !casingPrefab)
            {
                return;
            }

            //Create the casing
            var exitPosition = casingExitLocation.position;
            var tempCasing = Instantiate(casingPrefab, exitPosition, casingExitLocation.rotation) as GameObject;

            //Add force on casing to push it out
            tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower),
                (exitPosition - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);

            //Add torque to make casing spin in random direction
            tempCasing.GetComponent<Rigidbody>()
                .AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

            //Destroy casing after X seconds
            Destroy(tempCasing, casingDestroyTimer);
        }

        private void ResetShot()
        {
            _isReadyToShoot = true;
        }

        private void ReloadFinished()
        {
            _bulletsInMagazine = magazineSize;
            _isReloading = false;
        }
    }
}