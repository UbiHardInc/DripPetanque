using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidingGauge : MonoBehaviour
{
    public enum FillingBehaviour
    {
        Forth,
        BackAndForth,
    }

    public float CurrentFilling => m_currentFilling;

    [SerializeField] private Image m_slider = null;
    [SerializeField] private float m_fillingSpeed = 0.0f;
    [SerializeField] private FillingBehaviour m_fillingBehaviour = FillingBehaviour.BackAndForth;

    [NonSerialized] private float m_currentFilling = 0.0f;
    [NonSerialized] private float m_inernalCurrentFilling = 0.0f;

    private void Start()
    {
        m_slider.type = Image.Type.Filled;
        m_slider.fillMethod = Image.FillMethod.Vertical;
        m_slider.fillOrigin = (int)Image.OriginVertical.Bottom;
    }

    // Update is called once per frame
    void Update()
    {
        m_inernalCurrentFilling += Time.deltaTime * m_fillingSpeed;
        switch (m_fillingBehaviour)
        {
            case FillingBehaviour.Forth:
                m_currentFilling = (m_inernalCurrentFilling + 1) % 1;
                break;
            case FillingBehaviour.BackAndForth:
                m_currentFilling = Mathf.Abs((m_inernalCurrentFilling % 2) - 1);
                break;
            default:
                break;
        }

        m_slider.fillAmount = m_currentFilling;
    }
}
