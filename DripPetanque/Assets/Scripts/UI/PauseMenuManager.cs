using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityUtility.CustomAttributes;

public class PauseMenuManager : MonoBehaviour
{
    [Title("PauseMenuObjects")]
    [SerializeField] private GameObject m_pauseMenu;
    [SerializeField] private Button m_firstSelectedButton;
    [SerializeField] private AudioMixerGroup m_musicMixerGroup;
    [SerializeField] private AudioMixerGroup m_sfxMixerGroup;
    [SerializeField] private Slider m_musicSlider;
    [SerializeField] private Slider m_sfxSlider;
    [SerializeField] private Button m_backMainMenuButton;

    [Title("InputActionReferences")]
    [SerializeField] private InputActionReference m_openMenuInput;
    [SerializeField] private InputActionReference m_moveInput;
    [SerializeField] private InputActionReference m_submitInput;
    [SerializeField] private InputActionReference m_cancelInput;

    [Title("Misc")]
    [SerializeField] private float m_sliderStep = 0.1f;

    //private variables

    [NonSerialized] private GameManager m_gameManager;
    [NonSerialized] private bool m_isActive;


    // Start is called before the first frame update
    protected void Start()
    {
        m_gameManager = GameManager.Instance;

        m_isActive = false;
        m_pauseMenu.SetActive(false);
        m_openMenuInput.action.performed += OpenMenu;
    }

    #region InputActionsMethods

    private void OpenMenu(InputAction.CallbackContext obj)
    {
        Debug.LogError("OpenMenuCalled");
        OpenPauseMenu(false);
    }

    private void CloseMenu(InputAction.CallbackContext obj)
    {
        ClosePauseMenu();
    }

    #endregion

    #region ButtonMethods
    public void OpenPauseMenu(bool fromMainMenu)
    {
        if (m_isActive)
        {
            return;
        }

        // Prevents from opening the pause menu with the input if we're already on the main menu
        if (!fromMainMenu && m_gameManager.CurrentGameState == GameState.MainMenu)
        {
            return;
        }
        
        m_isActive = true;
        SubscribeToInputEvents();
        m_pauseMenu.SetActive(true);

        if (!fromMainMenu)
        {
            m_backMainMenuButton.gameObject.SetActive(true);
            Time.timeScale = 0f;
            SoundManager.Instance.PlayUISFX("start");
        }
        else
        {
            SoundManager.Instance.PlayUISFX("submit");
            m_backMainMenuButton.gameObject.SetActive(false);
        }
        m_firstSelectedButton.Select();

    }

    private void ClosePauseMenu()
    {
        if (!m_isActive)
        {
            return;
        }

        m_isActive = false;
        UnsubscribeToInputEvents();
        m_pauseMenu.SetActive(false);
        if (m_gameManager.CurrentGameState == GameState.MainMenu)
        {
            m_gameManager.MainMenuSubGameManager.SelectStartButton();
        }
        else
        {
            Time.timeScale = 1f;
        }

    }

    // Called by buttons
    public void MusicVolumeChange(bool add)
    {
        if (add)
        {
            m_musicSlider.value += m_sliderStep;
        }
        else
        {
            m_musicSlider.value -= m_sliderStep;
        }
        _ = m_musicMixerGroup.audioMixer.SetFloat("musicVol", Mathf.Log10(m_musicSlider.value) * 20);

    }

    // Called by buttons
    public void SfxVolumeChange(bool add)
    {
        if (add)
        {
            m_sfxSlider.value += m_sliderStep;
        }
        else
        {
            m_sfxSlider.value -= m_sliderStep;
        }
        _ = m_sfxMixerGroup.audioMixer.SetFloat("sfxVol", Mathf.Log10(m_sfxSlider.value) * 20);
        SoundManager.Instance.PlayBallSounds(SoundManager.BallSFXType.swoop);
    }

    private void BackToMainMenu()
    {
        ClosePauseMenu();
        m_gameManager.GoBackToMainMenu();
    }

    private void SubscribeToInputEvents()
    {
        m_cancelInput.action.performed += CloseMenu;

        if (m_gameManager.CurrentGameState != GameState.MainMenu)
        {
            m_moveInput.action.started += UiSFXUtils.MoveInUI;
            m_submitInput.action.performed += UiSFXUtils.PressSubmit;
            m_cancelInput.action.performed += UiSFXUtils.PressCancel;
        }

        m_backMainMenuButton.onClick.AddListener(BackToMainMenu);
    }

    private void UnsubscribeToInputEvents()
    {
        m_cancelInput.action.performed -= CloseMenu;

        if (m_gameManager.CurrentGameState != GameState.MainMenu)
        {
            m_moveInput.action.started -= UiSFXUtils.MoveInUI;
            m_submitInput.action.performed -= UiSFXUtils.PressSubmit;
            m_cancelInput.action.performed -= UiSFXUtils.PressCancel;
        }

        m_backMainMenuButton.onClick.RemoveListener(BackToMainMenu);
    }

    #endregion

}
