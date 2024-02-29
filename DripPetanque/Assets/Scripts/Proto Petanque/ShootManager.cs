using System;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityUtility.CustomAttributes;

public class ShootManager : MonoBehaviour
{
    private enum ShootState
    {
        NotStarted = 0,
        Steps = 1,
        LaunchBall = 2,
        Finished = 3,
    }

    [Separator]

    [Label(bold: true)]
    [SerializeField] private ShootStep m_leftRightStep;
    [Separator]

    [Label(bold: true)]
    [SerializeField] private ShootStep m_forceStep;
    [Separator]

    [Label(bold: true)]
    [SerializeField] private ShootStep m_upDownStep;
    [Separator]

    [SerializeField] private Transform m_arrow;
    [SerializeField] private Transform m_arrowPivot;
    [SerializeField] private Camera m_camera;
    [SerializeField] private InputActionReference m_startShootInput;

    [SerializeField] private CustomSplineController m_splineController;

    [NonSerialized] private ShootStep[] m_allSteps;
    [NonSerialized] private int m_currentStep = 0;

    [NonSerialized] private Vector3 m_currentDirection;
    [NonSerialized] private ShootState m_currentState;

    private void Awake()
    {
        m_allSteps = new ShootStep[]
        {
            m_leftRightStep,
            m_upDownStep,
            m_forceStep,
        };

        m_leftRightStep.Init(m_arrowPivot, m_camera);
        m_forceStep.Init(m_arrow, m_camera);
        m_upDownStep.Init(m_arrowPivot, m_camera);

        m_startShootInput.action.performed += StartShoot;
    }

    private void StartShoot(InputAction.CallbackContext context)
    {
        StartSteps();
    }

    private void Update()
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
        }
    }

    private void StartSteps()
    {
        m_currentState = ShootState.Steps;
        m_currentStep = 0;
        m_allSteps[m_currentStep].Start();
    }

    private void UpdateSteps(float deltaTime)
    {
        if (m_allSteps[m_currentStep].IsFinished())
        {
            m_currentStep++;
            if (m_currentStep >= m_allSteps.Length)
            {
                LaunchBall();
                return;
            }
            m_allSteps[m_currentStep].Start();
        }
        m_allSteps[m_currentStep].Update(deltaTime);
        m_splineController.SetSplineParameters(m_leftRightStep.StepOutputValue, m_upDownStep.StepOutputValue, m_forceStep.StepOutputValue);
    }

    private void LaunchBall()
    {
        m_currentState = ShootState.LaunchBall;

        Debug.LogWarning($"Left-Right : {m_leftRightStep.StepOutputValue}");
        Debug.LogWarning($"Force : {m_forceStep.StepOutputValue}");
        Debug.LogWarning($"Up-Down : {m_upDownStep.StepOutputValue}");
    }
}
