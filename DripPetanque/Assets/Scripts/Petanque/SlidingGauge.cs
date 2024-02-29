using System;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility.CustomAttributes;
using static UnityEngine.UI.Image;

public class SlidingGauge : MonoBehaviour
{
    public enum FillingBehaviourEnum
    {
        Forth,
        BackAndForth,
    }

    public float CurrentFilling => m_currentFilling;

    public float FillingSpeed { get => m_fillingSpeed; set => m_fillingSpeed = value; }
    public FillingBehaviourEnum FillingBehaviour { get => m_fillingBehaviour; set => m_fillingBehaviour = value; }

    [SerializeField] private Image m_slider = null;
    [SerializeField] private float m_fillingSpeed = 0.0f;
    [SerializeField] private FillingBehaviourEnum m_fillingBehaviour = FillingBehaviourEnum.BackAndForth;
    [SerializeField] private FillMethod m_fillMethod = FillMethod.Vertical;

    [SerializeField, ShowIf(nameof(m_fillMethod), FillMethod.Horizontal)]
    private OriginHorizontal m_horizontalOrigin = OriginHorizontal.Right;
    [SerializeField, ShowIf(nameof(m_fillMethod), FillMethod.Vertical)]
    private OriginVertical m_verticalOrigin = OriginVertical.Bottom;
    [SerializeField, ShowIf(nameof(m_fillMethod), FillMethod.Radial90)]
    private Origin90 m_origin90 = Origin90.BottomLeft;
    [SerializeField, ShowIf(nameof(m_fillMethod), FillMethod.Radial180)]
    private Origin180 m_origin180 = Origin180.Bottom;
    [SerializeField, ShowIf(nameof(m_fillMethod), FillMethod.Radial360)]
    private Origin360 m_origin360 = Origin360.Bottom;

    [NonSerialized] private float m_currentFilling = 0.0f;
    [NonSerialized] private float m_inernalCurrentFilling = 0.0f;

    private void Start()
    {
        m_slider.type = Image.Type.Filled;
        switch (m_fillMethod)
        {
            case FillMethod.Horizontal:
                SetHorizontalFillMethodAndOrigin(m_horizontalOrigin);
                break;
            case FillMethod.Vertical:
                SetVerticalFillMethodAndOrigin(m_verticalOrigin);
                break;
            case FillMethod.Radial90:
                SetRadial90FillMethodAndOrigin(m_origin90);
                break;
            case FillMethod.Radial180:
                SetRadial180FillMethodAndOrigin(m_origin180);
                break;
            case FillMethod.Radial360:
                SetRadial360FillMethodAndOrigin(m_origin360);
                break;
        }
    }

    // Update is called once per frame
    public void UpdateGauge(float deltaTime)
    {
        m_inernalCurrentFilling += deltaTime * m_fillingSpeed;
        switch (m_fillingBehaviour)
        {
            case FillingBehaviourEnum.Forth:
                m_currentFilling = (m_inernalCurrentFilling + 1) % 1;
                break;
            case FillingBehaviourEnum.BackAndForth:
                m_currentFilling = Mathf.Abs((m_inernalCurrentFilling % 2) - 1);
                break;
            default:
                break;
        }

        m_slider.fillAmount = m_currentFilling;
    }

    public void SetHorizontalFillMethodAndOrigin(OriginHorizontal origin)
    {
        SetFillMethodAndOrigin(FillMethod.Horizontal, (int)origin);
    }

    public void SetVerticalFillMethodAndOrigin(OriginVertical origin)
    {
        SetFillMethodAndOrigin(FillMethod.Vertical, (int)origin);
    }

    public void SetRadial90FillMethodAndOrigin(Origin90 origin)
    {
        SetFillMethodAndOrigin(FillMethod.Radial90, (int)origin);
    }

    public void SetRadial180FillMethodAndOrigin(Origin180 origin)
    {
        SetFillMethodAndOrigin(FillMethod.Radial180, (int)origin);
    }

    public void SetRadial360FillMethodAndOrigin(Origin360 origin)
    {
        SetFillMethodAndOrigin(FillMethod.Radial360, (int)origin);
    }

    public void SetFillMethodAndOrigin(FillMethod fillMethod, int origin)
    {
        m_slider.fillMethod = fillMethod;
        m_slider.fillOrigin = origin;
    }

}