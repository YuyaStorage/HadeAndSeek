using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LockerController : ResearchObject
{
    // �����̃��b�J�[���ǂ���
    [SerializeField]
    private bool _isCorrectLocker = false;

    // GameManager
    private GameManager _gameManager;
    // Fade�����p
    private FadeProcessController _fadeProcessController;
    // Player
    private GameObject _player;
    // Player����Script
    private PlayerController _playerController;
    // �B��Ă���Ƃ��ɕ\������Canvas
    private GameObject _hadeStatusCanvas;

    // Player���B��Ă����Ԃ�
    private bool _isPlayerHade = false;

    private void Start()
    {
        InitializeLocker();
    }

    private void Update()
    {
        if (_isPlayerHade)
        {
            DisplaytHadeStatusCanvas();
        }
    }

    // �e��ǂݍ��݁CButton�ɋ@�\�ǉ��C�L�����o�X�̐���
    private void InitializeLocker()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _fadeProcessController = _gameManager.GetComponent<FadeProcessController>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>();

        Buttons[0].GetComponent<Button>().onClick.AddListener(() => _fadeProcessController.SetFadeProcess(_playerController.UnmovablePlayer, HadeInLocker, () => { _isPlayerHade = true; }));
        _hadeStatusCanvas = CreatHadeStatusCanvas();
        _hadeStatusCanvas.transform.Find("Panel/CloseButton").gameObject.SetActive(false);
        _hadeStatusCanvas.SetActive(false);
    }

    // Button[0]�ɒǉ�����C�x���g�C���b�J�[�ɉB���D
    private void HadeInLocker()
    {
        Vector3 newPlayerPos = transform.position;
        newPlayerPos.x += 0.05f * Mathf.Sin(transform.localEulerAngles.y / 180f * Mathf.PI);
        newPlayerPos.z += 0.05f * Mathf.Cos(transform.localEulerAngles.y / 180f * Mathf.PI);
        _player.transform.position = newPlayerPos;
        _player.transform.rotation = transform.rotation;
        Camera.main.transform.localEulerAngles = Vector3.zero;
        Camera.main.transform.Find("FlashLight").Translate(new Vector3(0, -10, 0));

        Rigidbody rigidbody = _player.GetComponent<Rigidbody>();
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rigidbody.isKinematic = true;
        rigidbody.detectCollisions = false;


        _playerController.UnmovablePlayer();
        ResearchObjectCanvas.SetActive(false);
    }

    // �B��Ă����Ԃŕ\������L�����o�X�̐���
    private GameObject CreatHadeStatusCanvas()
    {
        string mainText = "�ǂ�����?";
        System.Tuple<string, UnityAction>[] buttonDate =
            { new System.Tuple<string, UnityAction>("������܂ŉB���", ContinueHadeStatus),
              new System.Tuple<string, UnityAction>("�O�ɏo��", GoOutside) };

        return GenerateCanvas(mainText, buttonDate);
    }

    // ������܂ŉB����I�������Ƃ��̏���
    private void ContinueHadeStatus()
    {
        if (_isCorrectLocker)
        {
            _gameManager.IsHadeCorrectLocker = true;
        }
        Destroy(_hadeStatusCanvas);
        _gameManager.GameEnd();
    }

    // �N���b�N�����m���ăL�����o�X��\��
    private void DisplaytHadeStatusCanvas()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _hadeStatusCanvas.SetActive(true);
            _isPlayerHade = false;
        }
    }

    // ���b�J�[����O�ɏo��
    private void GoOutside()
    {
        _player.transform.Translate(_player.transform.forward);
        Camera.main.transform.Find("FlashLight").Translate(new Vector3(0, 10, 0));

        Rigidbody rigidbody = _player.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.detectCollisions = true;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _playerController.BeMovablePlayer();
        _hadeStatusCanvas.SetActive(false);
    }
}
