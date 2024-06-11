using System;
using UnityEngine;
using UnityUtility.CustomAttributes;

using Random = UnityEngine.Random;

public class MovingBonusRoulette : BonusRoulette
{
    [Title("Movement parameters")]
    [SerializeField] private Transform m_startPoint;
    [SerializeField] private Transform m_endPoint;

    [SerializeField] private float m_speed;

    [NonSerialized] private float m_currentProgress;

    protected virtual void Start()
    {
        m_currentProgress = Random.value;
    }

    protected override void Update()
    {
        base.Update();


        if (!m_ballLaunched)
        {
            m_currentProgress = (m_currentProgress + Time.deltaTime * m_speed) % 2;

            float lerpFactor = MathUtils.Smoothstep(Mathf.Abs(m_currentProgress - 1));
            transform.position = Vector3.Lerp(m_startPoint.position, m_endPoint.position, lerpFactor);
        }
    }

    private void OnDrawGizmos()
    {
        if (m_startPoint == null || m_endPoint == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(m_startPoint.position, m_endPoint.position);
    }
}
