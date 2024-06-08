using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityUtility.CustomAttributes;

public class MainMenuManager : MonoBehaviour
{
    public event Action OnStartButtonClicked;

    [SerializeField] private PauseMenuManager m_pauseMenuManager;

    [SerializeField] private RectTransform m_logoRoot;

    [Title("MainMenuVariables")]
    [SerializeField] private float m_timeForMenuToAppear = 6.30f;

    [Title("MainMenuObjects")]
    [SerializeField] private GameObject m_transitionImage;
    [SerializeField] private Button m_startButton;
    [SerializeField] private Button m_quitButton;
    [SerializeField] private Button m_optionsButton;

    public void StartMainMenu()
    {
        _ = StartCoroutine(IntroMainMenu());

        EventSystem.current.SetSelectedGameObject(m_startButton.gameObject);
        SelectStartButton();

        m_startButton.onClick.AddListener(StartGame);
        m_quitButton.onClick.AddListener(QuitGame);
        m_optionsButton.onClick.AddListener(OpenOptions);
    }

    public void SelectStartButton()
    {
        m_startButton.Select();
    }

    public void QuitMainMenu()
    {
        m_startButton.onClick.RemoveListener(StartGame);
        m_quitButton.onClick.RemoveListener(QuitGame);
        m_optionsButton.onClick.RemoveListener(OpenOptions);

        m_logoRoot.gameObject.SetActive(false);
        m_transitionImage.SetActive(true);
    }

    private IEnumerator IntroMainMenu()
    {
        m_logoRoot.gameObject.SetActive(true);
        yield return new WaitForSeconds(m_timeForMenuToAppear);
        yield return FadeInAndOutGameObject.FadeInAndOut(m_transitionImage, false, 1f);
        m_transitionImage.SetActive(false);
    }

    private void StartGame()
    {
        SoundManager.Instance.PlayUISFX("start");
        OnStartButtonClicked?.Invoke();
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void OpenOptions()
    {
        m_pauseMenuManager.OpenPauseMenu(true);
    }
}
