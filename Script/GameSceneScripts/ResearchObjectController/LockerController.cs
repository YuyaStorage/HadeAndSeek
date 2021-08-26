using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LockerController : ResearchObject
{
    // 正解のロッカーかどうか
    [SerializeField]
    private bool _isCorrectLocker = false;

    // GameManager
    private GameManager _gameManager;
    // Fade処理用
    private FadeProcessController _fadeProcessController;
    // Player
    private GameObject _player;
    // Player操作Script
    private PlayerController _playerController;
    // 隠れているときに表示するCanvas
    private GameObject _hadeStatusCanvas;

    // Playerが隠れている状態か
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

    // 各種読み込み，Buttonに機能追加，キャンバスの生成
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

    // Button[0]に追加するイベント，ロッカーに隠れる．
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

    // 隠れている状態で表示するキャンバスの生成
    private GameObject CreatHadeStatusCanvas()
    {
        string mainText = "どうする?";
        System.Tuple<string, UnityAction>[] buttonDate =
            { new System.Tuple<string, UnityAction>("鐘が鳴るまで隠れる", ContinueHadeStatus),
              new System.Tuple<string, UnityAction>("外に出る", GoOutside) };

        return GenerateCanvas(mainText, buttonDate);
    }

    // 鐘が鳴るまで隠れるを選択したときの処理
    private void ContinueHadeStatus()
    {
        if (_isCorrectLocker)
        {
            _gameManager.IsHadeCorrectLocker = true;
        }
        Destroy(_hadeStatusCanvas);
        _gameManager.GameEnd();
    }

    // クリックを検知してキャンバスを表示
    private void DisplaytHadeStatusCanvas()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _hadeStatusCanvas.SetActive(true);
            _isPlayerHade = false;
        }
    }

    // ロッカーから外に出る
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
