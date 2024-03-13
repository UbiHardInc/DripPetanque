using System;
using UnityEngine;
using UnityEngine.Splines;

public class BallTrajectoryController : MonoBehaviour
{
    [SerializeField] private GameObject m_floor;
    [SerializeField] private SplineContainer m_spline;

    [SerializeField] private AnimationCurve m_speedAlongTheSpline = new AnimationCurve();
    [SerializeField] private float m_speedFactor = 0.001f;
    [SerializeField, Range(0.0f, 1.0f)] private float m_releaseBallProgress = 0.95f;
    [SerializeField] private float m_releaseForce = 10.0f;

    [NonSerialized] private Ball m_currentBall = default;

    [NonSerialized] private bool m_ballOnSpline = false;
    [NonSerialized] private float m_ballProgress = 0.0f;

    private void Start()
    {
        m_ballOnSpline = false;
    }

    private void FixedUpdate()
    {
        if (m_ballOnSpline)
        {
            if (m_ballProgress > m_releaseBallProgress)
            {
                ReleaseBall();
                return;
            }

            m_ballProgress += m_speedAlongTheSpline.Evaluate(m_ballProgress) * Time.fixedDeltaTime * m_speedFactor / m_spline.CalculateLength();
            m_currentBall.transform.position = m_spline.EvaluatePosition(m_ballProgress);
        }
    }

    public void StartNewBall(Ball ball)
    {
        if (m_ballOnSpline)
        {
            ReleaseBall();
        }

        m_ballOnSpline = true;
        m_ballProgress = 0.0f;

        ball.transform.position = m_spline.EvaluatePosition(0);
        ball.Rigidbody.isKinematic = true;
        ball.gameObject.SetActive(true);

        m_currentBall = ball;
    }

    private void ReleaseBall()
    {
        m_ballOnSpline = false;

        Vector3 ballForward = ((Vector3)m_spline.EvaluateTangent(m_ballProgress)).normalized;
        float ballSpeed = m_speedAlongTheSpline.Evaluate(m_ballProgress);

        m_currentBall.Rigidbody.isKinematic = false;
        m_currentBall.Rigidbody.velocity = ballSpeed * m_releaseForce * ballForward * m_speedFactor;
    }
}
