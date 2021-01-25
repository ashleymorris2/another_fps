using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ToExport.Scripts.Player.Combat
{
    public class Gun : MonoBehaviour
    {
        [Header("Gun Stats")] [SerializeField] private int damage;
        [SerializeField] private float timeBetweenShooting;
        [SerializeField] private float spread;
        [SerializeField] private float reloadTime;
        [SerializeField] private float timeBetweenShots;
        [SerializeField] private int magazineSize;
        [SerializeField] private int bulletsPerBurst;
        [SerializeField] private bool isFullAuto;

        [Header("Animator References")] 
        [SerializeField] private Animator gunAnimator;
        
        [Header("Location References")]
        [SerializeField] private Transform barrelLocation;
        
        [Header("Bullet Settings")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject muzzleFlashPrefab; 
        
        [Header("Casing Settings (Optional)")]
        [Tooltip("Casing Ejection Speed")] 
        [SerializeField] private float ejectPower = 150f;
        [Tooltip("Specify time to destroy the casing object")] 
        [SerializeField] private float casingDestroyTimer = 2f;
        [SerializeField] private GameObject casingPrefab;
        [SerializeField] private Transform casingExitLocation;

        private int _bulletsLeft;
        private int _bulletsShot;

        private bool _isShooting;
        private bool _isReadyToShoot;
        private bool _isReloading;
        
        private ParticleSystem _muzzleFlash;

        private static readonly int Fire = Animator.StringToHash("Fire");
        
        private void Awake()
        {
            _bulletsLeft = magazineSize;
            _isReadyToShoot = true;
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

            if (Input.GetKeyDown(KeyCode.R) && _bulletsLeft < magazineSize && !_isReloading)
                Reload();

            if (_isReadyToShoot && _isShooting && !_isReloading && _bulletsLeft > 0)
            {
                _bulletsShot = bulletsPerBurst;
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
            gunAnimator.SetTrigger(Fire);
            _muzzleFlash.Emit(1);
            
            _isReadyToShoot = false;

            //Spread
            var randomX = Random.Range(-spread, spread);
            var randomY = Random.Range(-spread, spread);
            
            //Check if we hit something at the middle of the screen
            if (Physics.Raycast(PlayerCamera.Instance.transform.position, PlayerCamera.Instance.transform.forward, out var hit, 50f))
            {
                if (Vector3.Distance(PlayerCamera.Instance.transform.position, hit.point) > 1f)
                {
                    barrelLocation.LookAt(hit.point);
                }
            }
            else
            {
                var cameraTransform = PlayerCamera.Instance.transform;
                barrelLocation.LookAt(cameraTransform.position + (cameraTransform.forward * 30));
            }
            
            //Do Shot
            Vector3 shotDirection = barrelLocation.eulerAngles + new Vector3(randomX, randomY, 0);
            Instantiate(bulletPrefab, barrelLocation.position, Quaternion.Euler(shotDirection.x, shotDirection.y, shotDirection.z));
            
            CasingRelease();
            
            _bulletsLeft--;
            _bulletsShot--;
            
            Invoke(nameof(ResetShot), timeBetweenShooting);

            if (_bulletsShot > 0 && _bulletsLeft > 0)
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
            _bulletsLeft = magazineSize;
            _isReloading = false;
        }
    }
}