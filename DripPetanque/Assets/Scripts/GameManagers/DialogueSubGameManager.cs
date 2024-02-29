using System;
using UnityEngine;

public class DialogueSubGameManager : MonoBehaviour, ISubGameManager
{
    public GameState CorrespondingState => GameState.Dialogue;

    [NonSerialized] private GameState m_requestedGameState = GameState.None;

    public void Init()
    {
    }

    public void BeginState(GameState previousState)
    {
        m_requestedGameState = GameState.None;
    }

    public void EndState(GameState nextState)
    {
        m_requestedGameState = GameState.None;
    }

    public bool RequestStateChange(out GameState nextState)
    {
        nextState = m_requestedGameState;
        return m_requestedGameState != GameState.None;
    }

    public void UpdateState(float deltaTime)
    {
    }
}
