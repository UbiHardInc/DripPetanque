using System;
using UnityEngine;
using UnityUtility.SceneReference;

public class MainMenuSubGameManager : SubGameManager
{
    public override GameState CorrespondingState => GameState.MainMenu;

    [SerializeField] private SceneTransitioner m_mainMenuSceneTransitioner;

    [SerializeField] private SceneReference m_mainMenuScene;
    [SerializeField] private SceneReference m_explorationScene;

    [SerializeField] private MainMenuManager m_mainMenuManager;

    public override void BeginState(GameState previousState)
    {
        base.BeginState(previousState);

        m_mainMenuSceneTransitioner.OnFadeInOver += OnMainMenuSceneFadedIn;
        m_mainMenuSceneTransitioner.OnTransitionEnd += OnMainMenuSceneLoaded;

        m_mainMenuSceneTransitioner.SetScene(m_mainMenuScene);
        m_mainMenuSceneTransitioner.StartLoadTransition(fadeIn: false, fadeOut: true);

    }

    public void SelectStartButton()
    {
        m_mainMenuManager.SelectStartButton();
    }

    private void OnMainMenuSceneFadedIn()
    {
        m_mainMenuSceneTransitioner.OnFadeInOver -= OnMainMenuSceneFadedIn;

        m_mainMenuManager.gameObject.SetActive(true);
    }

    private void OnMainMenuSceneLoaded(SceneReference reference)
    {
        m_mainMenuSceneTransitioner.OnTransitionEnd -= OnMainMenuSceneLoaded;
        m_mainMenuManager.StartMainMenu();
        m_mainMenuManager.OnStartButtonClicked += StartGame;

    }

    private void StartGame()
    {
        m_mainMenuManager.OnStartButtonClicked -= StartGame;

        m_mainMenuSceneTransitioner.SetScene(m_explorationScene);
        m_mainMenuSceneTransitioner.StartLoadTransition(fadeIn: true, fadeOut: true);

        m_mainMenuSceneTransitioner.OnFadeInOver += OnExplorationSceneFadedIn;
        m_mainMenuSceneTransitioner.OnTransitionEnd += OnExplorationSceneLoaded;
    }

    private void OnExplorationSceneFadedIn()
    {
        m_mainMenuSceneTransitioner.OnFadeInOver -= OnExplorationSceneFadedIn;

        m_mainMenuManager.QuitMainMenu();
        m_mainMenuManager.gameObject.SetActive(false);
    }

    private void OnExplorationSceneLoaded(SceneReference reference)
    {
        m_mainMenuSceneTransitioner.OnTransitionEnd -= OnExplorationSceneLoaded;
        m_requestedGameState = GameState.Exploration;
    }

}
