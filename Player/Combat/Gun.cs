using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player.Combat
{
    public class Gun : MonoBehaviour
    {
        [Header("Prefab References")] public GameObject bulletPrefab;
        [SerializeField] private GameObject casingPrefab;
        [SerializeField] private GameObject muzzleFlashPrefab;
        [SerializeField] private TrailRenderer bulletEffect;

        [Header("Location References")] [SerializeField]
        private Transform barrelLocation;

        [SerializeField] private Transform casingExitLocation;

        [Header("Settings")] [Tooltip("Specify time to destroy the casing object")] [SerializeField]
        private float destroyTimer = 2f;

        [Tooltip("Bullet Speed")] [SerializeField]
        private float bulletSpeed = 500f;

        [Tooltip("Bullet Drop")] [SerializeField]
        private float bulletDrop = 500f;

        [Tooltip("Casing Ejection Speed")] [SerializeField]
        private float ejectPower = 150f;

        [Tooltip("How many bullets are fired per second")] [SerializeField]
        private int fireRate = 4;

        [SerializeField] private Animator gunAnimator;

        [SerializeField] private GameObject bullet;

        private ParticleSystem _muzzleFlash;
        private Transform _playerCameraTransform;
        private Vector3 _target;
        
        private static readonly int Fire = Animator.StringToHash("Fire");
        
        void Start()
        {
            if (barrelLocation == null)
                barrelLocation = transform;

            if (gunAnimator == null)
                gunAnimator = GetComponentInChildren<Animator>();

            if (muzzleFlashPrefab)
                _muzzleFlash =
                    Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation, barrelLocation)
                        .GetComponent<ParticleSystem>();

            _playerCameraTransform = PlayerCamera.Instance.transform;
        }

        void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                //Calls animation on the gun that has the relevant animation events that will fire
                gunAnimator.SetTrigger(Fire);
            }
        }


        //This function creates the bullet behavior
        void Shoot()
        {
            if (_muzzleFlash)
            {
                _muzzleFlash.Emit(1);
            }

            //cancels if there's no bullet prefab
            if (!bulletPrefab)
            {
                return;
            }

            GetBarrelDirection();
            
            Instantiate(bullet, barrelLocation.position, barrelLocation.rotation);
            
            // var tracer = Instantiate(bulletEffect, barrelLocation.position, barrelLocation.rotation);
            // tracer.AddPosition(barrelLocation.position);

            // Create a bullet and add force on it in direction of the barrel
            // Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation).GetComponent<Rigidbody>()
            //     .AddForce(barrelLocation.forward * shotPower);
        }
        
        public Vector3 GetBarrelDirection()
        {
            if (Physics.Raycast(_playerCameraTransform.position, _playerCameraTransform.forward, out var hit, 50f))
            {
                if (Vector3.Distance(_playerCameraTransform.position, hit.point) > 1f)
                {
                    _target = hit.point;
                    barrelLocation.LookAt(_target);
                    Debug.DrawLine(barrelLocation.position, _target, Color.red, 2f);
                    return _target;
                }
            }
            else
            {
                _target = _playerCameraTransform.position + (_playerCameraTransform.forward * 30);
                barrelLocation.LookAt(_target);
                Debug.DrawLine(barrelLocation.position, _target, Color.red, 2f);
                return _target;
            }
            
            return  Vector3.forward;
        }

        //This function creates a casing at the ejection slot
        void CasingRelease()
        {
            //Cancels function if ejection slot hasn't been set or there's no casing
            if (!casingExitLocation || !casingPrefab)
            {
                return;
            }

            //Create the casing
            var tempCasing =
                Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;

            //Add force on casing to push it out
            tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower),
                (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);

            //Add torque to make casing spin in random direction
            tempCasing.GetComponent<Rigidbody>()
                .AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

            //Destroy casing after X seconds
            Destroy(tempCasing, destroyTimer);
        }
    }
}