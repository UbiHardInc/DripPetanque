using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityUtility.Pools;

public class BallTrajectoryController : MonoBehaviour
{
    [SerializeField] private Rigidbody m_ball;
    [SerializeField] private GameObject m_floor;
    [SerializeField] private SplineContainer m_spline;

    [SerializeField] private AnimationCurve m_speedAlongTheSpline = new AnimationCurve();
    [SerializeField] private float m_speedFactor = 0.001f;
    [SerializeField, Range(0.0f, 1.0f)] private float m_releaseBallProgress = 0.95f;
    [SerializeField] private float m_releaseForce = 10.0f;

    [NonSerialized] private PooledObject<Rigidbody> m_currentBallPO;
    [NonSerialized] private Rigidbody m_currentBall = null;

    [NonSerialized] private bool m_ballOnSpline = false;
    [NonSerialized] private float m_ballProgress = 0.0f;

    [NonSerialized] private float m_currentSpeed = 0.0f;

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

            m_ballProgress += m_speedAlongTheSpline.Evaluate(m_ballProgress) * Time.fixedDeltaTime * m_speedFactor;
            m_currentBall.transform.position = m_spline.EvaluatePosition(m_ballProgress);
        }
    }

    public void StartNewBall()
    {
        if (m_ballOnSpline)
        {
            ReleaseBall();
        }

        m_ballOnSpline = true;
        m_ballProgress = 0.0f;

        m_currentBall = Instantiate(m_ball);
        //m_currentBall.collisionDetectionMode = m_ball.collisionDetectionMode;
        m_currentBall.transform.position = m_spline.EvaluatePosition(0);


        m_currentBall.isKinematic = true;
        m_currentBall.gameObject.SetActive(true);
    }

    public IEnumerator ClearAllBalls()
    {
        m_floor.SetActive(false);
        yield return new WaitForSeconds(2.0f);
        m_floor.SetActive(true);
    }

    private void ReleaseBall()
    {
        m_ballOnSpline = false;

        Vector3 ballForward = m_spline.EvaluateTangent(m_ballProgress);
        float ballSpeed = m_speedAlongTheSpline.Evaluate(m_ballProgress);

        m_currentBall.isKinematic = false;
        m_currentBall.velocity = ballSpeed * m_releaseForce * ballForward * m_speedFactor;
    }
}
