using Cinemachine;
using System;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityUtility.Utils;
using static ShootStepData;

[Serializable]
public class ShootStep : BaseShootStep
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

    [NonSerialized] private StepState m_currentState;
    [NonSerialized] private Transform m_arrow;
    [NonSerialized] private Camera m_camera;

    [NonSerialized] private Vector3 m_startScaleOrRotation;

    [NonSerialized] private Vector3 m_camStartPosition;
    [NonSerialized] private Quaternion m_camStartRotation;
    [NonSerialized] private float m_camTransitionTimer;

    public void Init(Transform arrow, Camera camera)
    {
        m_currentState = StepState.NotStarted;
        m_arrow = arrow;
        m_camera = camera;
        VirtualCamerasManager.RegisterCamera(m_cameraPosition);
    }

    public override void Start()
    {
        m_currentState = StepState.MovingCamera;
        VirtualCamerasManager.SwitchToCamera(m_cameraPosition, m_camTransitionTime);
        m_startScaleOrRotation = m_data.ScaleOrRotation == ScaleOrRotationEnum.Scale ? m_arrow.localScale : m_arrow.localRotation.eulerAngles;
        m_camStartPosition = m_camera.transform.position;
        m_camStartRotation = m_camera.transform.rotation;
        m_camTransitionTimer = 0.0f;
        m_stepOutputValue = 0.0f;
    }

    public override void Update(float deltaTime)
    {
        switch (m_currentState)
        {
            case StepState.NotStarted:
                Debug.LogWarning($"{nameof(Update)} should not be called on a {nameof(ShootStep)} that did not start");
                break;
            case StepState.MovingCamera:
                MoveCamera(deltaTime);
                break;
            case StepState.Gauge:
                UpdateGauge(deltaTime);
                break;
            case StepState.Finished:
                Debug.LogWarning($"{nameof(Update)} should no longer be called on a {nameof(ShootStep)} that already finished");
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
        }
    }

    private void MoveCamera(float deltaTime)
    {
        if (VirtualCamerasManager.IsBrainBlending())
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

        m_gauge.gameObject.SetActive(true);

        m_validateInput.action.performed += OnValidateInput;
    }

    private void OnValidateInput(InputAction.CallbackContext context)
    {
        m_validateInput.action.performed -= OnValidateInput;
        m_gauge.gameObject.SetActive(false);
        m_currentState = StepState.Finished;
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
        }

        m_stepOutputValue = gaugeValue;
    }
}