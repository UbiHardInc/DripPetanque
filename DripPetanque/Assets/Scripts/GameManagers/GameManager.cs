using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.SerializedDictionary;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<GameState, ISubGameManager> m_subGameManagers;
    [SerializeField] private GameState m_startState;

    [NonSerialized] private ISubGameManager m_currentSubGameManager;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        foreach (KeyValuePair<GameState, ISubGameManager> manager in m_subGameManagers)
        {
            manager.Value.Init();
        }

        m_currentSubGameManager = m_subGameManagers[m_startState];
        m_currentSubGameManager.BeginState(GameState.None);
    }

    private void Update()
    {
        m_currentSubGameManager.UpdateState(Time.deltaTime);
        if (m_currentSubGameManager.RequestStateChange(out GameState nextState))
        {
            if (nextState != m_currentSubGameManager.CorrespondingState)
            {
                m_currentSubGameManager.EndState(nextState);
                m_currentSubGameManager = m_subGameManagers[m_startState];
                m_currentSubGameManager.BeginState(nextState);
            }
        }
    }
}
