using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Pools;
using UnityUtility.Utils;

public class PetanqueSubGameManager : SubGameManager
{
    public enum PetanquePlayers
    {
        None = 0,
        Human = 1,
        Computer = 2,
    }

    public override GameState CorrespondingState => GameState.Petanque;

    [SerializeField] private PetanqueGameSettings m_gameSettings;
    [SerializeField] private ShootManager m_playerShootManager;
    [SerializeField] private ComputerShootManager m_computerShootManager;
    [SerializeField] private PetanqueField m_field;

    [SerializeField] private Transform m_jack;

    [NonSerialized] private List<PooledObject<ControllableBall>> m_playerBalls = new List<PooledObject<ControllableBall>>();
    [NonSerialized] private List<PooledObject<Ball>> m_computerBalls = new List<PooledObject<Ball>>();
    [NonSerialized] private List<Ball> m_allBalls = new List<Ball>();

    [NonSerialized] private PetanquePlayers m_shootTurn;
    [NonSerialized] private int m_playerThrownBalls = 0;
    [NonSerialized] private int m_computerThrownBalls = 0;

    public event Action<bool> OnBallLauched;
    public event Action<bool> OnNextTurn;

    public override void BeginState(GameState previousState)
    {
        base.BeginState(previousState);
        StartPetanque(m_field);
    }

    private void StartPetanque(PetanqueField field)
    {
        ResetPetanque();
        m_playerShootManager.OnBallSpawned += OnHumanBallSpawned;
        m_computerShootManager.OnBallSpawned += OnComputerBallSpawned;
        m_jack.position = field.JackPosition.position;
        m_shootTurn = PetanquePlayers.Human;
        NextTurn();
    }

    private void EndPetanque()
    {
        m_playerShootManager.OnBallSpawned -= OnHumanBallSpawned;
        m_computerShootManager.OnBallSpawned -= OnComputerBallSpawned;

        (List<Ball> closestBalls, PetanquePlayers ballsOwner) = GetClosestBallsFromJack();

        Debug.LogError($"{ballsOwner} won the game with {closestBalls.Count} points");
    }

    private void ResetPetanque()
    {
        m_playerBalls.ForEach(ball => ball.Release());
        m_playerBalls.Clear();
        m_playerThrownBalls = 0;

        m_computerBalls.ForEach(ball => ball.Release());
        m_computerBalls.Clear();
        m_computerThrownBalls = 0;

        m_allBalls.Clear();
    }

    private void OnHumanBallSpawned(PooledObject<ControllableBall> spawnedBall)
    {
        m_playerBalls.Add(spawnedBall);
        m_playerThrownBalls++;
        spawnedBall.Object.OnBallStopped += OnBallStopped;
        m_allBalls.Add(spawnedBall.Object);

        OnBallLauched?.Invoke(false);
    }

    private void OnComputerBallSpawned(PooledObject<Ball> spawnedBall)
    {
        m_computerBalls.Add(spawnedBall);
        m_computerThrownBalls++;
        spawnedBall.Object.OnBallStopped += OnBallStopped;
        m_allBalls.Add(spawnedBall.Object);

        OnBallLauched?.Invoke(false);
    }

    private void OnBallStopped(Ball ball)
    {
        ball.OnBallStopped -= OnBallStopped;
        NextTurn();
    }

    private void NextTurn()
    {
        if (m_computerThrownBalls + m_playerThrownBalls == 2 * m_gameSettings.BallsPerGame)
        {
            EndPetanque();
            return;
        }
        PetanquePlayers nextTurn = ComputeNextTurn();
        m_shootTurn = nextTurn == PetanquePlayers.None ? m_shootTurn : nextTurn;

        OnNextTurn?.Invoke(true);

        switch (m_shootTurn)
        {
            case PetanquePlayers.Human:
                HumanShoot();
                break;

            case PetanquePlayers.Computer:
                ComputerShoot();
                break;

            case PetanquePlayers.None:
                throw new Exception($"Invalid {nameof(m_shootTurn)} value");
        };
    }

    private void HumanShoot()
    {
        m_playerShootManager.StartShoot();
    }

    private void ComputerShoot()
    {
        m_computerShootManager.StartShoot();
    }

    private PetanquePlayers ComputeNextTurn()
    {
        if (m_gameSettings.BallsPerGame <= m_computerThrownBalls)
        {
            return PetanquePlayers.Human;
        }
        if (m_gameSettings.BallsPerGame <= m_playerThrownBalls)
        {
            return PetanquePlayers.Computer;
        }

        (List<Ball> _, PetanquePlayers closestBallsOwner) = GetClosestBallsFromJack();

        return closestBallsOwner switch
        {
            PetanquePlayers.None => PetanquePlayers.None,
            PetanquePlayers.Human => PetanquePlayers.Computer,
            PetanquePlayers.Computer => PetanquePlayers.Human,
            _ => throw new NotImplementedException(),
        };
    }

    private (List<Ball> closestBalls, PetanquePlayers ballsOwner) GetClosestBallsFromJack()
    {
        int ballsCount = m_allBalls.Count;
        List<Ball> closestBalls = new List<Ball>();
        if (ballsCount == 0)
        {
            return (closestBalls, PetanquePlayers.None);
        }
        
        m_allBalls.Sort(BallPositionComparison);

        PetanquePlayers ballsOwner = m_allBalls[0].BallOwner;
        int ballIndex = 0;
        while (ballIndex < ballsCount && m_allBalls[ballIndex].BallOwner == ballsOwner)
        {
            closestBalls.Add(m_allBalls[ballIndex++]);
        }

        return (closestBalls, ballsOwner);
    }

    private int BallPositionComparison(Ball b0, Ball b1)
    {
        float b0Dist = Vector3Utils.SqrDistance(b0.transform.position, m_jack.position);
        float b1Dist = Vector3Utils.SqrDistance(b1.transform.position, m_jack.position);
        return b0Dist.CompareTo(b1Dist);
    }
}
