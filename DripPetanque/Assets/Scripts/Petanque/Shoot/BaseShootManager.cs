using System;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Pools;
using UnityUtility.Utils;

public abstract class BaseShootManager<TShootStep, TBall> : MonoBehaviour
    where TShootStep : BaseShootStep
    where TBall : Ball
{
    protected enum ShootState
    {
        NotStarted = 0,
        Steps = 1,
        LaunchBall = 2,
        Finished = 3,
    }

    protected abstract PetanqueSubGameManager.PetanquePlayers Owner { get; }

    public event Action<PooledObject<TBall>> OnBallSpawned;

    [Title("Shoot Steps")]
    [SerializeField] protected TShootStep m_leftRightStep;
    [Separator]

    [Label(bold: true)]
    [SerializeField] protected TShootStep m_forceStep;
    [Separator]

    [Label(bold: true)]
    [SerializeField] protected TShootStep m_upDownStep;
    [Separator]

    [SerializeField] protected CustomSplineController m_splineController;
    [SerializeField] private BallTrajectoryController m_trajectoryController;

    [SerializeField] private CallbackRecieverComponentPool<TBall> m_ballsPool;

    [NonSerialized] protected ShootState m_currentState;

    [NonSerialized] protected TShootStep[] m_allSteps;

    [NonSerialized] protected int m_currentStep = 0;


    public virtual void Init()
    {
        m_allSteps = new TShootStep[]
        {
            m_leftRightStep,
            m_upDownStep,
            m_forceStep,
        };
    }

    protected virtual void Update()
    {
        switch (m_currentState)
        {
            case ShootState.NotStarted:
                break;
            case ShootState.Steps:
                UpdateSteps(Time.deltaTime);
                break;
            case ShootState.LaunchBall:
                break;
            case ShootState.Finished:
                break;
            default:
                break;
        }
    }

    public void StartShoot()
    {
        StartSteps();
    }

    protected virtual void StartSteps()
    {
        m_currentStep = 0;
        m_allSteps[m_currentStep].Start();
        m_currentState = ShootState.Steps;

        if (Owner == PetanqueSubGameManager.PetanquePlayers.Human)
        {
            _ = StartCoroutine(SoundManager.Instance.SwitchBattleMusic(SoundManager.BattleFilters.Low));
        }
    }

    private void UpdateSteps(float deltaTime)
    {
        if (m_allSteps[m_currentStep].IsFinished())
        {
            m_currentStep++;
            if (m_currentStep >= m_allSteps.Length)
            {
                m_splineController.SetSplineParameters(m_leftRightStep.StepOutputValue, m_upDownStep.StepOutputValue, m_forceStep.StepOutputValue);
                LaunchBall();
                return;
            }
            Debug.Log($"{Owner} starts step {m_currentStep} at frame {Time.frameCount}");
            m_allSteps[m_currentStep].Start();
        }
        m_allSteps[m_currentStep].Update(deltaTime);
        m_splineController.SetSplineParameters(m_leftRightStep.StepOutputValue, m_upDownStep.StepOutputValue, m_forceStep.StepOutputValue);
    }

    protected virtual void LaunchBall()
    {
        m_currentState = ShootState.LaunchBall;

        Debug.LogWarning($"Left-Right : {m_leftRightStep.StepOutputValue}");
        Debug.LogWarning($"Force : {m_forceStep.StepOutputValue}");
        Debug.LogWarning($"Up-Down : {m_upDownStep.StepOutputValue}");
        m_splineController.SetSplineParameters(m_leftRightStep.StepOutputValue, m_upDownStep.StepOutputValue, m_forceStep.StepOutputValue);

        PooledObject<TBall> requestedBall = m_ballsPool.Request();
        requestedBall.Object.BallOwner = Owner;
        requestedBall.Object.OnBallStopped += OnBallStopped;
        OnBallSpawned?.Invoke(requestedBall);

        m_trajectoryController.StartNewBall(requestedBall.Object);

        if (Owner == PetanqueSubGameManager.PetanquePlayers.Human)
        {
            _ = StartCoroutine(SoundManager.Instance.SwitchBattleMusic(SoundManager.BattleFilters.None));
        }
        _ = StartCoroutine(SoundManager.Instance.PlayBallSounds(SoundManager.BallSFXType.swoop));
    }

    private void OnBallStopped(Ball ball)
    {
        ball.OnBallStopped -= OnBallStopped;
        m_currentState = ShootState.Finished;
    }

    public void Dispose()
    {
        m_allSteps.ForEach(step => step.Dispose());
    }
}
