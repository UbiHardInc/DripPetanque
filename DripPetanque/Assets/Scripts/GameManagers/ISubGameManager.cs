using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISubGameManager
{
    public GameState CorrespondingState { get; }

    void Init();
    void BeginState(GameState previousState);
    void UpdateState(float deltaTime);
    void EndState(GameState nextState);
    bool RequestStateChange(out GameState nextState);
}
