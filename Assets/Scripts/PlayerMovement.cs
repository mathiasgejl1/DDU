using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float walkSpeed = 5;
    [SerializeField] private float runSpeed = 10;
    [SerializeField] private float crouchSpeed = 2.5f;

    [Header("Jump and Fall")]
    [SerializeField] private float jumpForce = 7;
    [SerializeField] private float gravity = -12;
    [SerializeField] private float initialFallVelocity = -2;

    [Header("Crouching")]
    [SerializeField] private float standingHeight = 2;
    [SerializeField] private float crouchingHeight = 1;
    [SerializeField] private float crouchTransitionSpeed = 10;
    [SerializeField] private float cameraOffset = 0.5f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference crouchAction;
    [SerializeField] private InputActionReference sprintAction;

    [Header("Audio")]
    [SerializeField] private AudioSource walkAudioSource;

    private CharacterController _characterController;
    private Vector2 _moveInput;
    private bool _isCrouching;
    private float _verticalVelocity;
    private bool _isGrounded;
    private bool _isRunning;
    private float _targetHeight;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _targetHeight = standingHeight;
        if (walkAudioSource != null) walkAudioSource.Stop();
    }

    private void OnEnable()
    {
        moveAction.action.performed += StoreMovementInput;
        moveAction.action.canceled += StoreMovementInput;
        jumpAction.action.performed += Jump;
        sprintAction.action.performed += Sprint;
        sprintAction.action.canceled += Sprint;
        crouchAction.action.performed += Crouch;
    }

    private void OnDisable()
    {
        moveAction.action.performed -= StoreMovementInput;
        moveAction.action.canceled -= StoreMovementInput;
        jumpAction.action.performed -= Jump;
        sprintAction.action.performed -= Sprint;
        sprintAction.action.canceled -= Sprint;
        crouchAction.action.performed -= Crouch;
    }

    private void Update()
    {
        _isGrounded = _characterController.isGrounded;
        HandleGravity();
        HandleMovement();
        HandleCrouchTransition();
        HandleWalkingSound();
    }

    private void StoreMovementInput(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (_isGrounded)
        {
            _verticalVelocity = jumpForce;
        }
    }

    private void Crouch(InputAction.CallbackContext context)
    {
        if (_isCrouching)
        {
            if (!CanStandUp())
            {
                return;
            }
            _targetHeight = standingHeight;
        }
        else
        {
            _targetHeight = crouchingHeight;
        }
        _isCrouching = !_isCrouching;
    }

    private bool CanStandUp()
    {
        return !Physics.CapsuleCast(
            transform.position + _characterController.center,
            transform.position + (Vector3.up * _characterController.height / 2),
            _characterController.radius,
            Vector3.up);
    }

    private void Sprint(InputAction.CallbackContext context)
    {
        _isRunning = context.performed;
    }

    private void HandleGravity()
    {
        if (_isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = initialFallVelocity;
        }

        _verticalVelocity += gravity * Time.deltaTime;
    }

    private void HandleMovement()
    {
        var move = cameraTransform.TransformDirection(new Vector3(_moveInput.x, 0, _moveInput.y)).normalized;
        var currentSpeed = _isCrouching ? crouchSpeed : _isRunning ? runSpeed : walkSpeed;
        var finalMove = move * currentSpeed;
        finalMove.y = _verticalVelocity;

        var collisions = _characterController.Move(finalMove * Time.deltaTime);
        if ((collisions & CollisionFlags.Above) != 0)
        {
            _verticalVelocity = 0;
        }
    }

    private void HandleCrouchTransition()
    {
        var currentHeight = _characterController.height;
        if (Mathf.Abs(currentHeight - _targetHeight) < 0.01f)
        {
            _characterController.height = _targetHeight;
        }

        var newHeight = Mathf.Lerp(currentHeight, _targetHeight, crouchTransitionSpeed * Time.deltaTime);
        _characterController.height = newHeight;
        _characterController.center = Vector3.up * (newHeight * 0.5f);

        var cameraTargetPosition = cameraTransform.localPosition;
        cameraTargetPosition.y = _targetHeight - cameraOffset;
        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition,
            cameraTargetPosition,
            crouchTransitionSpeed * Time.deltaTime);
    }

    private void HandleWalkingSound()
    {
        bool isMoving = _moveInput.magnitude > 0.1f;
        if (_isGrounded && isMoving)
        {
            if (walkAudioSource != null && !walkAudioSource.isPlaying)
                walkAudioSource.Play();
        }
        else
        {
            if (walkAudioSource != null && walkAudioSource.isPlaying)
                walkAudioSource.Stop();
        }
    }
}