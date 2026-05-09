using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IMovementLock, IPlayerState
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference sprintAction;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -20f;

    // IPlayerState
    public float MoveInputMagnitude { get; private set; }
    public bool IsGrounded => cc.isGrounded;
    public bool IsSprinting { get; private set; }
    public event Action OnJumped;

    // IMovementLock
    public void LockMovement(bool _locked) => movementLocked = _locked;

    private CharacterController cc;
    private Camera mainCamera;
    private Vector3 velocity;
    private Vector3 jumpMoveDir;
    private float jumpSpeed;
    private bool movementLocked;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        sprintAction.action.Enable();
        jumpAction.action.Enable();
        jumpAction.action.performed += OnJump;
    }

    private void OnDisable()
    {
        jumpAction.action.performed -= OnJump;
        moveAction.action.Disable();
        sprintAction.action.Disable();
        jumpAction.action.Disable();
    }

    private void Update()
    {
        if (movementLocked)
        {
            if (cc.isGrounded && velocity.y < 0f) velocity.y = -2f;
            velocity.y += gravity * Time.deltaTime;
            cc.Move(velocity * Time.deltaTime);
            return;
        }
        Move();
    }

    private void Move()
    {
        if (cc.isGrounded && velocity.y < 0f)
            velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;

        Vector3 moveDir;
        float speed;

        if (cc.isGrounded)
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight   = mainCamera.transform.right;
            camForward.y = 0f; camRight.y = 0f;
            camForward.Normalize(); camRight.Normalize();

            moveDir    = camForward * input.y + camRight * input.x;
            IsSprinting = sprintAction.action.IsPressed();
            speed       = IsSprinting ? sprintSpeed : walkSpeed;

            MoveInputMagnitude = moveDir.magnitude;
            jumpMoveDir        = moveDir;
            jumpSpeed          = speed;

            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            moveDir            = jumpMoveDir;
            speed              = jumpSpeed;
            MoveInputMagnitude = moveDir.magnitude;
        }

        cc.Move((moveDir * speed + velocity) * Time.deltaTime);
    }

    private void OnJump(InputAction.CallbackContext _ctx)
    {
        if (!cc.isGrounded) return;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        OnJumped?.Invoke();
    }
}
