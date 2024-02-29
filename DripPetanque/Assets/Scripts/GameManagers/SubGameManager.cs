using System;
using UnityEngine;

public abstract class SubGameManager : MonoBehaviour, ISubGameManager
{
    public abstract GameState CorrespondingState { get; }

    [NonSerialized] protected GameState m_requestedGameState = GameState.None;

    public abstract void Init();

    public virtual void BeginState(GameState previousState)
    {
        m_requestedGameState = GameState.None;
    }

    public virtual void EndState(GameState nextState)
    {
        m_requestedGameState = GameState.None;
    }

    public virtual bool RequestStateChange(out GameState nextState)
    {
        nextState = m_requestedGameState;
        return m_requestedGameState != GameState.None;
    }

    public abstract void UpdateState(float deltaTime);
}
