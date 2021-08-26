using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomADoorController : ResearchObject
{
    // RoomA�̎��̃X�N���v�g
    [SerializeField]
    private ResearchObject _roomAPaperScript = null;
    // Button1��Canvas��MainText
    [SerializeField, TextArea(1, 6)]
    private string _button1Text = null;
    // �h�A���J�������̉�
    [SerializeField]
    private AudioSource _openDoorSE = null;

    // Fade�����p
    private FadeProcessController _fadeProcessController;
    // Player
    private GameObject _player;
    // Player����Script
    private PlayerController _playerController;

    private void Start()
    {
        InitializeDoor();
    }

    // �h�A�̏�����
    private void InitializeDoor()
    {
        _fadeProcessController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<FadeProcessController>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>();
        Buttons[0].GetComponent<Button>().onClick.AddListener(OpenDoor);
        Buttons[1].GetComponent<Button>().onClick.AddListener(LookOutDoor);
    }

    // Button[0]�ɓo�^���鏈��, �h�A���J����Player�̈ʒu�𒲐�����D�h�A�𒲂ׂ��Ȃ��悤�ɂ���D
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
            string maintext = "�������������𒲂ׂĂ݂悤";
            GenerateCanvas(maintext);
            ResearchObjectCanvas.SetActive(false);
        }
    }

    // Button[1]�ɓo�^���鏈���CCanvas�̐���
    private void LookOutDoor()
    {
        GenerateCanvas(_button1Text);
        ResearchObjectCanvas.SetActive(false);
    }
}
