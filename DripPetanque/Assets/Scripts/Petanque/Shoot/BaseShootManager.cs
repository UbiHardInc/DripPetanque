using System;
using Cinemachine;
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

    protected BasePetanquePlayer Owner { get; private set; }

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

    [Header("Cameras")]
    [SerializeField] protected CinemachineVirtualCamera m_pintanqueOverviewCam;
    [SerializeField] protected CinemachineVirtualCamera m_embutOverviewCam;

    [NonSerialized] protected ShootState m_currentState;

    [NonSerialized] protected TShootStep[] m_allSteps;

    [NonSerialized] protected int m_currentStep = 0;


    public virtual void Init(BasePetanquePlayer owner)
    {
        VirtualCamerasManager.RegisterCamera(m_pintanqueOverviewCam);
        VirtualCamerasManager.RegisterCamera(m_embutOverviewCam);

        m_allSteps = new TShootStep[]
        {
            m_leftRightStep,
            m_upDownStep,
            m_forceStep,
        };
        Owner = owner;
    }

    public void StartShoot()
    {
        StartSteps();
    }

    public virtual void UpdateShoot()
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

    public virtual void Reset()
    {
        m_allSteps.ForEach(step => step.Reset());
    }

    protected virtual void StartSteps()
    {
        m_currentStep = 0;
        m_allSteps[m_currentStep].Start();
        m_currentState = ShootState.Steps;
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

        SoundManager.Instance.PlayBallSounds(SoundManager.BallSFXType.swoop);
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
