using Cinemachine;
using System;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityUtility.Utils;

using Random = UnityEngine.Random;

using static ShootStepData;

[Serializable]
public class HumanShootStep : BaseShootStep
{
    private static readonly Vector2 s_gaugeRange = new Vector2(0, 1);

    private enum StepState
    {
        NotStarted = 0,
        MovingCamera = 1,
        Gauge = 2,
        Finished = 3,
    }

    [SerializeField] private SlidingGauge m_gauge;

    [SerializeField] private CinemachineVirtualCamera m_cameraPosition;
    [SerializeField] private float m_camTransitionTime;

    [SerializeField] private InputActionReference m_validateInput;
    [SerializeField] private InfoPanel m_infoPanel;

    [SerializeField] private float m_cheersProbability = 0.0f;

    // Cache
    [NonSerialized] private StepState m_currentState;
    [NonSerialized] private Transform m_arrow;

    [NonSerialized] private Vector3 m_startScaleOrRotation;

    public void Init(Transform arrow)
    {
        m_currentState = StepState.NotStarted;
        m_arrow = arrow;
        VirtualCamerasManager.RegisterCamera(m_cameraPosition);
        m_gauge.transform.parent.gameObject.SetActive(false);
        m_infoPanel.Activate(false);
    }

    public override void Start()
    {
        m_currentState = StepState.MovingCamera;
        VirtualCamerasManager.SwitchToCamera(m_cameraPosition, m_camTransitionTime);
        m_startScaleOrRotation = m_data.ScaleOrRotation == ScaleOrRotationEnum.Scale ? m_arrow.localScale : m_arrow.localRotation.eulerAngles;
        m_stepOutputValue = 0.0f;
        m_infoPanel.SetText(m_data.InfoPanelMessage);
    }

    public override void Update(float deltaTime)
    {
        switch (m_currentState)
        {
            case StepState.NotStarted:
                Debug.LogWarning($"{nameof(Update)} should not be called on a {nameof(HumanShootStep)} that did not start");
                break;
            case StepState.MovingCamera:
                MoveCamera(deltaTime);
                break;
            case StepState.Gauge:
                UpdateGauge(deltaTime);
                break;
            case StepState.Finished:
                Debug.LogWarning($"{nameof(Update)} should no longer be called on a {nameof(HumanShootStep)} that already finished");
                break;
            default:
                break;
        }
    }

    public override bool IsFinished()
    {
        return m_currentState == StepState.Finished;
    }

    public void ResetArrow()
    {
        switch (m_data.ScaleOrRotation)
        {
            case ScaleOrRotationEnum.Scale:
                m_arrow.localScale = m_startScaleOrRotation;
                break;
            case ScaleOrRotationEnum.Rotation:
                m_arrow.localRotation = Quaternion.Euler(m_startScaleOrRotation);
                break;
            default:
                break;
        }
    }

    public override void Dispose()
    {
        base.Dispose();

        VirtualCamerasManager.UnRegisterCamera(m_cameraPosition);
    }

    public override void Reset()
    {
        base.Reset();
    }

    private void MoveCamera(float deltaTime)
    {
        if (VirtualCamerasManager.IsBrainMoving())
        {            
            //float lerpFactor = Mathf.SmoothStep(0, 1, m_camTransitionTimer / m_camTransitionTime);
            //m_camera.transform.position = Vector3.Lerp(m_camStartPosition, m_cameraPosition.transform.position, lerpFactor);
            //m_camera.transform.rotation = Quaternion.Lerp(m_camStartRotation, m_cameraPosition.transform.rotation, lerpFactor);
            //m_camTransitionTimer += deltaTime;
        }
        else
        {
            StartGauge();
        }
    }

    private void StartGauge()
    {
        m_currentState = StepState.Gauge;
        m_gauge.FillingSpeed = m_data.GaugeSpeed;
        m_gauge.FillingBehaviour = m_data.FillingBehaviour;

        m_gauge.transform.parent.gameObject.SetActive(true);
        m_infoPanel.Activate(true);

        m_validateInput.action.performed += OnValidateInput;

        if (Random.value < m_cheersProbability)
        {
            SoundManager.Instance.PlayBallSounds(SoundManager.BallSFXType.cheer);
        }
    }

    private void OnValidateInput(InputAction.CallbackContext context)
    {
        m_validateInput.action.performed -= OnValidateInput;
        m_gauge.transform.parent.gameObject.SetActive(false);
        m_infoPanel.Activate(false);
        m_currentState = StepState.Finished;

        SoundManager.Instance.PlayUISFX("submit");
    }

    private void UpdateGauge(float deltaTime)
    {
        m_gauge.UpdateGauge(deltaTime);
        float gaugeValue = m_gauge.CurrentFilling.Remap(s_gaugeRange, m_data.Range);

        switch (m_data.ScaleOrRotation)
        {
            case ScaleOrRotationEnum.Scale:
                Vector3 newScale = Vector3.Scale(m_startScaleOrRotation, m_data.Axis.ToVector() * gaugeValue) + Vector3.Scale(m_startScaleOrRotation, (~m_data.Axis).ToVector());
                m_arrow.localScale = newScale;
                break;
            case ScaleOrRotationEnum.Rotation:
                Vector3 axisVal = m_data.Axis.ToVector() * gaugeValue;
                Vector3 newRotation = m_startScaleOrRotation + axisVal;
                m_arrow.localRotation = Quaternion.Euler(newRotation);
                break;
            default:
                break;
        }

        m_stepOutputValue = gaugeValue;
    }

    public override bool HasTempValue()
    {
        return m_currentState >= StepState.Gauge;
    }
}