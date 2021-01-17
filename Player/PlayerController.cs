using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    private float speed = 6f;
    private float mouseSensitivity = 3f;

    PlayerBaseState currentState;

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


    [SerializeField] AudioSource playerAudio;
    [SerializeField] AudioSource[] footstepsAudio;
    [SerializeField] AudioClip jumpAudio;
    [SerializeField] AudioClip landAudio;
    [SerializeField] AudioClip ammoPickupAudio;
    [SerializeField] AudioClip healthPickupAudio;
    [SerializeField] AudioClip emptyGunAudio;
    [SerializeField] AudioClip reloadAudio;


    public bool IsMoving { get; private set; } = false;
    public bool RifleIsReady { get; private set; } = false;


    (int count, int max) Ammo = (0, 50);

    (int count, int max) AmmoClip = (0, 10);



    private string currentAnimationState;

    private const string AMMO_TAG = "Ammo";


    void Start()
    {
        playerBody = this.GetComponent<Rigidbody>();
        playerCapsule = this.GetComponent<CapsuleCollider>();

        cameraRotation = playerCamera.transform.localRotation;
        playerRotation = this.transform.localRotation;

        TransitionToState(idleState);
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleShoot();
        HandleReload();

        if (Input.GetKeyDown(KeyCode.F))
        {
            RifleIsReady = true;
        }

        currentState.Update(this);
    }

    private void HandleReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && RifleIsReady && Ammo.count > 0)
        {
            
            ChangeAnimationState("Reloading", 1f);

            playerAudio.clip = reloadAudio;
            playerAudio.Play();

            int ammoNeeded = AmmoClip.max - AmmoClip.count;
            int ammoAvailable = ammoNeeded < Ammo.count ? ammoNeeded : Ammo.count;

            Ammo.count -= ammoAvailable;
            AmmoClip.count += ammoAvailable;

            Debug.Log($"Current ammo: {Ammo.count}");
            Debug.Log($"Current ammo in clip: {AmmoClip.count}");
        }
    }

    private void HandleShoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && RifleIsReady)
        {
            if (AmmoClip.count > 0)
            {
                ChangeAnimationState("Fire");
                AmmoClip.count--;
                Debug.Log($"Current ammo in clip: {AmmoClip.count}");
            }
            else if (RifleIsReady)
            {
                playerAudio.clip = emptyGunAudio;
                playerAudio.Play();
            }
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
        if (newState == currentAnimationState)
            return;

        playerAnimator.CrossFadeInFixedTime(newState, transitionTime);

        currentAnimationState = newState;
    }

    private void HandleMovement()
    {
        Vector2 targetDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDirection.Normalize();

        // currentDirection = Vector2.SmoothDamp(currentDirection, targetDirection, ref currentDirectionVelocity, moveSmoothTime);

        // var speedModifier = walkSpeed;

        // if (Input.GetKey(KeyCode.LeftShift))
        //     speedModifier = runSpeed;

        Vector3 movementVelocity = (transform.right * targetDirection.x + transform.forward * targetDirection.y) * speed;
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
        playerBody.transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);
    }

    public bool IsGrounded()
    {
        return (Physics.SphereCast(transform.position, playerCapsule.radius, Vector3.down, out var hitInfo, (playerCapsule.height / 2f) - playerCapsule.radius + 0.1f));
    }

    public void TransitionToState(PlayerBaseState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            currentState.EnterState(this);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<IPickupable>(out var pickup))
        {
            pickup.OnPickup();
        }

        if (other.gameObject.tag == AMMO_TAG && Ammo.count < Ammo.max)
        {
            // ammoPickupAudio.Play();

            // Ammo.count = Mathf.Clamp(Ammo.count + 10, 0, Ammo.max);
            // Debug.Log($"Current ammo: {Ammo.count}");
            // Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Terrain")
        {
            currentState.OnCollisionEnter(this);
        }
    }
}
