using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtility.CustomAttributes;
using UnityUtility.Pools;

public class HumanShootManager : BaseShootManager<HumanShootStep, ControllableBall>
{
    [SerializeField] private Transform m_arrow;
    [SerializeField] private Transform m_arrowPivot;
    [SerializeField] private GameObject m_controllerUI;

    [Title("Inputs")]
    [SerializeField] private InputActionReference m_startShootInput;
    [SerializeField] private InputActionReference m_zenithalViewInput;

    [Title("Zenithal View")]
    [SerializeField] private CinemachineVirtualCamera m_zenithalCamera;

    // Zenithal View
    [NonSerialized] private bool m_zenithalViewEnabled = false;

    public override void Init(BasePetanquePlayer owner)
    {
        base.Init(owner);

        m_arrow.gameObject.SetActive(false);

        m_leftRightStep.Init(m_arrowPivot);
        m_forceStep.Init(m_arrow);
        m_upDownStep.Init(m_arrowPivot);

        VirtualCamerasManager.RegisterCamera(m_zenithalCamera);

    }

    public void StartShoot(InputAction.CallbackContext _)
    {
        StartShoot();
    }

    protected override void StartSteps()
    {
        base.StartSteps();

        m_arrow.gameObject.SetActive(true);

        SoundManager.Instance.SwitchBattleFilterMusic(SoundManager.BattleFilters.Low);

        m_zenithalViewEnabled = false;
        if (m_zenithalViewInput.action.ReadValue<float>() > 0.9f)
        {
            ToggleZenithalView();
        }

        m_zenithalViewInput.action.performed += OnZenithalViewPerformed;
    }

    protected override PooledObject<ControllableBall> LaunchBall()
    {
        PooledObject<ControllableBall> launchedBall = base.LaunchBall();

        launchedBall.Object.OnBallControllable += OnLaunchedBallControllable;

        m_arrow.gameObject.SetActive(false);
        for (int i = m_allSteps.Length - 1; i >= 0; i--)
        {
            m_allSteps[i].ResetArrow();
        }

        VirtualCamerasManager.SwitchToCamera(m_pintanqueOverviewCam);
        m_zenithalViewInput.action.performed -= OnZenithalViewPerformed;

        SoundManager.Instance.SwitchBattleFilterMusic(SoundManager.BattleFilters.None);

        return launchedBall;
    }

    private void OnLaunchedBallControllable(ControllableBall ball)
    {
        ball.OnBallControllable -= OnLaunchedBallControllable;
        m_controllerUI.SetActive(true);
    }

    protected override void OnBallStopped(Ball ball)
    {
        base.OnBallStopped(ball);
        m_controllerUI.SetActive(false);
    }

    public override void Dispose()
    {
        base.Dispose();
    }

    private void OnZenithalViewPerformed(InputAction.CallbackContext context)
    {
        ToggleZenithalView();
    }

    private void ToggleZenithalView()
    {
        m_zenithalViewEnabled ^= true;

        CinemachineVirtualCamera cameraToSwitchTo = m_zenithalViewEnabled ? m_zenithalCamera : m_allSteps[m_currentStep].CameraPosition;

        VirtualCamerasManager.SwitchToCamera(cameraToSwitchTo);
    }
}
