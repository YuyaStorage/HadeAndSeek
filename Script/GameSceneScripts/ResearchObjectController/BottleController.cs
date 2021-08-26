using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BottleController : ResearchObject
{
    // ���F�̃{�g����
    [SerializeField]
    private bool _isRedBottle = false;
    [SerializeField]
    private bool _isBlueBottle = false;
    [SerializeField]
    private bool _isGreenBottle = false;

    // ���񂾎��ɕ\������e�L�X�g
    [SerializeField, TextArea(1, 4)]
    private string _drinkText = null;
    // ���т����ɕ\������e�L�X�g
    [SerializeField, TextArea(1, 4)]
    private string _showerText = null;
    // GameOver���ɕ\������e�L�X�g
    private string _medicineGameOverText = "�S�g�ɂ��̂������ɂ݂�����B\n\n���܂�̒ɂ݂Ɉӎ����������B";

    // GameManager
    private GameManager _gameManager;
    // Fade�����p
    private FadeProcessController _fadeProcessController;

    private void Start()
    {
        InitializeBottle();
    }
    
    // �{�g���̏�����
    private void InitializeBottle()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _fadeProcessController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<FadeProcessController>();

        // �{�^���ɃC�x���g�ǉ�
        if (_isRedBottle)
        {
            Buttons[0].GetComponent<Button>().onClick.AddListener(CreatDrinkCanvas);
            Buttons[1].GetComponent<Button>().onClick.AddListener(CreatShowerCanvas);
        }
        else
        {
            Color panelColor = Color.red;
            panelColor.a = 0;
            Buttons[0].GetComponent<Button>().onClick.AddListener(() => _fadeProcessController.SetFadeProcess(() => { }, () => { }, CreatDrinkCanvas, 0.2f, panelColor));
            Buttons[1].GetComponent<Button>().onClick.AddListener(() => _fadeProcessController.SetFadeProcess(() => { }, () => { }, CreatShowerCanvas, 0.2f, panelColor));
        }
        
    }

    // ���ނ�I�������Ƃ��̃L�����o�X
    private void CreatDrinkCanvas()
    {
        if(!_isRedBottle && (_gameManager.IsBlueMedicineUsed || _gameManager.IsGreenMedicineUsed))
        {
            GameObject gameOverCanvas = GenerateCanvas(_medicineGameOverText);
            ResearchObjectCanvas.SetActive(false);
            gameOverCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("GameOverScene"));
            gameOverCanvas.transform.Find("Panel/CloseButton/Text").GetComponent<TextMeshProUGUI>().text = "����";
        }
        else
        {
            GenerateCanvas(_drinkText);
            ResearchObjectCanvas.SetActive(false);
            if (_isBlueBottle) _gameManager.IsBlueMedicineUsed = true;
            if (_isGreenBottle) _gameManager.IsGreenMedicineUsed = true;
        }
    }

    // ���т��I�������Ƃ��̃L�����o�X
    private void CreatShowerCanvas()
    {
        if (!_isRedBottle && (_gameManager.IsBlueMedicineUsed || _gameManager.IsGreenMedicineUsed))
        {
            GameObject gameOverCanvas = GenerateCanvas(_medicineGameOverText);
            ResearchObjectCanvas.SetActive(false);
            gameOverCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("GameOverScene"));
            gameOverCanvas.transform.Find("Panel/CloseButton/Text").GetComponent<TextMeshProUGUI>().text = "����";
        }
        else
        {
            GenerateCanvas(_showerText);
            ResearchObjectCanvas.SetActive(false);
            if (_isBlueBottle)
            {
                _gameManager.IsBlueMedicineUsed = true;
                _gameManager.IsBlueMedicineShowered = true;
            }
            if (_isGreenBottle) _gameManager.IsGreenMedicineUsed = true;
        }
    }
}
