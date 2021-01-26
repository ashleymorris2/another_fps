using System;
using RootMotion.Dynamics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Player.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 420f; //Set by the gun - speed in m/s

        private int predictionStepsPerFrame = 6;
        private Vector3 _bulletVelocity;

        private void Start()
        {
            _bulletVelocity =
                transform.forward * speed; //Gives us the direction of the bullet and the speed at which it moves
        }

        private void Update()
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
                if (Physics.Raycast(ray, out var hitInfo, direction.magnitude))
                {
                    //Hit something here check the collider
                    Debug.Log("hit" + hitInfo.collider.gameObject);

                    TestingCode(hitInfo, direction);

                    Destroy(gameObject);
                }

                startPosition = secondPosition;
            }

            transform.position = startPosition;
        }

        void TestingCode(RaycastHit hit, Vector3 direction)
        {
            float unpin = 5;
            float force = 1800f;

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
                hitLimb.AddForceAtPosition( direction * force, hit.point);
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
    }
}