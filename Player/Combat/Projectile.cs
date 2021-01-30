using System;
using RootMotion.Dynamics;
using ToExport.Scripts.Enemy;
using UnityEngine;

namespace ToExport.Scripts.Player.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private LayerMask layersToIgnore;

        private float _speed;
        private bool _parametersSet;
        private int _damage;
        private int predictionStepsPerFrame = 6;

        private Vector3 _bulletVelocity;

        // private void Start()
        // {
        //     _bulletVelocity = transform.forward * _speed; //Gives us the direction of the bullet and the speed at which it moves
        // }

        private void Update()
        {
            if (_parametersSet)
            {
                Vector3 startPosition = transform.position;
                var stepSize = 1.0f / predictionStepsPerFrame;

                for (var step = 0f; step < 1; step += stepSize)
                {
                    //Can add drag here:-
                    _bulletVelocity += Physics.gravity * (stepSize * Time.deltaTime);

                    // Multiply by deltaTime, instead of a full second in a single frame.
                    Vector3 secondPosition = startPosition + _bulletVelocity * (stepSize * Time.deltaTime);

                    var direction = secondPosition - startPosition;
                    Ray ray = new Ray(startPosition, direction);

                    if (Physics.Raycast(ray, out var hitInfo, direction.magnitude, ~layersToIgnore))
                    {
                        //Hit something here check the collider
                        Debug.Log("hit" + hitInfo.collider.gameObject);
                        Debug.Log("hit parent" + hitInfo.collider.gameObject.transform.root);
                        
                        HandleCollision(hitInfo.collider.gameObject.transform.root);

                        TestingCode(hitInfo, direction);

                        Destroy(gameObject);
                    }

                    startPosition = secondPosition;
                }

                transform.position = startPosition;
            }
        }

        private void HandleCollision(Transform transformRoot)
        {
            var damageable = transformRoot.gameObject.GetComponentInChildren<IDamageable>();
            damageable.TakeDamage(_damage);
        }

        void TestingCode(RaycastHit hit, Vector3 direction)
        {
            float unpin = 2;
            float force = 800f;

            var rigid = hit.collider.gameObject.GetComponent<Rigidbody>();
            if (rigid) rigid.AddForceAtPosition(Vector3.forward, hit.point, ForceMode.Impulse);

            if (hit.collider.attachedRigidbody)
            {
                hit.collider.attachedRigidbody.TryGetComponent<MuscleCollisionBroadcaster>(out var broadcaster);

                if (broadcaster != null)
                {
                    broadcaster.Hit(unpin, direction * force, hit.point);
                }

                hit.collider.gameObject.TryGetComponent<Rigidbody>(out var hitLimb);
                hitLimb.AddForceAtPosition(direction * force, hit.point);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Vector3 startPosition = transform.position;
            Vector3 predictedBulletVelocity = _bulletVelocity;
            var stepSize = 0.01f;

            //Steps the bullet through time, multiple times in a single frame.
            //Step size is a fraction of a second that the calculation uses
            //Add 9.81m/s over time to that acceleration
            for (var step = 0f; step < 1; step += stepSize)
            {
                predictedBulletVelocity += Physics.gravity * stepSize;
                Vector3 secondPosition = startPosition + predictedBulletVelocity * stepSize;
                Gizmos.DrawLine(startPosition, secondPosition);
                startPosition = secondPosition;
            }
        }

        public void SetParameters(int damage, float speed)
        {
            _damage = damage;
            _speed = speed;

            _bulletVelocity = transform.forward * _speed; //Gives us the direction of the bullet and the speed at which it moves

            _parametersSet = true;
        }

        public LayerMask GetLayersToIgnore()
        {
            return ~layersToIgnore;
        }
    }
}