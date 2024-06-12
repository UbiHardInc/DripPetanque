using UnityEngine;
using UnityEngine.VFX;
using UnityUtility.CustomAttributes;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    private const float TERMINAL_VELOCITY = 53.0f;
    private const float LOOK_INPUT_THRESHOLD = 0.01f;

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    [SerializeField] private float m_moveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    [SerializeField] private float m_sprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    [SerializeField] private float m_rotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float m_speedChangeRate = 10.0f;

    [SerializeField] private AudioClip m_landingAudioClip;
    [SerializeField] private AudioClip[] m_footstepAudioClips;
    [Range(0, 1)]
    [SerializeField] private float m_footstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    [SerializeField] private float m_jumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [SerializeField] private float m_gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    [SerializeField] private float m_jumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    [SerializeField] private float m_fallTimeout = 0.15f;

    [Tooltip("The maximum number of time to do a jump in a row.")]
    [SerializeField] private int m_maxJumps = 1;
    [SerializeField] private float m_minTimeBeforeJumpRefill = 0.5f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    [SerializeField] private bool m_grounded = true;

    [Tooltip("Useful for rough ground")]
    [SerializeField] private float m_groundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    [SerializeField] private float m_groundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    [SerializeField] private LayerMask m_groundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    [SerializeField] private GameObject m_cinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    [SerializeField] private float m_topClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    [SerializeField] private float m_bottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    [SerializeField] private float m_cameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    [SerializeField] private bool m_lockCameraPosition = false;

    [SerializeField, Label(bold: true)] private PlayerInput m_input;

    [Title("Animation")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private AnimationEventReciever m_animationEventReciever;

    [Title("Visual Effect")]
    [SerializeField] private VisualEffect m_dustTrail;
    [SerializeField] private VisualEffect m_dustJump;


    // cinemachine
    private float m_cinemachineTargetYaw;
    private float m_cinemachineTargetPitch;

    // player
    private float m_speed;
    private float m_animationBlend;
    private float m_targetRotation = 0.0f;
    private float m_rotationVelocity;
    private float m_verticalVelocity;

    private float m_timeBeforeJumpRefillTimer;

    // timeout deltatime
    private float m_jumpTimeoutDelta;
    private float m_fallTimeoutDelta;
    private int m_jumps = 0;

    // animation IDs
    private int m_animIDSpeed;
    private int m_animIDGrounded;
    private int m_animIDJump;
    private int m_animIDFreeFall;
    private int m_animIDMotionSpeed;

    private CharacterController m_controller;
    private GameObject m_mainCamera;

    private bool IsCurrentDeviceMouse =>
#if false
            m_playerInput.currentControlScheme == "KeyboardMouse";
#else
            false;
#endif



    private void Awake()
    {
        // get a reference to our main camera
        if (m_mainCamera == null)
        {
            m_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        m_cinemachineTargetYaw = m_cinemachineCameraTarget.transform.rotation.eulerAngles.y;

        m_controller = GetComponent<CharacterController>();

        AssignAnimationIDs();

        // reset our timeouts on start
        m_jumpTimeoutDelta = m_jumpTimeout;
        m_fallTimeoutDelta = m_fallTimeout;

        m_input.Init();

        m_animationEventReciever.OnLandEvent += OnLand;
        m_animationEventReciever.OnFootstepEvent += OnFootstep;
    }

    private void Update()
    {
        m_input.Update();

        JumpAndGravity();
        GroundedCheck();
        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void OnDestroy()
    {
        m_input.Dispose();

        m_animationEventReciever.OnLandEvent -= OnLand;
        m_animationEventReciever.OnFootstepEvent -= OnFootstep;
    }

    private void AssignAnimationIDs()
    {
        m_animIDSpeed = Animator.StringToHash("Speed");
        m_animIDGrounded = Animator.StringToHash("Grounded");
        m_animIDJump = Animator.StringToHash("Jump");
        m_animIDFreeFall = Animator.StringToHash("FreeFall");
        m_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - m_groundedOffset,
            transform.position.z);
        m_grounded = Physics.CheckSphere(spherePosition, m_groundedRadius, m_groundLayers,
            QueryTriggerInteraction.Ignore);

        m_animator.SetBool(m_animIDGrounded, m_grounded);
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (m_input.Look.sqrMagnitude >= LOOK_INPUT_THRESHOLD && !m_lockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            m_cinemachineTargetYaw += m_input.Look.x * deltaTimeMultiplier;
            m_cinemachineTargetPitch += m_input.Look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        m_cinemachineTargetYaw = ClampAngle(m_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        m_cinemachineTargetPitch = ClampAngle(m_cinemachineTargetPitch, m_bottomClamp, m_topClamp);

        // Cinemachine will follow this target
        m_cinemachineCameraTarget.transform.rotation = Quaternion.Euler(m_cinemachineTargetPitch + m_cameraAngleOverride,
            m_cinemachineTargetYaw, 0.0f);
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = m_input.Sprint ? m_sprintSpeed : m_moveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (m_input.Move == Vector2.zero)
        {
            targetSpeed = 0.0f;
        }

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(m_controller.velocity.x, 0.0f, m_controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = m_input.AnalogMovement ? m_input.Move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            m_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * m_speedChangeRate);

            // round speed to 3 decimal places
            m_speed = Mathf.Round(m_speed * 1000f) / 1000f;
        }
        else
        {
            m_speed = targetSpeed;
        }

        m_animationBlend = Mathf.Lerp(m_animationBlend, targetSpeed, Time.deltaTime * m_speedChangeRate);
        if (m_animationBlend < 0.01f)
        {
            m_animationBlend = 0f;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(m_input.Move.x, 0.0f, m_input.Move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (m_input.Move != Vector2.zero)
        {
            m_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                m_mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_targetRotation, ref m_rotationVelocity,
                m_rotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, m_targetRotation, 0.0f) * Vector3.forward;

        // move the player
        _ = m_controller.Move(targetDirection.normalized * (m_speed * Time.deltaTime) +
                            new Vector3(0.0f, m_verticalVelocity, 0.0f) * Time.deltaTime);


        m_animator.SetFloat(m_animIDSpeed, m_animationBlend);
        m_animator.SetFloat(m_animIDMotionSpeed, inputMagnitude);
    }

    private void JumpAndGravity()
    {
        if (m_grounded)
        {
            // reset the fall timeout timer
            m_fallTimeoutDelta = m_fallTimeout;

            if(m_timeBeforeJumpRefillTimer <= 0.0f)
            {
                m_jumps = m_maxJumps;
            }


            m_animator.SetBool(m_animIDJump, false);
            m_animator.SetBool(m_animIDFreeFall, false);

            // stop our velocity dropping infinitely when grounded
            if (m_verticalVelocity < 0.0f)
            {
                m_verticalVelocity = -2f;
            }
        }

        if (m_timeBeforeJumpRefillTimer >= 0.0f)
        {
            m_timeBeforeJumpRefillTimer -= Time.deltaTime;
        }

        if (m_jumps > 0)
        {
            // Jump
            if (m_input.Jump)
            {
                m_input.Jump = false;
                m_jumps--;
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                if (m_jumps != m_maxJumps)
                {
                    m_verticalVelocity = Mathf.Sqrt((m_jumpHeight * 2) * -2f * m_gravity);
                }
                else
                {
                    m_verticalVelocity = Mathf.Sqrt(m_jumpHeight * -2f * m_gravity);
                }

                m_timeBeforeJumpRefillTimer = m_minTimeBeforeJumpRefill;

                m_animator.SetBool(m_animIDJump, true);
                m_dustJump.SendEvent("PlayDustJump");
            }
        }
        else
        {

            // fall timeout
            if (m_fallTimeoutDelta >= 0.0f)
            {
                m_fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                m_animator.SetBool(m_animIDFreeFall, true);
            }

            // if we are not grounded, do not jump
            if (m_jumps == 0)
            {
                m_input.Jump = false;
            }
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (m_verticalVelocity < TERMINAL_VELOCITY)
        {
            m_verticalVelocity += m_gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f)
        {
            lfAngle += 360f;
        }

        if (lfAngle > 360f)
        {
            lfAngle -= 360f;
        }

        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (m_grounded)
        {
            Gizmos.color = transparentGreen;
        }
        else
        {
            Gizmos.color = transparentRed;
        }

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - m_groundedOffset, transform.position.z),
            m_groundedRadius);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (m_footstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, m_footstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(m_footstepAudioClips[index], transform.TransformPoint(m_controller.center), m_footstepAudioVolume);
            }
            m_dustTrail.SendEvent("PlayDustTrail");
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(m_landingAudioClip, transform.TransformPoint(m_controller.center), m_footstepAudioVolume);
        }
    }
}