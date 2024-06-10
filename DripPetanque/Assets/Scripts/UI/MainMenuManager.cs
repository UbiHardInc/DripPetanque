using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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

    [Title("Inputs")]
    [SerializeField] private InputActionReference m_moveInput;
    [SerializeField] private InputActionReference m_submitInput;
    [SerializeField] private InputActionReference m_cancelInput;

    public void StartMainMenu()
    {
        _ = StartCoroutine(IntroMainMenu());

        SoundManager.Instance.StopAllAudioSources();

        EventSystem.current.SetSelectedGameObject(m_startButton.gameObject);
        SelectStartButton();

        m_startButton.onClick.AddListener(StartGame);
        m_quitButton.onClick.AddListener(QuitGame);
        m_optionsButton.onClick.AddListener(OpenOptions);

        SubscibeToInputEvents();
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
        Image transitionImage = m_transitionImage.GetComponent<Image>();
        Color imageColor = transitionImage.color;
        imageColor.a = 1.0f;
        transitionImage.color = imageColor;
        m_transitionImage.SetActive(true);

        UnsubscibeFromInputEvents();
    }

    private IEnumerator IntroMainMenu()
    {
        yield return new WaitForSeconds(0.5f);

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

    private void SubscibeToInputEvents()
    {
        m_moveInput.action.started += UiSFXUtils.MoveInUI;
        m_submitInput.action.performed += UiSFXUtils.PressSubmit;
        m_cancelInput.action.performed += UiSFXUtils.PressCancel;
    }

    private void UnsubscibeFromInputEvents()
    {
        m_moveInput.action.started -= UiSFXUtils.MoveInUI;
        m_submitInput.action.performed -= UiSFXUtils.PressSubmit;
        m_cancelInput.action.performed -= UiSFXUtils.PressCancel;
    }
}
