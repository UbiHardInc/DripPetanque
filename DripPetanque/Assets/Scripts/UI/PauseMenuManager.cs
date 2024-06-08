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
    [SerializeField] private InputActionReference m_closeMenuInput;
    [SerializeField] private InputActionReference m_moveInput;
    [SerializeField] private InputActionReference m_submitInput;
    [SerializeField] private InputActionReference m_cancelInput;

    [Title("Misc")]
    [SerializeField] private float m_sliderStep = 0.1f;

    //private variables

    [NonSerialized] private GameManager m_gameManager;


    // Start is called before the first frame update
    protected void Start()
    {
        m_gameManager = GameManager.Instance;
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

    private void PressCancel(InputAction.CallbackContext obj)
    {
        SoundManager.Instance.PlayUISFX("cancel");
    }

    private void PressSubmit(InputAction.CallbackContext obj)
    {
        //SoundManager.Instance.PlayUISFX("submit");
    }

    private void MoveInUI(InputAction.CallbackContext obj)
    {
        SoundManager.Instance.PlayUISFX("move");
    }
    #endregion

    #region ButtonMethods

    public void OpenPauseMenu(bool fromMainMenu)
    {
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

    public void ClosePauseMenu()
    {
        UnsubscribeToInputEvents();
        m_pauseMenu.SetActive(false);
        if (m_gameManager.CurrentSubGameManager.CorrespondingState == GameState.MainMenu)
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

    private void SubscribeToInputEvents()
    {
        m_openMenuInput.action.performed += OpenMenu;
        m_closeMenuInput.action.performed += CloseMenu;
        m_moveInput.action.started += MoveInUI;
        m_submitInput.action.performed += PressSubmit;
        m_cancelInput.action.performed += PressCancel;
    }

    private void UnsubscribeToInputEvents()
    {
        m_openMenuInput.action.performed -= OpenMenu;
        m_closeMenuInput.action.performed -= CloseMenu;
        m_moveInput.action.started -= MoveInUI;
        m_submitInput.action.performed -= PressSubmit;
        m_cancelInput.action.performed -= PressCancel;
    }

    #endregion

}
