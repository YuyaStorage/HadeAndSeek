using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : MonoBehaviour
{
    // �������Ԃ��ǂ������O����擾�ł���悤�ɂ���D
    public bool IsMovable
    {
        get { return _isMovable; }
        private set { _isMovable = value; }
    }

    // MainCamera
    [SerializeField]
    private Camera _playerCamera = null;
    [SerializeField]
    private AudioSource _footStep = null;

    [Header("CentorPointSetting")]
    // ���S���킩��₷�����邽�߂�UI
    [SerializeField]
    private GameObject _centerPointCanvas = null;

    // ���S�Ɏg���摜
    [SerializeField]
    private Image _centerPointImage = null;

    // �ʏ펞�̒��S�̐F
    [SerializeField]
    private Color _centerPointImageOffColor;

    // �����Ώۂ�I���ł���Ƃ��̐F
    [SerializeField]
    private Color _centerPointImageOnColor;

    // InputManager�Ɏg�p���Ă��閼�O
    [Header("NameOfInputManager")]
    // ��������
    [SerializeField]
    private string _horizontalName = "Horizontal";
    // ��������
    [SerializeField]
    private string _verticalName = "Vertical";
    // Mouse��x��
    [SerializeField]
    private string _rotateXName = "Mouse X";
    // Mouse�̂���
    [SerializeField]
    private string _rotateYName = "Mouse Y";

    // Player�̐ݒ�
    [Header("PlayerSetting")]

    // �������x
    [SerializeField]
    private float _walkMaxSpeed = 3f;

    // ���葬�x
    [SerializeField]
    private float _runMaxSpeed = 6f;

    // ���������x
    [SerializeField]
    private float _horizontalMaxSpeed = 2f;

    // �J�����̉�]���x
    [SerializeField]
    private float _rotateSpeed = 6f;

    // �����Ώۂ𔻒�ł��鋗��
    [SerializeField]
    private float _maxDistanceResearchObject = 2f;

    // �R�͒萔
    private readonly float _drag = 15f;

    // �����C�W��
    private readonly float _dynamicFrictionCoefficient = 0.6f;

    // �������Ԃ�
    private bool _isMovable = true;

    // �ڒn���
    private bool _isGrounded = true;

    // ������
    private bool _isRuning = false;

    // LeftMouseButton�������ꂽ��
    private bool _isMouseButtonDown;

    // ���������̓��͒l
    private float _horizontalInputValue;

    // ���������̓��͒l
    private float _verticalInputValue;

    // Mouse��x���̓��͒l
    private float _rotateXInputValue;

    // Mouse��y���̓��͒l
    private float _rotateYInputValue;

    // ������Ԃ̎���Player�ɉ������
    private float _walkForce;

    // �����Ԃ̎���Player�ɉ������
    private float _runForce;

    // ����������Player�ɉ������
    private float _horizontalForce;

    // ���C��
    private float _frictionForce;

    // Player��Rigidbody
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        // �n�߂͐ݒ��ʂ��J���Ă��邽�ߓ����Ȃ��D
        UnmovablePlayer();
        // ��ɗ͂̌v�Z�����Ă����D
        CaluculateForce();
    }

    private void Update()
    {
        if (_isMovable)
        {
            InputKey();
            RotatePlayer();
            DetectResearchObject();
        }
    }

    private void FixedUpdate()
    {
        if (_isMovable)
        {
            MovePlayer();
        }
    }

    // UI������Ƃ���Player�𓮂���悤�ɂ���D
    public void BeMovablePlayer()
    {
        _isMovable = true;
        _centerPointCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Player�𓮂��Ȃ���Ԃɂ���D
    public void UnmovablePlayer()
    {
        _isMovable = false;
        _centerPointCanvas.SetActive(false);
        _rigidbody.velocity = Vector3.zero;
        _footStep.Stop();
        Cursor.lockState = CursorLockMode.None;
    }

    // Player�𓮂����D
    private void MovePlayer()
    {
        AddFriction();
        AddDrag();


        if(_horizontalInputValue != 0 || _verticalInputValue != 0)
        {
            if (!_footStep.isPlaying)
            {
                _footStep.Play();
            }
        }
        else
        {
            _footStep.Stop();
        }
        // ����ƕ����C�o�b�N�͕����Ɠ������x
        if (_isRuning && _verticalInputValue > 0)
        {
            _rigidbody.AddForce(transform.forward * _verticalInputValue * _runForce, ForceMode.Force);
        }
        else
        {
            _rigidbody.AddForce(transform.forward * _verticalInputValue * _walkForce, ForceMode.Force);
        }

        // ������
        if (_horizontalInputValue != 0)
        {
            _rigidbody.AddForce(transform.right * _horizontalInputValue * _horizontalForce, ForceMode.Force);
        }
    }

    // ���͂��󂯎��D
    private void InputKey()
    {
        _horizontalInputValue = Input.GetAxis(_horizontalName);
        _verticalInputValue = Input.GetAxis(_verticalName);
        _rotateXInputValue = Input.GetAxis(_rotateXName);
        _rotateYInputValue = Input.GetAxis(_rotateYName);
        _isRuning = Input.GetKey(KeyCode.LeftShift);
        _isMouseButtonDown = Input.GetMouseButtonDown(0);
    }

    // Player����]������D
    private void RotatePlayer()
    {
        // �{�̂�y�����S�ɉ�]������D
        transform.Rotate(0, _rotateXInputValue * _rotateSpeed, 0);

        // �J������x�����S�ɉ�]������D
        _playerCamera.transform.Rotate(-_rotateYInputValue * _rotateSpeed, 0, 0);
        if (_playerCamera.transform.localEulerAngles.x > 60 && _playerCamera.transform.localEulerAngles.x < 320 || _playerCamera.transform.localEulerAngles.y == 180)
        {
            _playerCamera.transform.Rotate(_rotateYInputValue * _rotateSpeed, 0, 0);
        }
    }

    // �͂̌v�Z
    private void CaluculateForce()
    {
        // Player�̎���
        float playerMass = _rigidbody.mass;
        // �d��
        float gravity = Mathf.Abs(Physics.gravity.y);

        // �� = ���� * (�����C�W�� * �d�� + �ő呬�x * �R�͒萔)
        _walkForce = playerMass * (_dynamicFrictionCoefficient * gravity + _walkMaxSpeed * _drag);
        _runForce = playerMass * (_dynamicFrictionCoefficient * gravity + _runMaxSpeed * _drag);
        _horizontalForce = playerMass * (_dynamicFrictionCoefficient * gravity + _horizontalMaxSpeed * _drag);

        // ���C�� = �����C�W�� * ���� * �d��
        _frictionForce = _dynamicFrictionCoefficient * playerMass * Mathf.Abs(Physics.gravity.y);
    }

    // ���x�ɔ�Ⴕ���͂𑬓x�Ƌt�����ɂ�����D
    private void AddDrag()
    {
        Vector3 vector3 = _rigidbody.velocity;
        // y���ɗ͂������Ȃ��D
        vector3.y = 0;

        _rigidbody.AddForce(-1 * vector3 * _drag, ForceMode.Force);
    }

    // ���C��������D���x�����ɂȂ�Ȃ��悤�ɂ���D
    private void AddFriction()
    {
        if (_isGrounded)
        {
            if (_rigidbody.velocity.magnitude - (_frictionForce / _rigidbody.mass * Time.fixedDeltaTime) > 0)
            {
                _rigidbody.AddForce(-1 * _rigidbody.velocity.normalized * _frictionForce);
            }
            else
            {
                _rigidbody.velocity = Vector3.zero;
            }
        }
    }

    // �����Ώۂ����m����D
    private void DetectResearchObject()
    {
        // Ray�̑Ώۂ�"ResearchObject"�̂�
        int layerMask = 1 << 3;
        // Ray�����������Ώ�
        RaycastHit hit;
        if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, _maxDistanceResearchObject, layerMask, QueryTriggerInteraction.Collide))
        {
            _centerPointImage.color = _centerPointImageOnColor;
            if (_isMouseButtonDown && hit.transform.Find("ResearchObjectCanvas") != null)
            {
                hit.transform.Find("ResearchObjectCanvas").gameObject.SetActive(true);
                hit.transform.GetComponent<ResearchObject>().IsChecked = true;
                UnmovablePlayer();
            }
            else if (_isMouseButtonDown)
            {
                Debug.Log("Error");
            }
        }
        else
        {
            _centerPointImage.color = _centerPointImageOffColor;
        }
    }
}
