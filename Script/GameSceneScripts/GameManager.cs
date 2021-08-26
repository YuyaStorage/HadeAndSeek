using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    // ���������b�J�[�ɉB�ꂽ��
    public bool IsHadeCorrectLocker { private get; set; } = false;
    // �F�̖�i���g������
    public bool IsBlueMedicineUsed { get; set; } = false;
    // �F�̖�i�𗁂т���
    public bool IsBlueMedicineShowered { private get; set; } = false;
    // �ΐF�̖�i���g������
    public bool IsGreenMedicineUsed { get; set; } = false;

    // �ݒ��ʂ�UI
    [SerializeField]
    private GameObject _settingCanvas = null;
    // StoryCanvas
    [SerializeField]
    private GameObject _storyCanvas = null;
    // GameClear���ɕ\������L�����o�X
    [SerializeField]
    private GameObject _gameClearCanvas = null;
    // Player
    [SerializeField]
    private GameObject _player = null;
    // Enemy
    [SerializeField]
    private GameObject _creature = null;
    // �g�p���Ă���BGM
    [SerializeField]
    private AudioSource _mainBGM = null;
    // GameEnd���ɂȂ炷��
    [SerializeField]
    private AudioSource _gameEndBell = null;
    // �g�p���Ă���AudioMixer
    [SerializeField]
    private AudioMixer _mainAudioMixer = null;

    // Player�𑀍삷��Script
    private PlayerController _playerController;
    // FadeIn, FadeOut���s��Script
    private FadeProcessController _fadeProcessController;
    // �S�̉���
    private Slider _masterVolumeSlider;
    // BGM�̉���
    private Slider _BGMVolumeSlider;
    // SE�̉���
    private Slider _gameSEVolumeSlider;

    private void Awake()
    {
        InitializeGame();
    }

    private void Start()
    {
        // �Ó]������Ԃ���J�n����D
        StartCoroutine(GamesStart());
    }

    private void Update()
    {
        DisplaySettingCanvas();
    }

    // �G���f�B���O����
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

    // Game�̏�����
    private void InitializeGame()
    {
        // Player�̐���
        _player = GameObject.Instantiate(_player);
        _playerController = _player.GetComponent<PlayerController>();
        _fadeProcessController = GetComponent<FadeProcessController>();

        // StoryCanvas�̐���
        _storyCanvas = GameObject.Instantiate(_storyCanvas);
        _storyCanvas.transform.SetParent(transform, false);
        _storyCanvas.SetActive(false);

        // �ݒ��ʂ̐���
        _settingCanvas = GameObject.Instantiate(_settingCanvas);
        _settingCanvas.transform.SetParent(transform, false);
        _settingCanvas.SetActive(false);

        // Slider�ɉ��ʒ����̃C�x���g�ǉ�
        Transform settingPanel = _settingCanvas.transform.Find("Panel");
        _masterVolumeSlider = settingPanel.Find("MasterVolumeSlider").GetComponent<Slider>();
        _BGMVolumeSlider = settingPanel.Find("BGMVolumeSlider").GetComponent<Slider>();
        _gameSEVolumeSlider = settingPanel.Find("SEVolumeSlider").GetComponent<Slider>();
        _masterVolumeSlider.onValueChanged.AddListener(delegate { _mainAudioMixer.SetFloat("MasterVolume", _masterVolumeSlider.value); });
        _BGMVolumeSlider.onValueChanged.AddListener(delegate { _mainAudioMixer.SetFloat("BGMVolume", _BGMVolumeSlider.value); });
        _gameSEVolumeSlider.onValueChanged.AddListener(delegate { _mainAudioMixer.SetFloat("GameSEVolume", _gameSEVolumeSlider.value); });

        // Button��Event�ǉ�
        _storyCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(()=> { _settingCanvas.SetActive(true); GameObject.Destroy(_storyCanvas); });
        _settingCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(_playerController.BeMovablePlayer);
        _settingCanvas.transform.Find("Panel/LoadTitleButton").GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("TitleScene"));
    }


    // Escape�Őݒ���J���D
    private void DisplaySettingCanvas()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _playerController.IsMovable)
        {
            _settingCanvas.SetActive(true);
            _playerController.UnmovablePlayer();
        }
    }

    // GameClear���[�g
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

    // GameOver���[�g
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

    // GameStart���C�Ó]������Ԃ�1�b�҂�FadeIn����D
    private IEnumerator GamesStart()
    {
        transform.Find("FadeProcessCanvas").gameObject.SetActive(true);
        transform.Find("FadeProcessCanvas/Panel").GetComponent<Image>().color = Color.black;
        yield return new WaitForSeconds(1f);
        _fadeProcessController.SetFadeProcess(() => { }, _mainBGM.Play, () => _storyCanvas.SetActive(true), 0.02f, Color.black);
    }

    // GameOver���Ɏg���N���[�`���[�̐���
    private void CreatCreature()
    {
        _creature = GameObject.Instantiate(_creature);
        Vector3 creaturePos = Camera.main.transform.position;
        creaturePos += _player.transform.forward * 0.56f;
        creaturePos.y -= 1f;
        _creature.transform.position = creaturePos;
        _creature.transform.eulerAngles = new Vector3(0, 270 + _player.transform.eulerAngles.y, 0);
    }

    // GameClearCanvas�̐���
    private void CreateGameClearCanvas()
    {
        _gameClearCanvas = GameObject.Instantiate(_gameClearCanvas);
        _gameClearCanvas.transform.SetParent(transform, false);
        _gameClearCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("GameClearScene"));
    }
}
