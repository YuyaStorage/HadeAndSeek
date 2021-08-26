using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    // StartButton
    [SerializeField]
    private GameObject _startButton = null;
    // QuitButton
    [SerializeField]
    private GameObject _quitButton = null;

    private void Start()
    {
        _startButton.GetComponent<Button>().onClick.AddListener(LoadGameScean);
        _quitButton.GetComponent<Button>().onClick.AddListener(Quit);
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {

    }

    // GameSceneÇÃÉçÅ[Éh
    private void LoadGameScean()
    {
        SceneManager.LoadScene("GameScene");
    }

    // GameèIóπ
    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
