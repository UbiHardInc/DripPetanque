using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtility.CustomAttributes;
using UnityUtility.Singletons;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public CinematicsSubGameManager CinematicsSubGameManager => m_cinematicsSubGameManager;
    public DialogueSubGameManager DialogueSubGameManager => m_dialogueSubGameManager;
    public ExplorationSubGameManager ExplorationSubGameManager => m_explorationSubGameManager;
    public PetanqueSubGameManager PetanqueSubGameManager => m_petanqueSubGameManager;

    public GameManagersSharedDatas SharedDatas => m_sharedDatas;

    public event Action<GameState> OnGameStateEntered;
    public event Action<GameState> OnGameStateExited;

    [Title(nameof(SubGameManager) + "s")]
    [SerializeField] private CinematicsSubGameManager m_cinematicsSubGameManager;
    [SerializeField] private DialogueSubGameManager m_dialogueSubGameManager;
    [SerializeField] private ExplorationSubGameManager m_explorationSubGameManager;
    [SerializeField] private PetanqueSubGameManager m_petanqueSubGameManager;

    [Title("Misc")]
    [SerializeField] private GameState m_startState;

    [SerializeField] private CinemachineBrain m_mainCamera;

    [Title("Inputs")]
    [SerializeField] private InputActionAsset m_actionAsset;
    [SerializeField] private string m_commonActionMapName = "Common";

    [NonSerialized] private SubGameManager m_currentSubGameManager;
    [NonSerialized] private GameManagersSharedDatas m_sharedDatas;

    [NonSerialized] private Dictionary<GameState, SubGameManager> m_subGameManagers;

    public override void Initialize()
    {
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
        };

        foreach (KeyValuePair<GameState, SubGameManager> manager in m_subGameManagers)
        {
            manager.Value.Initialize(m_actionAsset, m_sharedDatas);
        }

        m_actionAsset.FindActionMap(m_commonActionMapName).Enable();

        m_currentSubGameManager = m_subGameManagers[m_startState];
        m_currentSubGameManager.BeginState(GameState.None);
        OnGameStateEntered?.Invoke(m_startState);
    }

    private void Update()
    {
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

        m_currentSubGameManager = m_subGameManagers[nextState];
        m_currentSubGameManager.BeginState(nextState);
        OnGameStateEntered?.Invoke(nextState);
    }
}
