using Player;
using Player.State;
using ToExport.Scripts.PickUps;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ToExport.Scripts.Player
{
    [RequireComponent(typeof(InputManager))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 6f;
        
        private float mouseSensitivity = 3f;

        PlayerBaseState _currentState;
        
        public readonly PlayerIdleState idleState = new PlayerIdleState();
        public readonly PlayerJumpingState jumpingState = new PlayerJumpingState();
        public readonly PlayerMovingState movingState = new PlayerMovingState();

        private Rigidbody playerBody;
        private CapsuleCollider playerCapsule;
        private Quaternion cameraRotation;
        private Quaternion playerRotation;
        private float cameraPitch = 0f;
        private Vector2 currentDirection = Vector2.zero;

        
        [SerializeField] Camera playerCamera;
        [SerializeField] Animator playerAnimator;

        #region refactor_out
        [SerializeField] AudioSource playerAudio;
        [SerializeField] AudioSource[] footstepsAudio;
        [SerializeField] AudioClip jumpAudio;
        [SerializeField] AudioClip landAudio;
        [SerializeField] AudioClip ammoPickupAudio;
        [SerializeField] AudioClip healthPickupAudio;
        [SerializeField] AudioClip emptyGunAudio;
        [SerializeField] AudioClip reloadAudio;
        #endregion

        public bool IsMoving { get; private set; } = false;
        
        public bool RifleIsReady { get; private set; } = false;

        (int count, int max) Ammo = (400, 400);
        (int count, int max) AmmoClip = (30, 30);


        private string _currentAnimationState;

        private void OnEnable()
        {
            InputManager.OnReloadKeyDown += HandleReload;
        }

        void Start()
        {
            playerBody = GetComponent<Rigidbody>();
            playerCapsule = GetComponent<CapsuleCollider>();

            cameraRotation = playerCamera.transform.localRotation;
            playerRotation = transform.localRotation;

            TransitionToState(idleState);
        }

        void Update()
        {
            HandleMovement();
            HandleMouseLook();
            HandleShoot();
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                RifleIsReady = true;
            }

            _currentState.Update(this);
        }

        private void HandleReload()
        {
            ChangeAnimationState("Reloading", 1f);

            playerAudio.clip = reloadAudio;
            playerAudio.Play();

            int ammoNeeded = AmmoClip.max - AmmoClip.count;
            int ammoAvailable = ammoNeeded < Ammo.count ? ammoNeeded : Ammo.count;

            Ammo.count -= ammoAvailable;
            AmmoClip.count += ammoAvailable;
            
        }

        private void HandleShoot()
        {
            if (Input.GetButton("Fire1") && RifleIsReady)
            {
                if (AmmoClip.count > 0)
                {
                    ChangeAnimationState("SHOOT_SUB-GUN");
                    AmmoClip.count--;
                }
                else if (RifleIsReady)
                {
                    playerAudio.clip = emptyGunAudio;
                    playerAudio.Play();
                }
            }
            else
            {
                ChangeAnimationState("IDLE_SUB-GUN", 0.25f);
            }
        }

        public void Jump()
        {
            playerBody.AddForce(0, 300, 0);
        }

        public void HandleWalking()
        {
            if (RifleIsReady)
            {
                ChangeAnimationState("Walk With Rifle", 1f);
            }

            InvokeRepeating("PlayRandomFootstepAudio", 0, 0.45f);
        }

        public void ChangeAnimationState(string newState, float transitionTime = 0f)
        {
            if (newState == _currentAnimationState)
                return;

            playerAnimator.CrossFadeInFixedTime(newState, transitionTime);

            _currentAnimationState = newState;
        }

        private void HandleMovement()
        {
            Vector2 targetDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            targetDirection.Normalize();

            // currentDirection = Vector2.SmoothDamp(currentDirection, targetDirection, ref currentDirectionVelocity, moveSmoothTime);

            // var speedModifier = walkSpeed;

            // if (Input.GetKey(KeyCode.LeftShift))
            //     speedModifier = runSpeed;

            Vector3 movementVelocity =
                (transform.right * targetDirection.x + transform.forward * targetDirection.y) * walkSpeed;
            transform.position += (movementVelocity * Time.deltaTime);

            IsMoving = (Mathf.Abs(targetDirection.x) > 0 || Mathf.Abs(targetDirection.y) > 0);
        }


        private void PlayRandomFootstepAudio()
        {
            AudioSource audioSource = new AudioSource();
            int randomIndex = Random.Range(1, footstepsAudio.Length);

            audioSource = footstepsAudio[randomIndex];
            audioSource.Play();

            footstepsAudio[randomIndex] = footstepsAudio[0];
            footstepsAudio[0] = audioSource;
        }

        private void HandleMouseLook()
        {
            Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            cameraPitch -= mouseDelta.y * mouseSensitivity;
            cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

            playerCamera.transform.localEulerAngles = Vector3.right * cameraPitch;
            playerBody.transform.Rotate(Vector3.up * (mouseDelta.x * mouseSensitivity));
        }

        public bool IsGrounded()
        {
            return (Physics.SphereCast(transform.position, playerCapsule.radius, Vector3.down, out var hitInfo,
                (playerCapsule.height / 2f) - playerCapsule.radius + 0.1f));
        }

        public void TransitionToState(PlayerBaseState newState)
        {
            if (_currentState != newState)
            {
                _currentState = newState;
                _currentState.EnterState(this);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent<IPickup>(out var pickup))
            {
                pickup.OnPickup();
            }

            // if (other.gameObject.CompareTag(AMMO_TAG) && Ammo.count < Ammo.max)
            // {
            //     ammoPickupAudio.Play();
            //
            //     Ammo.count = Mathf.Clamp(Ammo.count + 10, 0, Ammo.max);
            //     Debug.Log($"Current ammo: {Ammo.count}");
            //     Destroy(other.gameObject);
            // }

            if (other.gameObject.CompareTag("Terrain"))
            {
                _currentState.OnCollisionEnter(this);
            }
        }

        private void OnDisable()
        {
            InputManager.OnReloadKeyDown -= HandleReload;
        }
    }
}