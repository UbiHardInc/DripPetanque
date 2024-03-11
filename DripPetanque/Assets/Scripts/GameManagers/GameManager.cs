using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtility.CustomAttributes;
using UnityUtility.SerializedDictionary;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<GameState, SubGameManager> m_subGameManagers;
    [SerializeField] private GameState m_startState;

    [SerializeField] private CinemachineBrain m_mainCamera;

    [Title("Inputs")]
    [SerializeField] private InputActionAsset m_actionAsset;
    [SerializeField] private string m_commonActionMapName = "Common";

    [NonSerialized] private SubGameManager m_currentSubGameManager;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        foreach (KeyValuePair<GameState, SubGameManager> manager in m_subGameManagers)
        {
            manager.Value.Init(m_actionAsset);
        }

        m_actionAsset.FindActionMap(m_commonActionMapName).Enable();

        m_currentSubGameManager = m_subGameManagers[m_startState];
        m_currentSubGameManager.BeginState(GameState.None);

        VirtualCamerasManager.RegisterBrain(m_mainCamera);
    }

    private void Update()
    {
        m_currentSubGameManager.UpdateState(Time.deltaTime);
        if (m_currentSubGameManager.RequestStateChange(out GameState nextState))
        {
            if (nextState != m_currentSubGameManager.CorrespondingState)
            {
                m_currentSubGameManager.EndState(nextState);
                m_currentSubGameManager = m_subGameManagers[nextState];
                m_currentSubGameManager.BeginState(nextState);
            }
        }
    }
}
