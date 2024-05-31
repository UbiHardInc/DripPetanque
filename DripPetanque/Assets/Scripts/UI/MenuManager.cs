using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityUtility.CustomAttributes;

public class MenuManager : MonoBehaviour
{
    [Title("MainMenuVariables")]
    [SerializeField] private float m_timeForMenuToAppear = 6.30f;
    [Title("MainMenuObjects")]
    [SerializeField] private GameObject m_transitionImage;
    [SerializeField] private Button m_startButton;

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
    
    //private variables
    private bool m_isOnUI;


    // Start is called before the first frame update
    private void Start()
    {
        m_openMenuInput.action.performed += OpenMenu;
        m_closeMenuInput.action.performed += CloseMenu;
        m_moveInput.action.started += MoveInUI;
        m_submitInput.action.performed += PressSubmit;
        m_cancelInput.action.performed += PressCancel;

        if (SceneManager.GetActiveScene().name == "EntryScene")
        {
            _ = StartCoroutine(IntroMainMenu());
            EventSystem.current.SetSelectedGameObject(m_startButton.gameObject);
            m_startButton.Select();
            m_startButton.onClick.AddListener(() => StartGame());
            m_isOnUI = true;
        }
    }

    private IEnumerator IntroMainMenu()
    {
        yield return new WaitForSeconds(m_timeForMenuToAppear);
        _ = StartCoroutine(FadeInAndOutGameObject.FadeInAndOut(m_transitionImage, false, 1f));
    }

    #region InputActionsMethods

    private void OpenMenu(InputAction.CallbackContext obj)
    {
        Debug.LogError("OpenMenuCalled");
        OpenPauseMenu();
    }
    
    private void CloseMenu(InputAction.CallbackContext obj)
    {
        ClosePauseMenu();
    }
    
    private void PressCancel(InputAction.CallbackContext obj)
    {
        if (m_isOnUI)
        {
            SoundManager.Instance.PlayUISFX("cancel");
        }
    }

    private void PressSubmit(InputAction.CallbackContext obj)
    {
        if (m_isOnUI)
        {
            //SoundManager.Instance.PlayUISFX("submit");
        }
    }

    private void MoveInUI(InputAction.CallbackContext obj)
    {
        if (m_isOnUI)
        {
            SoundManager.Instance.PlayUISFX("move");
        }
    }
    #endregion

    #region ButtonMethods

    public void OpenPauseMenu()
    {
        Debug.LogError("OpenPauseMenuCalled");
        m_pauseMenu.SetActive(true);

        if (SceneManager.GetActiveScene().name != "EntryScene")
        {
            m_backMainMenuButton.gameObject.SetActive(true);
            Time.timeScale = 0f;
            SoundManager.Instance.PlayUISFX("start");
        }
        else
        {
            SoundManager.Instance.PlayUISFX("submit");
            m_isOnUI = true;
            m_backMainMenuButton.gameObject.SetActive(false);
        }
        m_firstSelectedButton.Select();

    }

    public void ClosePauseMenu()
    {
        m_pauseMenu.SetActive(false);
        if (SceneManager.GetActiveScene().name == "EntryScene")
        {
            m_startButton.GetComponent<Button>().Select();
        }
        else
        {
            m_isOnUI = false;
            Time.timeScale = 1f;
        }

    }
    
    public void MusicVolumeChange(bool add)
    {
        if (add)
        {
            m_musicSlider.value += 0.1f;
        }
        else
        {
            m_musicSlider.value -= 0.1f;
        }
        _ = m_musicMixerGroup.audioMixer.SetFloat("musicVol", Mathf.Log10(m_musicSlider.value) * 20);

    }

    public void SfxVolumeChange(bool add)
    {
        if (add)
        {
            m_sfxSlider.value += 0.1f;
        }
        else
        {
            m_sfxSlider.value -= 0.1f;
        }
        _ = m_sfxMixerGroup.audioMixer.SetFloat("sfxVol", Mathf.Log10(m_sfxSlider.value) * 20);
        _ = StartCoroutine(SoundManager.Instance.PlayBallSounds(SoundManager.BallSFXType.swoop));

    }

    public void StartGame()
    {
        m_isOnUI = false;
        SoundManager.Instance.PlayUISFX("start");
        
        //ToDo : add LoadGame
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion
    
}
