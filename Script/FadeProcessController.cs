using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// SetFadeProcess���ĂԂ��Ƃ�FadeProcess�����s����D
public class FadeProcessController : MonoBehaviour
{
    // FadeProcessCanvas
    [SerializeField]
    private GameObject _fadeProcessCanvas = null;

    // FadeProcess���x
    [SerializeField]
    private float _defaultFadeSpeed = 0.02f;

    // Panel�̃J���[
    [SerializeField]
    private Color _defaultFadePanelColor;

    // FadeProcess�p�p�l��
    private GameObject _fadeProcessPanel = null;
    // FadePanel�p��Image
    private Image _fadeImage;
    // FadeProcess���s���̑��x
    private float _fadeSpeed;

    // FadeProcess�̐i�s�t���O
    private bool _isFadeOut = false;
    private bool _isFadeIn = false;
    private bool _isFadeProcess = false;
    private bool _isStartAction = false;
    private bool _isMiddleAction = false;
    private bool _isEndAction = false;

    // FadeProcess���Ɏ��s���郁�\�b�h
    // FadeOut�O�̏���
    private System.Action _startAction;
    // FadeOut��̏���
    private System.Action _middleAction;
    // FadeIn��̏���
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

    // Fade�����̓o�^, �f�t�H���g���x�Ŏ��s
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

    // ���x�w��
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

    // ���x�ƃJ���[�w��
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

    // Canvas�̏�����
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

    // Fade����
    private void FadeProcess()
    {
        // FadeOut�O�̏���
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

        // FadeOut��̏���
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

        // FadeIn��̏���
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

        // alfa�l�𑝂₷�D
        Color color = _fadeImage.color;
        color.a += _fadeSpeed;
        // alfa�l��1f�𒴂�����FadeOut�I��
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
        // alfa�l�����炷�D
        Color color = _fadeImage.color;
        color.a -= _fadeSpeed;
        // alfa�l��0f�����������FadeIn�I��
        if (color.a <= 0f)
        {
            color.a = 0f;
            _isFadeIn = false;
            _fadeProcessCanvas.SetActive(false);
        }
        _fadeImage.color = color;
    }
}