using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// SetFadeProcessを呼ぶことでFadeProcessを実行する．
public class FadeProcessController : MonoBehaviour
{
    // FadeProcessCanvas
    [SerializeField]
    private GameObject _fadeProcessCanvas = null;

    // FadeProcess速度
    [SerializeField]
    private float _defaultFadeSpeed = 0.02f;

    // Panelのカラー
    [SerializeField]
    private Color _defaultFadePanelColor;

    // FadeProcess用パネル
    private GameObject _fadeProcessPanel = null;
    // FadePanel用のImage
    private Image _fadeImage;
    // FadeProcess実行時の速度
    private float _fadeSpeed;

    // FadeProcessの進行フラグ
    private bool _isFadeOut = false;
    private bool _isFadeIn = false;
    private bool _isFadeProcess = false;
    private bool _isStartAction = false;
    private bool _isMiddleAction = false;
    private bool _isEndAction = false;

    // FadeProcess時に実行するメソッド
    // FadeOut前の処理
    private System.Action _startAction;
    // FadeOut後の処理
    private System.Action _middleAction;
    // FadeIn後の処理
    private System.Action _endAction;

    private void Awake()
    {
        InitializeCanvas();
    }

    private void FixedUpdate()
    {
        if (_isFadeProcess)
        {
            FadeProcess();
        }
    }

    // Fade処理の登録, デフォルト速度で実行
    public void SetFadeProcess(System.Action startAction, System.Action middleAction, System.Action endAction)
    {
        _fadeSpeed = _defaultFadeSpeed;
        _fadeImage.color = _defaultFadePanelColor;

        _isFadeProcess = true;
        _isFadeOut = true;
        _isFadeIn = true;
        _isStartAction = true;
        _isMiddleAction = true;
        _isEndAction = true;

        _startAction = startAction;
        _middleAction = middleAction;
        _endAction = endAction;
    }

    // 速度指定
    public void SetFadeProcess(System.Action startAction, System.Action middleAction, System.Action endAction, float fadeSpeed)
    {
        _fadeSpeed = fadeSpeed;
        _fadeImage.color = _defaultFadePanelColor;

        _isFadeProcess = true;
        _isFadeOut = true;
        _isFadeIn = true;
        _isStartAction = true;
        _isMiddleAction = true;
        _isEndAction = true;

        _startAction = startAction;
        _middleAction = middleAction;
        _endAction = endAction;
    }

    // 速度とカラー指定
    public void SetFadeProcess(System.Action startAction, System.Action middleAction, System.Action endAction, float fadeSpeed, Color panelColor)
    {
        _fadeSpeed = fadeSpeed;
        _fadeImage.color = panelColor;

        _isFadeProcess = true;
        _isFadeOut = true;
        _isFadeIn = true;
        _isStartAction = true;
        _isMiddleAction = true;
        _isEndAction = true;

        _startAction = startAction;
        _middleAction = middleAction;
        _endAction = endAction;
    }

    // Canvasの初期化
    private void InitializeCanvas()
    {
        _fadeProcessCanvas = GameObject.Instantiate(_fadeProcessCanvas);
        _fadeProcessCanvas.name = "FadeProcessCanvas";
        _fadeProcessCanvas.transform.SetParent(transform, false);
        _fadeProcessPanel = _fadeProcessCanvas.transform.Find("Panel").gameObject;
        _fadeImage = _fadeProcessPanel.GetComponent<Image>();
        _fadeImage.color = _defaultFadePanelColor;
        _fadeProcessCanvas.SetActive(false);
    }

    // Fade処理
    private void FadeProcess()
    {
        // FadeOut前の処理
        if (_isStartAction)
        {
            _startAction();
            _isStartAction = false;
        }

        if (_isFadeOut)
        {
            FadeOut();
            return;
        }

        // FadeOut後の処理
        if (_isMiddleAction)
        {
            _middleAction();
            _isMiddleAction = false;
        }

        if (_isFadeIn)
        {
            FadeIn();
            return;
        }

        // FadeIn後の処理
        if (_isEndAction)
        {
            _endAction();
            _isEndAction = false;
            _isFadeProcess = false;
        }
    }

    // FadeOut
    private void FadeOut()
    {
        _fadeProcessCanvas.SetActive(true);

        // alfa値を増やす．
        Color color = _fadeImage.color;
        color.a += _fadeSpeed;
        // alfa値が1fを超えたらFadeOut終了
        if (color.a >= 1f)
        {
            color.a = 1f;
            _isFadeOut = false;
        }
        _fadeImage.color = color;
    }

    // FadeIn
    private void FadeIn()
    {
        // alfa値を減らす．
        Color color = _fadeImage.color;
        color.a -= _fadeSpeed;
        // alfa値が0fを下回ったらFadeIn終了
        if (color.a <= 0f)
        {
            color.a = 0f;
            _isFadeIn = false;
            _fadeProcessCanvas.SetActive(false);
        }
        _fadeImage.color = color;
    }
}