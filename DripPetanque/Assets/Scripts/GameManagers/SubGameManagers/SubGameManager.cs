using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SubGameManager : MonoBehaviour
{
    public abstract GameState CorrespondingState { get; }

    [SerializeField] private string[] m_specificActionMapNames;

    #region Cache
    [NonSerialized] protected GameState m_requestedGameState = GameState.None;
    [NonSerialized] protected GameManagersSharedDatas m_sharedDatas;

    // Inputs
    [NonSerialized] protected InputActionAsset m_actionAsset = null;
    [NonSerialized] private IEnumerable<InputActionMap> m_specificActionMaps;
    #endregion

    public virtual void Initialize(InputActionAsset actionAsset, GameManagersSharedDatas sharedDatas)
    {
        m_actionAsset = actionAsset;
        m_sharedDatas = sharedDatas;
        m_specificActionMaps = m_specificActionMapNames.Select(name => m_actionAsset.FindActionMap(name));
    }

    public virtual void BeginState(GameState previousState)
    {
        m_requestedGameState = GameState.None;
        ActivateInput();
    }

    public virtual void EndState(GameState nextState)
    {
        m_requestedGameState = GameState.None;
        DeactivateInput();
    }

    public virtual bool RequestStateChange(out GameState nextState)
    {
        nextState = m_requestedGameState;
        return m_requestedGameState != GameState.None;
    }

    public void ActivateInput()
    {
        foreach (InputActionMap map in m_specificActionMaps)
        {
            map.Enable();
        }
    }

    public void DeactivateInput()
    {
        foreach (InputActionMap map in m_specificActionMaps)
        {
            map.Disable();
        }
    }

    public virtual void UpdateState(float deltaTime)
    {

    }
}
