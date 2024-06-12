using System;
using UnityEngine;

[Serializable]
public abstract class BasePetanquePlayer : MonoBehaviour
{
    // Events
    public abstract event Action<Ball> OnBallThrown;
    public abstract event Action<Ball> OnShootOver;

    public abstract PetanquePlayerType PlayerType { get; }
    public abstract string PlayerName { get; }
    public abstract int ThownBallsCount { get; }
    public abstract int CurrentScore { get; }


    public abstract void Init();
    public abstract void StartShoot();
    public abstract void ResetForRound();
    public abstract void ResetForGame();
    public abstract void Dispose();
    public abstract Ball GetClosestBall(Comparison<Ball> comparison);
    public abstract void AddRoundResult(RoundResult result);
    public abstract RoundResult GetResultForRound(int roundNumber);
}
