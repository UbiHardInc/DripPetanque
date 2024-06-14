using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtility.CustomAttributes;
using UnityUtility.SceneReference;
using UnityUtility.Utils;

using Random = UnityEngine.Random;

public class PetanqueSubGameManager : SubGameManager
{
    public override GameState CorrespondingState => GameState.Petanque;

    public event Action OnPetanqueSceneLoaded;
    public event Action ReactivateMainScene;

    public event Action<bool> OnBallLauched;
    public event Action<bool> OnNextTurn;
    public event Action OnNextRound;

    [SerializeField] private PetanqueSceneDatas m_petanqueSceneDatas;

    [SerializeField] private SceneTransitioner m_petanqueSceneLoader;
    [SerializeField] private ResultDisplay m_resultDisplay;
    [SerializeField] private BonusTutoDisplay m_bonusTutoDisplay;
    [SerializeField] private TurnChangeDisplay m_turnChangeDisplay;
    [SerializeField] private ScoreDisplay m_scoreDisplayUI;
    [SerializeField] private GameObject m_invertedRulesText;

    [Title("Crowd Reaction")]
    [SerializeField] private float m_crowdReactionProbability = 0.25f;
    [SerializeField] private float m_reactionSoundDelay = 1.0f;

    [Title("Default values")]
    [SerializeField] private PetanqueGameSettings m_defaultGameSettings;

    [Title("Cheats")]
    [SerializeField] private InputActionReference m_winGameInput;

    // Thrown balls
    [NonSerialized] private readonly List<Ball> m_allBalls = new List<Ball>();

    [NonSerialized] private BasePetanquePlayer m_currentPlayer;
    [NonSerialized] private int m_currentRound;

    [NonSerialized] private PetanquePlayerType m_winningPlayerType;

    // Petanque Scene's data
    [NonSerialized] private PetanqueGameSettings m_gameSettings;
    [NonSerialized] private List<BasePetanquePlayer> m_players;
    [NonSerialized] private Transform m_jack;

    [NonSerialized] private bool m_invertDistances;
    [NonSerialized] private Comparison<float> m_currentDistanceComparer;

    [NonSerialized] private bool m_bonusTutoHasBeenDisplayed = false;

    public override void BeginState(GameState previousState)
    {
        base.BeginState(previousState);

        if (previousState == GameState.MainMenu)
        {
            m_bonusTutoHasBeenDisplayed = false;
        }

        m_petanqueSceneDatas.OnDatasFilled += OnPetanqueDatasFilled;

        m_gameSettings = GetGameSettings();
        m_sharedDatas.NextPetanqueGameSettings = null;
        m_petanqueSceneLoader.SetScene(m_gameSettings.PetanqueScene);

        m_petanqueSceneLoader.StartLoadTransition(fadeIn: true, fadeOut: false);
        m_petanqueSceneLoader.OnTransitionEnd += OnSceneLoaded;

    }

    private void OnWinGameInputPerformed(InputAction.CallbackContext context)
    {
        UnloadScene();
    }

    public void InvertDistances()
    {
        m_invertDistances = !m_invertDistances;
        m_invertedRulesText.SetActive(m_invertDistances);
        m_currentDistanceComparer = GetDistanceComparison(m_invertDistances);
    }

    public void DisplayScoringBalls(bool display)
    {
        m_allBalls.ForEach(ball => ball.SetHaloActive(display));
    }

    private PetanqueGameSettings GetGameSettings()
    {
        if (m_sharedDatas.NextPetanqueGameSettings == null)
        {
            Debug.LogError("No PetanqueGameSettings in the shared datas");
            return m_defaultGameSettings;
        }
        return m_sharedDatas.NextPetanqueGameSettings;
    }

    private void OnSceneLoaded(SceneReference reference)
    {
        m_petanqueSceneLoader.OnTransitionEnd -= OnSceneLoaded;
        OnPetanqueSceneLoaded?.Invoke();
    }

    private void OnPetanqueDatasFilled()
    {
        m_petanqueSceneDatas.OnDatasFilled -= OnPetanqueDatasFilled;

        m_jack = m_petanqueSceneDatas.Field.JackPosition;
        m_players = m_petanqueSceneDatas.PetanquePlayers;
        //activate score ui
        m_scoreDisplayUI.ActivateScoreDisplay(m_players);

        StartPetanque(m_petanqueSceneDatas.Field);
    }

    #region Petanque GameLoop
    private void StartPetanque(PetanqueField field)
    {
        m_winGameInput.action.performed += OnWinGameInputPerformed;

        foreach (BasePetanquePlayer player in m_players)
        {
            player.Init();
            player.OnBallThrown += OnPlayerThownBall;
        }
        m_currentRound = 0;
        m_jack.position = field.JackPosition.position;

        ResetGame();
        if (!m_bonusTutoHasBeenDisplayed && !m_gameSettings.IsTutorial)
        {
            DisplayBonusTuto();
        }
        else
        {
            StartRound(); 
        }
        
    }

    private void ResetGame()
    {
        m_players.ForEach(player => player.ResetForGame());
        ResetRound();

        m_resultDisplay.ResetPanel();
        m_scoreDisplayUI.ResetPanel();
    }

    private void StartRound()
    {
        ResetRound();
        m_currentRound++;
        OnNextRound?.Invoke();
        NextTurn();
    }

    private void OnPlayerThownBall(Ball ball)
    {
        m_allBalls.Add(ball);

        OnBallLauched?.Invoke(false);
    }

    private void NextTurn()
    {
        if (m_currentPlayer != null)
        {
            m_currentPlayer.OnShootOver -= OnPlayerShootOver;
        }

        if (m_players.Sum(player => player.ThownBallsCount) == m_players.Count * m_gameSettings.BallsPerRounds)
        {
            EndRound();
            return;
        }

        OnNextTurn?.Invoke(true);

        BasePetanquePlayer latestPlayer = m_currentPlayer;
        m_currentPlayer = ComputeNextTurnPlayer();

        if (m_currentPlayer != latestPlayer)
        {
            m_turnChangeDisplay.DisplayTurn(m_currentPlayer, OnDisplayTurnOver);
            return;
        }
        OnDisplayTurnOver();
    }

    private void OnDisplayTurnOver()
    {
        m_currentPlayer.OnShootOver += OnPlayerShootOver;
        m_currentPlayer.StartShoot();
    }

    private void OnPlayerShootOver(Ball thrownBall)
    {
        if (PlayReactionSound(thrownBall))
        {
            _ = StartCoroutine(DelayCoroutine(m_reactionSoundDelay, NextTurn));
        }
        else
        {
            NextTurn();
        }
    }

    private void EndRound()
    {
        BasePetanquePlayer roundWinner = ComputeRoundResults();

        if (roundWinner.CurrentScore >= m_gameSettings.PointsToWin)
        {
            DisplayGameResult(roundWinner);
            return;
        }

        DisplayRoundResults(roundWinner);

        if (roundWinner.CurrentScore >= Math.Round(m_gameSettings.PointsToWin * 0.7))
        {
            SoundManager.Instance.SwitchIntenseBattleMusic();
        }
    }

    private BasePetanquePlayer ComputeRoundResults()
    {
        (List<Ball> scoringBalls, BasePetanquePlayer roundWinner) = GetScoringBalls(BallDistanceComparison);

        int roundScore = 0;
        foreach (Ball ball in scoringBalls)
        {
            roundScore += ball.GetBallScore();
        }

        roundWinner.AddRoundResult(new RoundResult(roundScore));

        foreach (BasePetanquePlayer player in m_players)
        {
            if (player == roundWinner)
            {
                continue;
            }
            player.AddRoundResult(new RoundResult(0));
        }
        return roundWinner;
    }

    private void DisplayRoundResults(BasePetanquePlayer roundWinner)
    {
        RoundResultDatas result = new RoundResultDatas()
        {
            Winner = roundWinner,
            RoundIndex = m_currentRound,
            AllPlayers = m_players,
        };

        m_resultDisplay.DislayRoundResult(result, StartRound);
        m_scoreDisplayUI.UpdateScore(result);
    }

    private void DisplayGameResult(BasePetanquePlayer gameWinner)
    {
        Debug.LogError($"{gameWinner.PlayerName} won the game with {gameWinner.CurrentScore} points");

        m_winningPlayerType = gameWinner.PlayerType;

        GameResultDatas result = new GameResultDatas()
        {
            Winner = gameWinner,
            AllPlayers = m_players,
            RoundCount = m_currentRound,
        };

        m_resultDisplay.DisplayGameResult(result, UnloadScene);
    }

    private void DisplayBonusTuto()
    {
        m_bonusTutoDisplay.DislayBonusTuto(StartRound);
        m_bonusTutoHasBeenDisplayed = true;
    }

    private void UnloadScene()
    {
        m_petanqueSceneLoader.StartUnloadTransition(fadeIn: true, fadeOut: true);

        m_players.ForEach(player => player.Dispose());

        m_scoreDisplayUI.DeactivateScoreUI();

        m_petanqueSceneLoader.OnFadeInOver += EndPetanqueState;
    }

    private void EndPetanqueState()
    {
        ResetRound();

        m_petanqueSceneLoader.OnFadeInOver -= EndPetanqueState;
        ReactivateMainScene?.Invoke();

        m_winGameInput.action.performed += OnWinGameInputPerformed;

        m_requestedGameState = m_gameSettings.GetNextStateDatas(m_winningPlayerType).ApplyDatas(m_sharedDatas);
    }

    private void ResetRound()
    {
        m_players.ForEach(player => player.ResetForRound());
        m_currentPlayer = null;
        m_allBalls.Clear();

        m_invertDistances = false;
        m_invertedRulesText.SetActive(false);
        m_currentDistanceComparer = GetDistanceComparison(m_invertDistances);
    }

    private BasePetanquePlayer ComputeNextTurnPlayer()
    {
        float furthestClosestBallDistance = GetBound(m_invertDistances);
        Comparison<float> distanceComparison = GetDistanceComparison(m_invertDistances);

        BasePetanquePlayer nextPlayer = null;
        foreach (BasePetanquePlayer player in m_players)
        {
            if (player.ThownBallsCount == 0)
            {
                return player;
            }

            if (player.ThownBallsCount >= m_gameSettings.BallsPerRounds)
            {
                continue;
            }

            float closestBallDistance = GetBallSqrDistanceToJack(player.GetClosestBall(BallDistanceComparison));
            if (distanceComparison(furthestClosestBallDistance, closestBallDistance) < 0)
            {
                nextPlayer = player;
                furthestClosestBallDistance = closestBallDistance;
            }

            NotifyScoringBalls();
        }
        return nextPlayer;
    }

    private bool PlayReactionSound(Ball ball)
    {
        if (m_allBalls.Count <= m_players.Count)
        {
            return false;
        }

        if (Random.value > m_crowdReactionProbability)
        {
            return false;
        }

        int ballRank = GetBallRank(ball, BallDistanceComparison);

        if (ballRank == 0)
        {
            SoundManager.Instance.PlayBallSounds(SoundManager.BallSFXType.good);
            return true;
        }

        if (ballRank == m_allBalls.Count - 1)
        {
            SoundManager.Instance.PlayBallSounds(SoundManager.BallSFXType.bad);
            return true;
        }
        return false;
    }

    private int GetBallRank(Ball ball, Comparison<Ball> ballsComparer)
    {
        if (!m_allBalls.Contains(ball))
        {
            Debug.LogError("The given ball is not in all the balls");
            return -1;
        }

        Ball[] sortedBalls = m_allBalls.SortCopy(ballsComparer);
        return sortedBalls.IndexOf(ball);
    }

    private void NotifyScoringBalls()
    {
        m_allBalls.ForEach(b => b.BallScores = false);
        (List<Ball> scoringBalls, _) = GetScoringBalls(BallDistanceComparison);
        scoringBalls.ForEach(b => b.BallScores = true);
    }

    private (List<Ball> scoringBalls, BasePetanquePlayer ballsOwner) GetScoringBalls(Comparison<Ball> ballsComparer)
    {
        int ballsCount = m_allBalls.Count;
        List<Ball> scoringBalls = new List<Ball>();
        if (ballsCount == 0)
        {
            return (scoringBalls, null);
        }

        Ball[] sortedBalls = m_allBalls.SortCopy(ballsComparer);

        BasePetanquePlayer ballsOwner = sortedBalls[0].BallOwner;
        int ballIndex = 0;
        while (ballIndex < ballsCount && sortedBalls[ballIndex].BallOwner == ballsOwner)
        {
            scoringBalls.Add(sortedBalls[ballIndex++]);
        }

        return (scoringBalls, ballsOwner);
    }

    private int BallDistanceComparison(Ball b0, Ball b1)
    {
        float b0Dist = GetBallSqrDistanceToJack(b0);
        float b1Dist = GetBallSqrDistanceToJack(b1);
        return m_currentDistanceComparer(b0Dist, b1Dist);
    }

    private static Comparison<float> GetDistanceComparison(bool invertDistances)
    {
        return invertDistances ? BiggerFirst : SmallerFirst;
    }
    private static float GetBound(bool invertDistances)
    {
        return invertDistances ? float.MaxValue : float.MinValue;
    }

    private static int SmallerFirst(float f0, float f1)
    {
        return f0.CompareTo(f1);
    }

    private static int BiggerFirst(float f0, float f1)
    {
        return -SmallerFirst(f0, f1);
    }

    private float GetBallSqrDistanceToJack(Ball b0)
    {
        return Vector3Utils.SqrDistance(b0.transform.position, m_jack.position);
    }

    public static IEnumerator DelayCoroutine(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback();
    }
    #endregion
}
