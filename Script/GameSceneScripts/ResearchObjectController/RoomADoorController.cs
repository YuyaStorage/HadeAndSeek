using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomADoorController : ResearchObject
{
    // RoomAの紙のスクリプト
    [SerializeField]
    private ResearchObject _roomAPaperScript = null;
    // Button1のCanvasのMainText
    [SerializeField, TextArea(1, 6)]
    private string _button1Text = null;
    // ドアを開けた時の音
    [SerializeField]
    private AudioSource _openDoorSE = null;

    // Fade処理用
    private FadeProcessController _fadeProcessController;
    // Player
    private GameObject _player;
    // Player操作Script
    private PlayerController _playerController;

    private void Start()
    {
        InitializeDoor();
    }

    // ドアの初期化
    private void InitializeDoor()
    {
        _fadeProcessController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<FadeProcessController>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>();
        Buttons[0].GetComponent<Button>().onClick.AddListener(OpenDoor);
        Buttons[1].GetComponent<Button>().onClick.AddListener(LookOutDoor);
    }

    // Button[0]に登録する処理, ドアを開けてPlayerの位置を調整する．ドアを調べられないようにする．
    private void OpenDoor()
    {
        if (_roomAPaperScript.IsChecked)
        {
            _fadeProcessController.SetFadeProcess(() =>
            {
                _playerController.UnmovablePlayer();
                _openDoorSE.Play();
            }, () =>
            {
                transform.localEulerAngles = new Vector3(0, 120, 0);
                _player.transform.position = new Vector3(-4.5f, 0, -4f);
                _player.transform.localEulerAngles = Vector3.zero;
                Camera.main.transform.localEulerAngles = Vector3.zero;
                Destroy(ResearchObjectCanvas);
                gameObject.layer = 0;
            }, _playerController.BeMovablePlayer, 0.04f);
        }
        else
        {
            string maintext = "もう少し部屋を調べてみよう";
            GenerateCanvas(maintext);
            ResearchObjectCanvas.SetActive(false);
        }
    }

    // Button[1]に登録する処理，Canvasの生成
    private void LookOutDoor()
    {
        GenerateCanvas(_button1Text);
        ResearchObjectCanvas.SetActive(false);
    }
}
