using UnityEngine;

namespace Player.Combat
{
    public class RaytracedBullet : MonoBehaviour
    {
        [SerializeField] LayerMask layerMask;

        [Tooltip("The speed at which the bullet travels in m/s")] [SerializeField]
        float bulletVelocity = 90;

        [Tooltip("How long the bullet lives for in seconds before being destroyed")] [SerializeField]
        float lifeTime = 2;
        

        private float aliveTime;
        private Vector3 _initialPosition;
        private Vector3 _initialVelocity;
        private float bulletDrop = 0.0f;
        [SerializeField] private TrailRenderer _tracer;

        private Transform _playerCameraTransform;
        private Vector3 _target;
        private TrailRenderer tracer;

        void Start()
        {
            _playerCameraTransform = PlayerCamera.Instance.transform;

            var position = transform.position;
            _initialPosition = position;
            
            tracer = Instantiate(_tracer, position , Quaternion.identity);
            tracer.AddPosition(position);
           
            _initialVelocity = (Vector3.forward - position).normalized * bulletVelocity;
        }

        void FixedUpdate()
        {
            CheckForCollision();
            Move();
            Expire();
        }

        private void CheckForCollision()
        {
            // Vector3 movementThisUpdate = bulletModel.position - previousPosition;
            // float movementSqrMagnitude = movementThisUpdate.sqrMagnitude;
            //
            // if (movementSqrMagnitude > sqrMinimumMoveExtent)
            // {
            //     var movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
            //
            //     //Use a raycast to see what is infront of the bullet that isn't on this layermask
            //     if (Physics.Raycast(previousPosition, movementThisUpdate, out var hitInfo, movementMagnitude,
            //         ~layerMask))
            //     {
            //         if (!hitInfo.collider)
            //             return;
            //
            //         if (hitInfo.collider.isTrigger)
            //             hitInfo.collider.SendMessage("OnTriggerEnter",
            //                 bulletCollider); //Collider is a trigger let them handle OnTriggerEnter
            //
            //         if (!hitInfo.collider.isTrigger)
            //         {
            //             OnTriggerEnter(hitInfo
            //                 .collider); //Collider doesn't have a trigger call our own OnTriggerEnter..
            //             bulletModel.position =
            //                 hitInfo.point - (movementThisUpdate / movementMagnitude) * partialMoveExtent;
            //         }
            //     }
            // }
            //
            // previousPosition = bulletModel.position;
        }

        private void Move()
        {
            Vector3 currentPosition = GetBulletPosition();
            aliveTime += Time.deltaTime;
            Vector3 nextPosition = GetBulletPosition();
            
            RaycastSegment(currentPosition, nextPosition);

            transform.position = nextPosition;
        }

        private void Expire()
        {
            lifeTime -= Time.deltaTime;

            if (lifeTime <= 0)
            {
                Destroy(this);
                Debug.Log("Expired");
            }
        }

        private Vector3 GetBulletPosition()
        {
            var gravity = Vector3.down * bulletDrop;
            return (_initialPosition) + (_initialVelocity * aliveTime) + (0.5f * gravity * aliveTime * aliveTime);
        }
        
        //Raycasts the bullet between the currentPosition and the nextPosition
        private void RaycastSegment(Vector3 currentPosition, Vector3 nextPosition)
        {
            var distance = (nextPosition - currentPosition).magnitude;
            if (Physics.Raycast(currentPosition, nextPosition - currentPosition, out var hit, distance))
            {
                if (Vector3.Distance(_playerCameraTransform.position, hit.point) > 1f)
                {
                    _target = hit.point;
                
                    // barrelLocation.LookAt(_target);
                    // Debug.DrawLine(barrelLocation.position, _target, Color.red, 2f);
                }
            }
            else
            {
                _target = _playerCameraTransform.position + (_playerCameraTransform.forward * 30);
                

                // barrelLocation.LookAt(_target);
                // Debug.DrawLine(barrelLocation.position, _target, Color.red, 2f);
            }
        }
    }
}