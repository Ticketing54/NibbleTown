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
    public bool IsGrounded => _cc.isGrounded;
    public bool IsSprinting { get; private set; }
    public event Action OnJumped;

    // IMovementLock
    public void LockMovement(bool locked) => _movementLocked = locked;

    private CharacterController _cc;
    private Camera _mainCamera;
    private Vector3 _velocity;
    private Vector3 _jumpMoveDir;
    private float _jumpSpeed;
    private bool _movementLocked;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
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
        if (_movementLocked)
        {
            if (_cc.isGrounded && _velocity.y < 0f) _velocity.y = -2f;
            _velocity.y += gravity * Time.deltaTime;
            _cc.Move(_velocity * Time.deltaTime);
            return;
        }
        Move();
    }

    private void Move()
    {
        if (_cc.isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;
        _velocity.y += gravity * Time.deltaTime;

        Vector3 moveDir;
        float speed;

        if (_cc.isGrounded)
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            Vector3 camForward = _mainCamera.transform.forward;
            Vector3 camRight   = _mainCamera.transform.right;
            camForward.y = 0f; camRight.y = 0f;
            camForward.Normalize(); camRight.Normalize();

            moveDir    = camForward * input.y + camRight * input.x;
            IsSprinting = sprintAction.action.IsPressed();
            speed       = IsSprinting ? sprintSpeed : walkSpeed;

            MoveInputMagnitude = moveDir.magnitude;
            _jumpMoveDir       = moveDir;
            _jumpSpeed         = speed;

            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            moveDir            = _jumpMoveDir;
            speed              = _jumpSpeed;
            MoveInputMagnitude = moveDir.magnitude;
        }

        _cc.Move((moveDir * speed + _velocity) * Time.deltaTime);
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (!_cc.isGrounded) return;
        _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        OnJumped?.Invoke();
    }
}
