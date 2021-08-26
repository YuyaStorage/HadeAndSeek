using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BottleController : ResearchObject
{
    // 何色のボトルか
    [SerializeField]
    private bool _isRedBottle = false;
    [SerializeField]
    private bool _isBlueBottle = false;
    [SerializeField]
    private bool _isGreenBottle = false;

    // 飲んだ時に表示するテキスト
    [SerializeField, TextArea(1, 4)]
    private string _drinkText = null;
    // 浴びた時に表示するテキスト
    [SerializeField, TextArea(1, 4)]
    private string _showerText = null;
    // GameOver時に表示するテキスト
    private string _medicineGameOverText = "全身にものすごい痛みが走る。\n\nあまりの痛みに意識を失った。";

    // GameManager
    private GameManager _gameManager;
    // Fade処理用
    private FadeProcessController _fadeProcessController;

    private void Start()
    {
        InitializeBottle();
    }
    
    // ボトルの初期化
    private void InitializeBottle()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _fadeProcessController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<FadeProcessController>();

        // ボタンにイベント追加
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

    // 飲むを選択したときのキャンバス
    private void CreatDrinkCanvas()
    {
        if(!_isRedBottle && (_gameManager.IsBlueMedicineUsed || _gameManager.IsGreenMedicineUsed))
        {
            GameObject gameOverCanvas = GenerateCanvas(_medicineGameOverText);
            ResearchObjectCanvas.SetActive(false);
            gameOverCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("GameOverScene"));
            gameOverCanvas.transform.Find("Panel/CloseButton/Text").GetComponent<TextMeshProUGUI>().text = "次へ";
        }
        else
        {
            GenerateCanvas(_drinkText);
            ResearchObjectCanvas.SetActive(false);
            if (_isBlueBottle) _gameManager.IsBlueMedicineUsed = true;
            if (_isGreenBottle) _gameManager.IsGreenMedicineUsed = true;
        }
    }

    // 浴びるを選択したときのキャンバス
    private void CreatShowerCanvas()
    {
        if (!_isRedBottle && (_gameManager.IsBlueMedicineUsed || _gameManager.IsGreenMedicineUsed))
        {
            GameObject gameOverCanvas = GenerateCanvas(_medicineGameOverText);
            ResearchObjectCanvas.SetActive(false);
            gameOverCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("GameOverScene"));
            gameOverCanvas.transform.Find("Panel/CloseButton/Text").GetComponent<TextMeshProUGUI>().text = "次へ";
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
