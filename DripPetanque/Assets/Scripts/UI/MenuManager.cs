using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("MainMenuVariables")]
    [SerializeField] private float timeForMenuToAppear = 6.30f;
    [Header("MainMenuObjects")]
    [SerializeField] private GameObject transitionImage;
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    
    
    //[Header("PauseMenuVariables")]

    [Header("PauseMenuObjects")] 
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button firstSelectedButton;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("InputActionReferences")]
    [SerializeField] private InputActionReference openMenu;
    [SerializeField] private InputActionReference closeMenu;
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        openMenu.action.actionMap.Enable();
        closeMenu.action.actionMap.Enable();
        openMenu.action.performed += OpenMenu;
        closeMenu.action.performed += CloseMenu;
        
        if (SceneManager.GetActiveScene().name == "EntryScene")
        {
            StartCoroutine(IntroMainMenu());
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
            startButton.GetComponent<Button>().Select();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MusicVolumeChange(bool add)
    {
        if (add)
        {
            musicSlider.value += 0.1f;
        }
        else
        {
            musicSlider.value -= 0.1f;
        }
        musicMixerGroup.audioMixer.SetFloat("musicVol", Mathf.Log10(musicSlider.value) * 20);
        
    }
    
    public void SfxVolumeChange(bool add)
    {
        if (add)
        {
            sfxSlider.value += 0.1f;
        }
        else
        {
            sfxSlider.value -= 0.1f;
        }
        sfxMixerGroup.audioMixer.SetFloat("sfxVol", Mathf.Log10(sfxSlider.value) * 20);
        StartCoroutine(SoundManager.Instance.PlayBallSounds(SoundManager.BallSFXType.swoop));

    }

    private IEnumerator IntroMainMenu()
    {
        yield return new WaitForSeconds(timeForMenuToAppear);
        StartCoroutine(FadeInAndOutGameObject.FadeInAndOut(transitionImage, false, 1f));
    }
    
    private void OpenMenu(InputAction.CallbackContext obj)
    {
        Debug.LogError("OpenMenuCalled");
        OpenPauseMenu();
    }

    public void OpenPauseMenu()
    {
        Debug.LogError("OpenPauseMenuCalled");
        pauseMenu.SetActive(true);
        
        if (SceneManager.GetActiveScene().name != "EntryScene")
        {
            Time.timeScale = 0f;
        }
        firstSelectedButton.Select();
        
    }
    
    private void CloseMenu(InputAction.CallbackContext obj)
    {
        ClosePauseMenu();
    }

    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
        if (SceneManager.GetActiveScene().name == "EntryScene")
        {
            startButton.GetComponent<Button>().Select();
        }
        else
        {
            Time.timeScale = 1f;
        }
        
    }
}
