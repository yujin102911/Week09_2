using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Fields
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 14f;
    [SerializeField] private float airControl = 0.5f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.6f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBuffer = 0.12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask groundMask = ~0;

    private CharacterController cc;
    private Camera cam;
    private Vector3 velocity;
    private Vector3 planarVelocity;
    private bool isGrounded;
    private float lastGroundedTime;
    private float lastJumpPressedTime;
    #endregion

    #region lifecycle
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        cam = Camera.main;
    }
    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        jumpAction.action.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        jumpAction.action.performed -= OnJumpPerformed;
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    private void Update()
    {
        // --- Ground check ---
        bool groundedNow = Physics.CheckSphere(
            groundCheck ? groundCheck.position : transform.position + Vector3.down * (cc.height * 0.5f - cc.radius + 0.05f),
            groundRadius, groundMask, QueryTriggerInteraction.Ignore);

        if (groundedNow)
            lastGroundedTime = Time.time;

        isGrounded = groundedNow;

        // --- Read input ---
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        // 플레이어 자신의 회전 기준으로 이동
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
        inputDir = transform.TransformDirection(inputDir); // ← 핵심
        inputDir.y = 0f;
        inputDir.Normalize();

        // --- Planar movement with accel/decel ---
        float targetSpeed = moveSpeed * inputDir.magnitude;
        Vector3 targetPlanarVel = inputDir * targetSpeed;

        float lerpRate = (isGrounded ? (inputDir.sqrMagnitude > 0.001f ? acceleration : deceleration)
                                     : Mathf.Lerp(deceleration, acceleration, airControl));
        planarVelocity = Vector3.MoveTowards(planarVelocity, targetPlanarVel, lerpRate * Time.deltaTime);

        // --- Jump / gravity ---
        // Coyote & buffer 처리
        bool canCoyote = (Time.time - lastGroundedTime) <= coyoteTime;
        bool hasBufferedJump = (Time.time - lastJumpPressedTime) <= jumpBuffer;

        if ((isGrounded || canCoyote) && hasBufferedJump)
        {
            // v = sqrt(2gh) (g는 음수라 -gravity)
            velocity.y = Mathf.Sqrt(2f * -gravity * jumpHeight);
            lastJumpPressedTime = -999f; // 소모
        }

        // 중력
        velocity.y += gravity * Time.deltaTime;

        // 지상에 박히지 않게 클램프
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        // --- Move ---
        Vector3 motion = planarVelocity * Time.deltaTime + Vector3.up * velocity.y * Time.deltaTime;
        cc.Move(motion);

        //// --- Face move direction (선택)
        //Vector3 faceDir = new Vector3(planarVelocity.x, 0f, planarVelocity.z);
        //if (faceDir.sqrMagnitude > 0.001f)
        //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(faceDir), 12f * Time.deltaTime);
    }
    #endregion

    #region Methods
    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        lastJumpPressedTime = Time.time;
    }

    // 디버그용 기즈모
    private void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
    #endregion




}
