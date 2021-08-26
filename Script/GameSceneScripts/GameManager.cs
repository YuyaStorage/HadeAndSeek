using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    // 正しいロッカーに隠れたか
    public bool IsHadeCorrectLocker { private get; set; } = false;
    // 青色の薬品を使ったか
    public bool IsBlueMedicineUsed { get; set; } = false;
    // 青色の薬品を浴びたか
    public bool IsBlueMedicineShowered { private get; set; } = false;
    // 緑色の薬品を使ったか
    public bool IsGreenMedicineUsed { get; set; } = false;

    // 設定画面のUI
    [SerializeField]
    private GameObject _settingCanvas = null;
    // StoryCanvas
    [SerializeField]
    private GameObject _storyCanvas = null;
    // GameClear時に表示するキャンバス
    [SerializeField]
    private GameObject _gameClearCanvas = null;
    // Player
    [SerializeField]
    private GameObject _player = null;
    // Enemy
    [SerializeField]
    private GameObject _creature = null;
    // 使用しているBGM
    [SerializeField]
    private AudioSource _mainBGM = null;
    // GameEnd時にならす鐘
    [SerializeField]
    private AudioSource _gameEndBell = null;
    // 使用しているAudioMixer
    [SerializeField]
    private AudioMixer _mainAudioMixer = null;

    // Playerを操作するScript
    private PlayerController _playerController;
    // FadeIn, FadeOutを行うScript
    private FadeProcessController _fadeProcessController;
    // 全体音量
    private Slider _masterVolumeSlider;
    // BGMの音量
    private Slider _BGMVolumeSlider;
    // SEの音量
    private Slider _gameSEVolumeSlider;

    private void Awake()
    {
        InitializeGame();
    }

    private void Start()
    {
        // 暗転した状態から開始する．
        StartCoroutine(GamesStart());
    }

    private void Update()
    {
        DisplaySettingCanvas();
    }

    // エンディング分岐
    public void GameEnd()
    {
        _playerController.UnmovablePlayer();
        _mainBGM.Stop();
        if (IsHadeCorrectLocker && IsBlueMedicineShowered)
        {
            StartCoroutine(GameClear());
        }
        else
        {
            StartCoroutine(GameOver());
        }
    }

    // Gameの初期化
    private void InitializeGame()
    {
        // Playerの生成
        _player = GameObject.Instantiate(_player);
        _playerController = _player.GetComponent<PlayerController>();
        _fadeProcessController = GetComponent<FadeProcessController>();

        // StoryCanvasの生成
        _storyCanvas = GameObject.Instantiate(_storyCanvas);
        _storyCanvas.transform.SetParent(transform, false);
        _storyCanvas.SetActive(false);

        // 設定画面の生成
        _settingCanvas = GameObject.Instantiate(_settingCanvas);
        _settingCanvas.transform.SetParent(transform, false);
        _settingCanvas.SetActive(false);

        // Sliderに音量調整のイベント追加
        Transform settingPanel = _settingCanvas.transform.Find("Panel");
        _masterVolumeSlider = settingPanel.Find("MasterVolumeSlider").GetComponent<Slider>();
        _BGMVolumeSlider = settingPanel.Find("BGMVolumeSlider").GetComponent<Slider>();
        _gameSEVolumeSlider = settingPanel.Find("SEVolumeSlider").GetComponent<Slider>();
        _masterVolumeSlider.onValueChanged.AddListener(delegate { _mainAudioMixer.SetFloat("MasterVolume", _masterVolumeSlider.value); });
        _BGMVolumeSlider.onValueChanged.AddListener(delegate { _mainAudioMixer.SetFloat("BGMVolume", _BGMVolumeSlider.value); });
        _gameSEVolumeSlider.onValueChanged.AddListener(delegate { _mainAudioMixer.SetFloat("GameSEVolume", _gameSEVolumeSlider.value); });

        // ButtonにEvent追加
        _storyCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(()=> { _settingCanvas.SetActive(true); GameObject.Destroy(_storyCanvas); });
        _settingCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(_playerController.BeMovablePlayer);
        _settingCanvas.transform.Find("Panel/LoadTitleButton").GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("TitleScene"));
    }


    // Escapeで設定を開く．
    private void DisplaySettingCanvas()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _playerController.IsMovable)
        {
            _settingCanvas.SetActive(true);
            _playerController.UnmovablePlayer();
        }
    }

    // GameClearルート
    private IEnumerator GameClear()
    {
        bool isProcessEnd = false;
        _fadeProcessController.SetFadeProcess(_gameEndBell.Play, () => { }, () => { isProcessEnd = true; });
        yield return new WaitUntil(() => { return isProcessEnd; });
        isProcessEnd = false;
        _fadeProcessController.SetFadeProcess(() => { }, () => { }, () => { isProcessEnd = true; });
        yield return new WaitUntil(() => { return isProcessEnd; });
        yield return new WaitForSeconds(1f);
        Color panelColor = Color.white;
        panelColor.a = 0;
        _fadeProcessController.SetFadeProcess(() => { }, () => { GameObject.Instantiate(transform.Find("FadeProcessCanvas").gameObject); }, CreateGameClearCanvas, 0.2f, panelColor);
    }

    // GameOverルート
    private IEnumerator GameOver()
    {
        bool isProcessEnd = false;
        _fadeProcessController.SetFadeProcess(_gameEndBell.Play, () => { }, () => { isProcessEnd = true; });
        yield return new WaitUntil(() => { return isProcessEnd; });
        isProcessEnd = false;
        _fadeProcessController.SetFadeProcess(() => { }, () => CreatCreature(), () => { isProcessEnd = true; });
        yield return new WaitUntil(() => { return isProcessEnd; });
        yield return new WaitForSeconds(1f);
        Color panelColor = Color.red;
        panelColor.a = 0;
        _fadeProcessController.SetFadeProcess(() => { }, () => { GameObject.Instantiate(transform.Find("FadeProcessCanvas").gameObject); }, () => SceneManager.LoadScene("GameOverScene"), 0.2f, panelColor);

    }

    // GameStart時，暗転した状態で1秒待ちFadeInする．
    private IEnumerator GamesStart()
    {
        transform.Find("FadeProcessCanvas").gameObject.SetActive(true);
        transform.Find("FadeProcessCanvas/Panel").GetComponent<Image>().color = Color.black;
        yield return new WaitForSeconds(1f);
        _fadeProcessController.SetFadeProcess(() => { }, _mainBGM.Play, () => _storyCanvas.SetActive(true), 0.02f, Color.black);
    }

    // GameOver時に使うクリーチャーの生成
    private void CreatCreature()
    {
        _creature = GameObject.Instantiate(_creature);
        Vector3 creaturePos = Camera.main.transform.position;
        creaturePos += _player.transform.forward * 0.56f;
        creaturePos.y -= 1f;
        _creature.transform.position = creaturePos;
        _creature.transform.eulerAngles = new Vector3(0, 270 + _player.transform.eulerAngles.y, 0);
    }

    // GameClearCanvasの生成
    private void CreateGameClearCanvas()
    {
        _gameClearCanvas = GameObject.Instantiate(_gameClearCanvas);
        _gameClearCanvas.transform.SetParent(transform, false);
        _gameClearCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("GameClearScene"));
    }
}
