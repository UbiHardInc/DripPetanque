using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetanqueSubGameManager : MonoBehaviour, ISubGameManager
{
    public GameState CorrespondingState => GameState.Petanque;

    public void Init()
    {
    }

    public void BeginState(GameState previousState)
    {
    }

    public void EndState(GameState nextState)
    {
    }

    public bool RequestStateChange(out GameState nextState)
    {
        nextState = GameState.None;
        return false;
    }

    public void UpdateState(float deltaTime)
    {
    }
}
