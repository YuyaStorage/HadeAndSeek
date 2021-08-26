using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ResearchObject : MonoBehaviour
{
    // Playerにチェックされたかどうか
    public bool IsChecked { get; set; } = false;

    // 継承先でEventを追加
    protected GameObject[] Buttons;
    // 表示するキャンバス
    protected GameObject ResearchObjectCanvas;

    // 調査対象をクリックした時に表示するUI
    [SerializeField]
    private GameObject _defaultCanvasPrefab = null;

    // ResearchObjectCanvasに追加するボタン
    [SerializeField]
    private GameObject _buttonPrefab = null;

    // 調査対象の説明
    [SerializeField, TextArea(1, 6)]
    private string _mainText = null;

    // ボタンに表示するテキスト
    [SerializeField]
    private string[] _buttonTexts = null;

    [SerializeField]
    private float _buttonRectTransformPosionY = 180f;

    // ボタンの間隔
    [SerializeField]
    private float _buttonSpace = 400f;

    private void Awake()
    {
        InitializeCanvas();
    }

    // ResearchObjectCanvasの初期化
    private void InitializeCanvas()
    {
        // ResearchObjectCanvasの生成
        ResearchObjectCanvas = GameObject.Instantiate(_defaultCanvasPrefab);
        ResearchObjectCanvas.name = "ResearchObjectCanvas";
        ResearchObjectCanvas.transform.SetParent(transform, false);
        ResearchObjectCanvas.transform.Find("Panel/MainText").gameObject.GetComponent<TextMeshProUGUI>().text = _mainText;

        // CloseButtonにEvent追加
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        ResearchObjectCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(() => playerController.BeMovablePlayer());

        // ボタンの生成
        Buttons = new GameObject[_buttonTexts.Length];
        for (int i = 0; i < _buttonTexts.Length; ++i)
        {
            Buttons[i] = GameObject.Instantiate(_buttonPrefab);
            Buttons[i].transform.SetParent(ResearchObjectCanvas.transform.Find("Panel"), false);
            Buttons[i].GetComponent<TextMeshProUGUI>().text = _buttonTexts[i];

            RectTransform rectTransform = Buttons[i].GetComponent<RectTransform>();
            if (_buttonTexts.Length % 2 == 0)
            {
                rectTransform.anchoredPosition = new Vector2(_buttonSpace * (i - _buttonTexts.Length / 2 + 0.5f), _buttonRectTransformPosionY);
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(_buttonSpace * (i - _buttonTexts.Length / 2), _buttonRectTransformPosionY);
            }

        }

        // 非アクティブ化しておく．
        ResearchObjectCanvas.SetActive(false);
    }

    // ボタンのないキャンバスの生成
    protected GameObject GenerateCanvas(string mainText)
    {
        // ResearchObjectCanvasの生成
        GameObject researchObjectCanvas = GameObject.Instantiate(_defaultCanvasPrefab);
        researchObjectCanvas.transform.SetParent(transform, false);
        researchObjectCanvas.transform.Find("Panel/MainText").GetComponent<TextMeshProUGUI>().text = mainText;

        // CloseButtonにPlayerを動かせるようにしてキャンバスを破壊するイベントを追加
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        researchObjectCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(() => { playerController.BeMovablePlayer(); Destroy(researchObjectCanvas); });

        return researchObjectCanvas;
    }

    // ボタンのついたキャンバスの生成
    protected GameObject GenerateCanvas(string mainText, System.Tuple<string, UnityAction>[] buttonDate)
    {
        // ResearchObjectCanvasの生成
        GameObject researchObjectCanvas = GameObject.Instantiate(_defaultCanvasPrefab);
        researchObjectCanvas.transform.SetParent(transform, false);
        researchObjectCanvas.transform.Find("Panel/MainText").GetComponent<TextMeshProUGUI>().text = mainText;
        // CloseButtonにEvent追加
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        researchObjectCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(() => { playerController.BeMovablePlayer(); Destroy(researchObjectCanvas); });

        // Buttonの生成
        for (int i = 0; i < buttonDate.Length; ++i)
        {
            GameObject button = GameObject.Instantiate(_buttonPrefab);
            button.transform.SetParent(researchObjectCanvas.transform.Find("Panel"), false);
            button.GetComponent<TextMeshProUGUI>().text = buttonDate[i].Item1;
            button.GetComponent<Button>().onClick.AddListener(buttonDate[i].Item2);

            RectTransform rectTransform = button.GetComponent<RectTransform>();
            if (buttonDate.Length % 2 == 0)
            {
                rectTransform.anchoredPosition = new Vector2(_buttonSpace * (i - buttonDate.Length / 2 + 0.5f), _buttonRectTransformPosionY);
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(_buttonSpace * (i - buttonDate.Length / 2), _buttonRectTransformPosionY);
            }


        }
        return researchObjectCanvas;
    }

}
