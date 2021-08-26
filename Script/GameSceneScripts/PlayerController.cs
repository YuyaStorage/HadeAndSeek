using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : MonoBehaviour
{
    // 動ける状態かどうかを外から取得できるようにする．
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
    // 中心をわかりやすくするためのUI
    [SerializeField]
    private GameObject _centerPointCanvas = null;

    // 中心に使う画像
    [SerializeField]
    private Image _centerPointImage = null;

    // 通常時の中心の色
    [SerializeField]
    private Color _centerPointImageOffColor;

    // 調査対象を選択できるときの色
    [SerializeField]
    private Color _centerPointImageOnColor;

    // InputManagerに使用している名前
    [Header("NameOfInputManager")]
    // 水平方向
    [SerializeField]
    private string _horizontalName = "Horizontal";
    // 垂直方向
    [SerializeField]
    private string _verticalName = "Vertical";
    // Mouseのx軸
    [SerializeField]
    private string _rotateXName = "Mouse X";
    // Mouseのｙ軸
    [SerializeField]
    private string _rotateYName = "Mouse Y";

    // Playerの設定
    [Header("PlayerSetting")]

    // 歩き速度
    [SerializeField]
    private float _walkMaxSpeed = 3f;

    // 走り速度
    [SerializeField]
    private float _runMaxSpeed = 6f;

    // 横歩き速度
    [SerializeField]
    private float _horizontalMaxSpeed = 2f;

    // カメラの回転速度
    [SerializeField]
    private float _rotateSpeed = 6f;

    // 調査対象を判定できる距離
    [SerializeField]
    private float _maxDistanceResearchObject = 2f;

    // 抗力定数
    private readonly float _drag = 15f;

    // 動摩擦係数
    private readonly float _dynamicFrictionCoefficient = 0.6f;

    // 動ける状態か
    private bool _isMovable = true;

    // 接地状態
    private bool _isGrounded = true;

    // 走り状態
    private bool _isRuning = false;

    // LeftMouseButtonが押されたか
    private bool _isMouseButtonDown;

    // 水平方向の入力値
    private float _horizontalInputValue;

    // 垂直方向の入力値
    private float _verticalInputValue;

    // Mouseのx軸の入力値
    private float _rotateXInputValue;

    // Mouseのy軸の入力値
    private float _rotateYInputValue;

    // 歩き状態の時にPlayerに加える力
    private float _walkForce;

    // 走り状態の時にPlayerに加える力
    private float _runForce;

    // 横歩き時にPlayerに加える力
    private float _horizontalForce;

    // 摩擦力
    private float _frictionForce;

    // PlayerのRigidbody
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        // 始めは設定画面を開いているため動けない．
        UnmovablePlayer();
        // 先に力の計算をしておく．
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

    // UIを閉じたときにPlayerを動けるようにする．
    public void BeMovablePlayer()
    {
        _isMovable = true;
        _centerPointCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Playerを動けない状態にする．
    public void UnmovablePlayer()
    {
        _isMovable = false;
        _centerPointCanvas.SetActive(false);
        _rigidbody.velocity = Vector3.zero;
        _footStep.Stop();
        Cursor.lockState = CursorLockMode.None;
    }

    // Playerを動かす．
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
        // 走りと歩き，バックは歩きと同じ速度
        if (_isRuning && _verticalInputValue > 0)
        {
            _rigidbody.AddForce(transform.forward * _verticalInputValue * _runForce, ForceMode.Force);
        }
        else
        {
            _rigidbody.AddForce(transform.forward * _verticalInputValue * _walkForce, ForceMode.Force);
        }

        // 横歩き
        if (_horizontalInputValue != 0)
        {
            _rigidbody.AddForce(transform.right * _horizontalInputValue * _horizontalForce, ForceMode.Force);
        }
    }

    // 入力を受け取る．
    private void InputKey()
    {
        _horizontalInputValue = Input.GetAxis(_horizontalName);
        _verticalInputValue = Input.GetAxis(_verticalName);
        _rotateXInputValue = Input.GetAxis(_rotateXName);
        _rotateYInputValue = Input.GetAxis(_rotateYName);
        _isRuning = Input.GetKey(KeyCode.LeftShift);
        _isMouseButtonDown = Input.GetMouseButtonDown(0);
    }

    // Playerを回転させる．
    private void RotatePlayer()
    {
        // 本体をy軸中心に回転させる．
        transform.Rotate(0, _rotateXInputValue * _rotateSpeed, 0);

        // カメラをx軸中心に回転させる．
        _playerCamera.transform.Rotate(-_rotateYInputValue * _rotateSpeed, 0, 0);
        if (_playerCamera.transform.localEulerAngles.x > 60 && _playerCamera.transform.localEulerAngles.x < 320 || _playerCamera.transform.localEulerAngles.y == 180)
        {
            _playerCamera.transform.Rotate(_rotateYInputValue * _rotateSpeed, 0, 0);
        }
    }

    // 力の計算
    private void CaluculateForce()
    {
        // Playerの質量
        float playerMass = _rigidbody.mass;
        // 重力
        float gravity = Mathf.Abs(Physics.gravity.y);

        // 力 = 質量 * (動摩擦係数 * 重力 + 最大速度 * 抗力定数)
        _walkForce = playerMass * (_dynamicFrictionCoefficient * gravity + _walkMaxSpeed * _drag);
        _runForce = playerMass * (_dynamicFrictionCoefficient * gravity + _runMaxSpeed * _drag);
        _horizontalForce = playerMass * (_dynamicFrictionCoefficient * gravity + _horizontalMaxSpeed * _drag);

        // 摩擦力 = 動摩擦係数 * 質量 * 重力
        _frictionForce = _dynamicFrictionCoefficient * playerMass * Mathf.Abs(Physics.gravity.y);
    }

    // 速度に比例した力を速度と逆向きにかける．
    private void AddDrag()
    {
        Vector3 vector3 = _rigidbody.velocity;
        // y軸に力をかけない．
        vector3.y = 0;

        _rigidbody.AddForce(-1 * vector3 * _drag, ForceMode.Force);
    }

    // 摩擦を加える．速度が負にならないようにする．
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

    // 調査対象を検知する．
    private void DetectResearchObject()
    {
        // Rayの対象は"ResearchObject"のみ
        int layerMask = 1 << 3;
        // Rayが当たった対象
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
