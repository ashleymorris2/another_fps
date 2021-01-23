using UnityEngine;

namespace Player
{
    public class CameraCenterPoint : MonoBehaviour
    {
        private Camera _playerCamera;
        private Ray _ray;
        
        void Start()
        {
            _playerCamera = Camera.main;
        }

        void Update()
        {
            var cameraTransform = _playerCamera.transform;
            _ray.origin = cameraTransform.position;
            _ray.direction = cameraTransform.forward;

            Physics.Raycast(_ray, out var hitInfo);
            transform.position = hitInfo.point;
        }
    }
}
