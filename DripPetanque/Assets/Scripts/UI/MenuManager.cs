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
    [SerializeField, FormerlySerializedAs("timeForMenuToAppear")]
    private float m_timeForMenuToAppear = 6.30f;
    [Title("MainMenuObjects")]
    [SerializeField, FormerlySerializedAs("transitionImage")]
    private GameObject m_transitionImage;
    [SerializeField, FormerlySerializedAs("startButton")]
    private Button m_startButton;
    [SerializeField, FormerlySerializedAs("optionButton")]
    private Button m_optionButton;


    [Title("PauseMenuObjects")]
    [SerializeField, FormerlySerializedAs("pauseMenu")]
    private GameObject m_pauseMenu;
    [SerializeField, FormerlySerializedAs("firstSelectedButton")]
    private Button m_firstSelectedButton;
    [SerializeField, FormerlySerializedAs("musicMixerGroup")]
    private AudioMixerGroup m_musicMixerGroup;
    [SerializeField, FormerlySerializedAs("sfxMixerGroup")]
    private AudioMixerGroup m_sfxMixerGroup;
    [SerializeField, FormerlySerializedAs("musicSlider")]
    private Slider m_musicSlider;
    [SerializeField, FormerlySerializedAs("sfxSlider")]
    private Slider m_sfxSlider;

    [Title("InputActionReferences")]
    [SerializeField, FormerlySerializedAs("openMenu")]
    private InputActionReference m_openMenuInput;
    [SerializeField, FormerlySerializedAs("closeMenu")]
    private InputActionReference m_closeMenuInput;


    // Start is called before the first frame update
    private void Start()
    {
        m_openMenuInput.action.actionMap.Enable();
        m_closeMenuInput.action.actionMap.Enable();
        m_openMenuInput.action.performed += OpenMenu;
        m_closeMenuInput.action.performed += CloseMenu;

        if (SceneManager.GetActiveScene().name == "EntryScene")
        {
            _ = StartCoroutine(IntroMainMenu());
            EventSystem.current.SetSelectedGameObject(m_startButton.gameObject);
            m_startButton.GetComponent<Button>().Select();
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
        SoundManager.Instance.PlayBallSounds(SoundManager.BallSFXType.swoop);

    }

    private IEnumerator IntroMainMenu()
    {
        yield return new WaitForSeconds(m_timeForMenuToAppear);
        _ = StartCoroutine(FadeInAndOutGameObject.FadeInAndOut(m_transitionImage, false, 1f));
    }

    private void OpenMenu(InputAction.CallbackContext obj)
    {
        Debug.LogError("OpenMenuCalled");
        OpenPauseMenu();
    }

    public void OpenPauseMenu()
    {
        Debug.LogError("OpenPauseMenuCalled");
        m_pauseMenu.SetActive(true);

        if (SceneManager.GetActiveScene().name != "EntryScene")
        {
            Time.timeScale = 0f;
        }
        m_firstSelectedButton.Select();

    }

    private void CloseMenu(InputAction.CallbackContext obj)
    {
        ClosePauseMenu();
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
            Time.timeScale = 1f;
        }

    }
}
