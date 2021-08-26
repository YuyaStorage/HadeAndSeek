using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ResearchObject : MonoBehaviour
{
    // Player�Ƀ`�F�b�N���ꂽ���ǂ���
    public bool IsChecked { get; set; } = false;

    // �p�����Event��ǉ�
    protected GameObject[] Buttons;
    // �\������L�����o�X
    protected GameObject ResearchObjectCanvas;

    // �����Ώۂ��N���b�N�������ɕ\������UI
    [SerializeField]
    private GameObject _defaultCanvasPrefab = null;

    // ResearchObjectCanvas�ɒǉ�����{�^��
    [SerializeField]
    private GameObject _buttonPrefab = null;

    // �����Ώۂ̐���
    [SerializeField, TextArea(1, 6)]
    private string _mainText = null;

    // �{�^���ɕ\������e�L�X�g
    [SerializeField]
    private string[] _buttonTexts = null;

    [SerializeField]
    private float _buttonRectTransformPosionY = 180f;

    // �{�^���̊Ԋu
    [SerializeField]
    private float _buttonSpace = 400f;

    private void Awake()
    {
        InitializeCanvas();
    }

    // ResearchObjectCanvas�̏�����
    private void InitializeCanvas()
    {
        // ResearchObjectCanvas�̐���
        ResearchObjectCanvas = GameObject.Instantiate(_defaultCanvasPrefab);
        ResearchObjectCanvas.name = "ResearchObjectCanvas";
        ResearchObjectCanvas.transform.SetParent(transform, false);
        ResearchObjectCanvas.transform.Find("Panel/MainText").gameObject.GetComponent<TextMeshProUGUI>().text = _mainText;

        // CloseButton��Event�ǉ�
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        ResearchObjectCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(() => playerController.BeMovablePlayer());

        // �{�^���̐���
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

        // ��A�N�e�B�u�����Ă����D
        ResearchObjectCanvas.SetActive(false);
    }

    // �{�^���̂Ȃ��L�����o�X�̐���
    protected GameObject GenerateCanvas(string mainText)
    {
        // ResearchObjectCanvas�̐���
        GameObject researchObjectCanvas = GameObject.Instantiate(_defaultCanvasPrefab);
        researchObjectCanvas.transform.SetParent(transform, false);
        researchObjectCanvas.transform.Find("Panel/MainText").GetComponent<TextMeshProUGUI>().text = mainText;

        // CloseButton��Player�𓮂�����悤�ɂ��ăL�����o�X��j�󂷂�C�x���g��ǉ�
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        researchObjectCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(() => { playerController.BeMovablePlayer(); Destroy(researchObjectCanvas); });

        return researchObjectCanvas;
    }

    // �{�^���̂����L�����o�X�̐���
    protected GameObject GenerateCanvas(string mainText, System.Tuple<string, UnityAction>[] buttonDate)
    {
        // ResearchObjectCanvas�̐���
        GameObject researchObjectCanvas = GameObject.Instantiate(_defaultCanvasPrefab);
        researchObjectCanvas.transform.SetParent(transform, false);
        researchObjectCanvas.transform.Find("Panel/MainText").GetComponent<TextMeshProUGUI>().text = mainText;
        // CloseButton��Event�ǉ�
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        researchObjectCanvas.transform.Find("Panel/CloseButton").GetComponent<Button>().onClick.AddListener(() => { playerController.BeMovablePlayer(); Destroy(researchObjectCanvas); });

        // Button�̐���
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
