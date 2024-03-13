using UnityEngine;
using UnityUtility.Utils;

[CreateAssetMenu(fileName = nameof(ShootStepData), menuName = "Scriptables/Petanque/" + nameof(ShootStepData))]
public class ShootStepData : ScriptableObject
{
    public enum ScaleOrRotationEnum
    {
        Scale = 0,
        Rotation = 1,
    }
    public SlidingGauge.FillingBehaviourEnum FillingBehaviour => m_fillingBehaviour;
    public float GaugeSpeed => m_gaugeSpeed;
    public Axis Axis => m_axis;
    public ScaleOrRotationEnum ScaleOrRotation => m_scaleOrRotation;
    public Vector2 Range => m_range;

    [SerializeField] private SlidingGauge.FillingBehaviourEnum m_fillingBehaviour;
    [SerializeField] private float m_gaugeSpeed;

    [SerializeField] private Axis m_axis;
    [SerializeField] private ScaleOrRotationEnum m_scaleOrRotation;
    [SerializeField] private Vector2 m_range;

}
