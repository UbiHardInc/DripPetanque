using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityUtility.Pools;

public class PetanqueSubGameManager : SubGameManager
{
    private enum ShootTurn
    {
        Player,
        Computer,
    }

    public override GameState CorrespondingState => GameState.Petanque;

    [SerializeField] private PetanqueGameSettings m_gameSettings;
    [SerializeField] private ShootManager m_playerShootManager;
    [SerializeField] private PetanqueField m_field;

    [SerializeField] private Transform m_jack;

    [NonSerialized] private List<PooledObject<BallController>> m_playerBalls = new List<PooledObject<BallController>>();

    [NonSerialized] private ShootTurn m_shootTurn;
    [NonSerialized] private int m_playerThrownBalls = 0;
    [NonSerialized] private int m_computerThrownBalls = 0;

    public override void BeginState(GameState previousState)
    {
        base.BeginState(previousState);
        StartPetanque(m_field);
    }

    private void StartPetanque(PetanqueField field)
    {
        ResetPetanque();
        m_playerShootManager.OnBallSpawned += OnBallSpawned;
        m_jack.position = field.JackPosition.position;
        m_shootTurn = ShootTurn.Player;
    }

    private void ResetPetanque()
    {
        m_playerBalls.ForEach(ball => ball.Release());
        m_playerBalls.Clear();
        m_playerThrownBalls = 0;
        m_computerThrownBalls = 0;
    }

    private void OnBallSpawned(PooledObject<BallController> spawnedBall)
    {
        m_playerBalls.Add(spawnedBall);
        spawnedBall.Object.OnBallStopped += OnBallStopped;
    }

    private void OnBallStopped()
    {

    }

    private void PlayerShoot()
    {
        m_playerShootManager.StartShoot();
    }

    private void ComputerShoot()
    {

    }

    private ShootTurn ComputeNextTurn()
    {
        if (m_gameSettings.BallsPerGame <= m_computerThrownBalls)
        {
            return ShootTurn.Player;
        }
        if (m_gameSettings.BallsPerGame <= m_playerThrownBalls)
        {
            return ShootTurn.Computer;
        }
        return m_shootTurn == ShootTurn.Player ? ShootTurn.Player : ShootTurn.Computer;
    }
}
