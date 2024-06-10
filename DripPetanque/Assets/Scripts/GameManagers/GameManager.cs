using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityUtility.CustomAttributes;
using UnityUtility.SceneReference;
using UnityUtility.Singletons;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public CinematicsSubGameManager CinematicsSubGameManager => m_cinematicsSubGameManager;
    public DialogueSubGameManager DialogueSubGameManager => m_dialogueSubGameManager;
    public ExplorationSubGameManager ExplorationSubGameManager => m_explorationSubGameManager;
    public PetanqueSubGameManager PetanqueSubGameManager => m_petanqueSubGameManager;
    public MainMenuSubGameManager MainMenuSubGameManager => m_mainMenuSubGameManager;

    public SubGameManager CurrentSubGameManager => m_currentSubGameManager;

    public GameState CurrentGameState
    {
        get
        {
            if (m_currentSubGameManager == null)
            {
                return GameState.None;
            }
            return m_currentSubGameManager.CorrespondingState;
        }
    }

    public GameManagersSharedDatas SharedDatas => m_sharedDatas;

    public event Action<GameState> OnGameStateEntered;
    public event Action<GameState> OnGameStateExited;

    [Title(nameof(SubGameManager) + "s")]
    [SerializeField] private CinematicsSubGameManager m_cinematicsSubGameManager;
    [SerializeField] private DialogueSubGameManager m_dialogueSubGameManager;
    [SerializeField] private ExplorationSubGameManager m_explorationSubGameManager;
    [SerializeField] private PetanqueSubGameManager m_petanqueSubGameManager;
    [SerializeField] private MainMenuSubGameManager m_mainMenuSubGameManager;

    [Title("Start")]
    [SerializeField] private bool m_loadSceneOnStart = false;
    [SerializeField] private SceneReference m_startScene;
    [SerializeField] private GameState m_startState;

    [Title("Misc")]
    [SerializeField] private CinemachineBrain m_mainCamera;

    [Title("Inputs")]
    [SerializeField] private InputActionAsset m_actionAsset;
    [SerializeField] private string m_commonActionMapName = "Common";

    [NonSerialized] private SubGameManager m_currentSubGameManager;
    [NonSerialized] private GameManagersSharedDatas m_sharedDatas;

    [NonSerialized] private Dictionary<GameState, SubGameManager> m_subGameManagers;

    [NonSerialized] private bool m_initialized;

    public override void Initialize()
    {
        m_initialized = false;

        base.Initialize();

        DontDestroyOnLoad(gameObject);

        VirtualCamerasManager.RegisterBrain(m_mainCamera);
        m_sharedDatas = new GameManagersSharedDatas();

        m_subGameManagers = new Dictionary<GameState, SubGameManager>()
        {
            {
                m_cinematicsSubGameManager.CorrespondingState,
                m_cinematicsSubGameManager
            },
            {
                m_dialogueSubGameManager.CorrespondingState,
                m_dialogueSubGameManager
            },
            {
                m_explorationSubGameManager.CorrespondingState,
                m_explorationSubGameManager
            },
            {
                m_petanqueSubGameManager.CorrespondingState,
                m_petanqueSubGameManager
            },
            {
                m_mainMenuSubGameManager.CorrespondingState,
                m_mainMenuSubGameManager
            },
        };

        foreach (KeyValuePair<GameState, SubGameManager> manager in m_subGameManagers)
        {
            manager.Value.Initialize(m_actionAsset, m_sharedDatas);
        }

        m_actionAsset.FindActionMap(m_commonActionMapName).Enable();

        m_currentSubGameManager = m_subGameManagers[m_startState];
        m_currentSubGameManager.BeginState(GameState.None);
        OnGameStateEntered?.Invoke(m_startState);

        if (m_loadSceneOnStart)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(m_startScene, LoadSceneMode.Single);
            op.completed += OnStartSceneLoaded;
        }
        else
        {
            m_initialized = true;
        }
    }

    public void GoBackToMainMenu()
    {
        StartState(GameState.MainMenu);
    }

    private void OnStartSceneLoaded(AsyncOperation operation)
    {
        operation.completed -= OnStartSceneLoaded;
        m_initialized = true;
    }

    private void Update()
    {
        if (!m_initialized)
        {
            return;
        }

        m_currentSubGameManager.UpdateState(Time.deltaTime);
        if (m_currentSubGameManager.RequestStateChange(out GameState nextState))
        {
            if (nextState != m_currentSubGameManager.CorrespondingState)
            {
                StartState(nextState);
            }
        }
    }

    private void StartState(GameState nextState)
    {
        m_currentSubGameManager.EndState(nextState);
        OnGameStateExited?.Invoke(m_currentSubGameManager.CorrespondingState);

        Debug.LogWarning($"[{nameof(GameManager)}] Exiting GameState {m_currentSubGameManager.CorrespondingState} and entering GameState {nextState}");

        m_currentSubGameManager = m_subGameManagers[nextState];
        m_currentSubGameManager.BeginState(nextState);
        OnGameStateEntered?.Invoke(nextState);
    }
}
