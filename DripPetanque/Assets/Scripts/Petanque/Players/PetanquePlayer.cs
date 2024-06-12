using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtility.Pools;

[Serializable]
public abstract class PetanquePlayer<TShootStep, TBall> : BasePetanquePlayer
    where TShootStep : BaseShootStep
    where TBall : Ball
{
    public override event Action<Ball> OnBallThrown;
    public override event Action<Ball> OnShootOver;

    public override string PlayerName => m_playerName;
    public override int ThownBallsCount => m_thrownBallsCount;
    public override int CurrentScore => m_currentScore;


    [SerializeField] private BaseShootManager<TShootStep, TBall> m_shotManager;
    [SerializeField] private string m_playerName;

    // Thrown balls
    [NonSerialized] private readonly List<PooledObject<TBall>> m_playerBalls = new List<PooledObject<TBall>>();
    [NonSerialized] private int m_thrownBallsCount;

    [NonSerialized] private Ball m_lastThrownBall;

    // Results
    [NonSerialized] private readonly List<RoundResult> m_roundResults = new List<RoundResult>();
    [NonSerialized] private int m_currentScore;

    [NonSerialized] private bool m_shooting;

    public override void Init()
    {
        m_shotManager.Init(this);
        m_shotManager.OnBallSpawned += OnBallSpawned;
        m_lastThrownBall = null;
    }

    public override void StartShoot()
    {
        m_shooting = true;
        m_shotManager.StartShoot();
        m_lastThrownBall = null;
    }

    public override void ResetForRound()
    {
        m_playerBalls.ForEach(ball => ball.Release());
        m_playerBalls.Clear();
        m_thrownBallsCount = 0;

        m_shotManager.Reset();
    }

    public override void ResetForGame()
    {
        ResetForRound();
        m_roundResults.Clear();
        m_currentScore = 0;
    }

    public override void Dispose()
    {
        m_shotManager.Dispose();
    }

    public override void AddRoundResult(RoundResult result)
    {
        m_roundResults.Add(result);
        m_currentScore += result.Score;
    }

    public override RoundResult GetResultForRound(int roundNumber)
    {
        return m_roundResults[roundNumber - 1];
    }

    public override Ball GetClosestBall(Comparison<Ball> comparison)
    {
        if (m_thrownBallsCount == 0)
        {
            return null;
        }
        List<TBall> thrownBalls = m_playerBalls.Select(pooledBall => pooledBall.Object).ToList();
        thrownBalls.Sort(comparison);
        return thrownBalls[0];
    }

    protected virtual void Update()
    {
        if (!m_shooting)
        {
            return;
        }
        m_shotManager.UpdateShoot();
    }

    private void OnBallSpawned(PooledObject<TBall> spawnedBall)
    {
        m_playerBalls.Add(spawnedBall);
        m_thrownBallsCount++;
        spawnedBall.Object.OnBallStopped += OnBallStopped;

        m_lastThrownBall = spawnedBall.Object;

        OnBallThrown?.Invoke(spawnedBall.Object);
    }

    private void OnBallStopped(Ball ball)
    {
        ball.OnBallStopped -= OnBallStopped;
        m_shooting = false;
        OnShootOver?.Invoke(m_lastThrownBall);
        m_lastThrownBall = null;
    }
}
