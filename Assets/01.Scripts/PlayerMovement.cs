using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;

    [SerializeField] private float _gravity = -9.8f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpPower = 3f;
    [SerializeField] private float _desiredRotationSpeed = 0.3f;
    [SerializeField] private float _allowPlayerRotation = 0.1f;
    [SerializeField] private Transform _modelTrm;
    [SerializeField] private PlayerAnimator _animator;

    private CharacterController _characterController;
    public bool IsGround => _characterController.isGrounded;


    private float _verticalVelocity;
    private Vector3 _movementVelocity;
    private Vector2 _inputDirection;
    private Vector3 _desiredMoveDirection;
    private Camera _mainCamera;
    public bool blockRotationPlayer = false; //��ݸ���϶��� �÷��̾ ȸ������ �ʵ��� �Ѵ�.

    private void Awake()
    {
        _mainCamera = Camera.main;
        _characterController = GetComponent<CharacterController>();
        _inputReader.MovementEvent += SetMovement;
        _inputReader.JumpEvent += Jump;
    }

    private void OnDestroy()
    {
        _inputReader.MovementEvent -= SetMovement;
        _inputReader.JumpEvent -= Jump;
    }

    private void SetMovement(Vector2 value)
    {
        _inputDirection = value;
    }

    private void Jump()
    {
        if (!IsGround) return;
        _verticalVelocity += _jumpPower;
    }


    private void CalculatePlayerMovement()
    {
        var forward = _mainCamera.transform.forward;
        var right = _mainCamera.transform.right;
        forward.y = 0;
        right.y = 0;

        _desiredMoveDirection = forward.normalized * _inputDirection.y + right.normalized * _inputDirection.x;

        if (blockRotationPlayer == false && _inputDirection.sqrMagnitude > _allowPlayerRotation)
        {
            //�߻����� �ƴϰ� �����̰� �ִٸ� 
            _modelTrm.rotation = Quaternion.Slerp(
                _modelTrm.rotation,
                Quaternion.LookRotation(_desiredMoveDirection),
                _desiredRotationSpeed);
        }

        _movementVelocity = _desiredMoveDirection * (_moveSpeed * Time.fixedDeltaTime);
    }

    public void RotateToCamera(Transform t)
    {
        var forward = _mainCamera.transform.forward;
        var rot = _modelTrm.rotation;
        _desiredMoveDirection = forward;
        Quaternion lookAtRotation = Quaternion.LookRotation(_desiredMoveDirection);
        Quaternion lookAtRotationOnlyY = Quaternion.Euler(
            rot.eulerAngles.x,
            lookAtRotation.eulerAngles.y,
            rot.eulerAngles.z);
        t.rotation = Quaternion.Slerp(rot, lookAtRotationOnlyY, _desiredRotationSpeed);
    }

    private void ApplyGravity()
    {
        if (IsGround && _verticalVelocity < 0) //���� �����Ǿ��ִ� ����
        {
            _verticalVelocity = -1f;
        }
        else
        {
            _verticalVelocity += _gravity * Time.fixedDeltaTime;
        }

        _movementVelocity.y = _verticalVelocity;
    }

    private void Move()
    {
        _characterController.Move(_movementVelocity);
    }

    public void ApplyAnimation()
    {
        _animator.SetShooting(blockRotationPlayer); //��ϻ��¸� ������ �ȰŴϱ� ��ȯ
        float speed = _inputDirection.sqrMagnitude;
        _animator.SetBlendValue(speed);
        _animator.SetXY(_inputDirection);
    }

    private void FixedUpdate()
    {
        ApplyAnimation();                //�ִϸ��̼� �������ְ�
        CalculatePlayerMovement();       //�̵� ���� ������ְ�
        ApplyGravity();                  //�߷� ������ְ�
        Move();                          //���� �Ϸ�Ǿ��ٸ� ���� �̵�
    }

}