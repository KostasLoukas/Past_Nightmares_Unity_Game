using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;
        private float tmpSprintSpeed = 0;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.4f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        private float tmpJumpHeight = 0f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        public float fallThresholdVelocity = 5f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        
        private Player player;

        //Crouching variables
        [SerializeField] private GameObject headPosition;
        public bool canStand;
        public bool crouch = false;
        private CapsuleCollider playerCollider;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        public bool isClimbingLadder;
        private Vector3 lastGrabLadderDirection;
        private float previousRotation;
        private bool previousRotationIsSet;
        [SerializeField] private AudioClip climbingLadderSound;
        private bool climbingLadderSoundIsPlaying = false;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        AudioSource playerAudioSource;
        //[SerializeField] private AudioClip jumpSound;
        [SerializeField] private AudioClip fallDamageSound;
        private PlayerPickupDrop playerPickupDrop;
        private CustomRigidBodyPush playerCustomRigidBodyPush;
        

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDDying;
        

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        public StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            player = GetComponent<Player>();
            playerCollider = gameObject.GetComponent<CapsuleCollider>();
            playerAudioSource = GetComponent<AudioSource>();
            playerPickupDrop = GetComponent<PlayerPickupDrop>();
            playerCustomRigidBodyPush = GetComponent<CustomRigidBodyPush>();
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            bool previousGrounded = Grounded;

            
            
            //To trigger the ClimbingLadder animations and sound
            if (isClimbingLadder && _speed != 0) //we are climbing the ladder
            {
                _animator.SetBool("ClimbingLadder", true);
                if (!climbingLadderSoundIsPlaying)
                {
                    climbingLadderSoundIsPlaying = true;
                    playerAudioSource.PlayOneShot(climbingLadderSound);
                }
            }
            else if (isClimbingLadder && _speed == 0) //we stop midway on the ladder
            {
                _animator.SetBool("ClimbingLadder", false);
                playerAudioSource.Stop();
                climbingLadderSoundIsPlaying = false;
            }
            else if (!isClimbingLadder) //we left the ladder
            {
                playerAudioSource.Stop();
                climbingLadderSoundIsPlaying = false;
            }
            
            

            JumpAndGravity();
            GroundedCheck();
            if (!previousGrounded && Grounded && !GameMaster.gm.playerIsDead)  //only take fall damage if we fall for more than the threshold
            {
                //Debug.Log("Do damage: " + (this._verticalVelocity < -fallThresholdVelocity));
                
                if (this._verticalVelocity < -fallThresholdVelocity)  //do fall damage proportional to the player's vertical velocity
                {
                    float damage = Mathf.Abs(this._verticalVelocity + fallThresholdVelocity);
                    Debug.Log("Fall Damage dealt: " + damage*10);
                    player.DamagePlayer(damage*10);
                    SoundManager.instance.PlaySFXSound(fallDamageSound);  //works only if player is not carrying an item
                    
                }
            }
            Move();
            if (!DialogueManager.playerIsTalking && !InspectableObject.playerIsInspecting && !playerPickupDrop.isCarryingItem && !playerCustomRigidBodyPush.isPushing) //don't let the player crouch/uncrouch if they're having a conversation or inspecting an item or carrying an item or pushing a heavy object
            {
                Crouching();
            }
            
        }

        

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed (and if is not climbing a ladder)
            float targetSpeed = !isClimbingLadder ? (_input.sprint ? SprintSpeed : MoveSpeed) : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            
            //LADDER CLIMBING FUNCTION
            if (crouch == false && !playerPickupDrop.isCarryingItem && !GameMaster.gm.playerIsDead)  //don't let the player climb a ladder if they're crouching or holding an item (or they're dead)
            {
                if (!isClimbingLadder)  //not climbing any ladder
                {
                    _animator.SetBool("ClimbingLadder", false);
                    float avoidFloorDistance = 0.1f;
                    float ladderGrabDistance = 0.35f;
                    if (Physics.Raycast(transform.position + Vector3.up * avoidFloorDistance, targetDirection, out RaycastHit raycastHit, ladderGrabDistance))
                    {
                        if (raycastHit.transform.TryGetComponent(out Ladder ladder))
                        {
                            GrabLadder(targetDirection);
                        }
                    }
                }
                else  //climbing a ladder
                {
                    _animator.Play("ClimbingLadder");  //to force the ClimbingLadder animation
                    if (!previousRotationIsSet)
                    {
                        previousRotation = transform.rotation.eulerAngles.y;
                        previousRotationIsSet = true;
                    }
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, previousRotation, transform.rotation.eulerAngles.z); //lock rotation of player (0 = lock axis)
                    _animator.SetBool("ClimbingLadder", true);
                    float avoidFloorDistance = 0.1f;
                    float ladderGrabDistance = 0.35f;
                    if (Physics.Raycast(transform.position + Vector3.up * avoidFloorDistance, lastGrabLadderDirection, out RaycastHit raycastHit, ladderGrabDistance))
                    {
                        if (!raycastHit.transform.TryGetComponent(out Ladder ladder))
                        {
                            DropLadder();
                            _verticalVelocity = 4f;
                        }
                    }
                    else
                    {
                        DropLadder();
                        _verticalVelocity = 4f;
                    }

                    if (Vector3.Dot(targetDirection, lastGrabLadderDirection) < 0) //we are climbing down the ladder
                    {
                        float ladderFloorDropDistance = 0.3f;
                        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit floorRaycastHit, ladderFloorDropDistance))
                        {
                            DropLadder();
                            previousRotationIsSet = false;
                        }
                    }
                }
                if (isClimbingLadder)
                {
                    targetDirection.x = 0f;  //optional
                    targetDirection.y = targetDirection.z;
                    targetDirection.z = 0f;
                    _verticalVelocity = 0f;
                    Grounded = true;
                    _speed = targetSpeed;
                }
            }

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void GrabLadder(Vector3 lastGrabLadderDirection)
        {
            isClimbingLadder = true;
            this.lastGrabLadderDirection = lastGrabLadderDirection;
        }

        private void DropLadder()
        {
            isClimbingLadder = false;
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                    //SoundManager.instance.PlaySFXSound(jumpSound);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }
            
            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        private void Crouching()
        {
            if (Physics.Raycast(headPosition.transform.position, Vector3.up, 0.8f))
            {
                canStand = false;
                Debug.DrawRay(headPosition.transform.position, Vector3.up, Color.green);
            }
            else
            {
                canStand = true;
            }

            if(Input.GetKeyDown(KeyCode.LeftControl) && !isClimbingLadder)
            {
                if (crouch == true  && canStand == true)
                {
                    crouch = false;  //we are standing
                    _animator.SetBool("Crouch", false);

                    //so that the player can jump when not crouched
                    JumpHeight = tmpJumpHeight;
                    tmpJumpHeight = 0f;

                    //so that the player can sprint when not crouched
                    SprintSpeed = tmpSprintSpeed;
                    tmpJumpHeight = 0f;

                    //so that the player is not into glitched through the floor when not crouched
                    _controller.height = 1.9f;
                    _controller.center = new Vector3 (0f, 0.925f, 0f);
                    
                    //so that the player's collider follows his structure when not crouching
                    playerCollider.height = 1.9f;
                    playerCollider.center = new Vector3 (0f, 0.85f, 0f);
                }
                else if (crouch == true && canStand == false)
                {
                    //Nothing should be done in this case!
                }
                else
                {
                    crouch = true;  //we are crouching
                    _animator.SetBool("Crouch", true);

                    //so that the player can't jump when crouched
                    tmpJumpHeight = JumpHeight;
                    JumpHeight = 0f;
                    
                    //so that the player can't sprint when crouched
                    tmpSprintSpeed = SprintSpeed;
                    SprintSpeed = 0f;

                    //so that the player is not into glitched through the floor when crouched
                    _controller.height = 1.5f;
                    _controller.center = new Vector3 (0f, 0.65f, 0f);

                    //so that the player's collider follows his structure when crouching
                    playerCollider.height = 1.5f;
                    playerCollider.center = new Vector3 (0f, 0.65f, 0f);
                }
            }

            if (GameMaster.gm.playerIsDead && crouch == true)  //if the player died while crouched, uncrouch him
            {
                crouch = false;  //we are standing
                _animator.SetBool("Crouch", false);

                //so that the player can jump when not crouched
                JumpHeight = tmpJumpHeight;
                tmpJumpHeight = 0f;

                //so that the player can sprint when not crouched
                SprintSpeed = tmpSprintSpeed;
                tmpJumpHeight = 0f;

                //so that the player is not into glitched through the floor when not crouched
                _controller.height = 1.9f;
                _controller.center = new Vector3 (0f, 0.925f, 0f);
                    
                //so that the player's collider follows his structure when not crouching
                playerCollider.height = 1.9f;
                playerCollider.center = new Vector3 (0f, 0.85f, 0f);
            }

        }

        

    }
}