using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtility.CustomAttributes;
using UnityUtility.Pools;
using UnityUtility.Timer;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour, IPoolOperationCallbackReciever
{
    public event Action OnBallStopped;

    public Rigidbody Rigidbody => m_rigidbody;

    [SerializeField, Layer] private int m_groundLayer;

    [Title("Inputs")]
    [SerializeField] private float m_speedToStop = 0.01f;
    [SerializeField] private Timer m_timerToStop;

    [Title("Inputs")]
    [SerializeField] private InputActionReference m_ballDirectionInput;
    [SerializeField] private InputActionReference m_jumpInput;

    [Title("Ball Movement")]
    [SerializeField, Min(0)] private float m_globalMovementFactor;
    [SerializeField, Min(0)] private float m_lateralMovementFactor;
    [SerializeField, Min(0)] private float m_forwardMovementFactor;
    [SerializeField, Min(0)] private float m_backwardMovementFactor;
    [Space(6)]
    [SerializeField, Min(0)] private int m_jumpsCount = 1;

    [NonSerialized] private bool m_ballStopped = false;
    [NonSerialized] private bool m_touchedGround = false;
    [NonSerialized] private bool m_grounded = false;
    [NonSerialized] private int m_jumpsLeft = 0;

    [NonSerialized] private Vector3 m_additionnalForce = Vector3.zero;
    [NonSerialized] private Rigidbody m_rigidbody;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.maxAngularVelocity = 100000;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == m_groundLayer)
        {
            OnGroundTouched();
        }
    }

    private void FixedUpdate()
    {
        if (m_ballStopped)
        {
            return;
        }

        // We can't control the ball until the ball touched the ground 
        if (m_touchedGround)
        {
            Vector2 directionInput = m_ballDirectionInput.action.ReadValue<Vector2>();

            Vector3 forward = m_rigidbody.velocity.sqrMagnitude == 0.0f ? transform.forward : m_rigidbody.velocity;
            if (!m_grounded)
            {
                forward = Vector3.ProjectOnPlane(forward, Vector3.up);
            }
            forward = forward.normalized;
            Vector3 right = -Vector3.Cross(forward, Vector3.up).normalized;
            Vector3 up = Vector3.Cross(forward, right).normalized;

            m_additionnalForce =
                (directionInput.x * m_lateralMovementFactor * right +
                (Mathf.Clamp(directionInput.y, 0.0f, 1.0f) * m_forwardMovementFactor +
                Mathf.Clamp(directionInput.y, -1.0f, 0.0f) * m_backwardMovementFactor) * forward) * m_globalMovementFactor;

            m_rigidbody.AddForce(m_additionnalForce, ForceMode.Acceleration);
        }

        // Checks if the ball is slow enough for enough time
        if (m_rigidbody.velocity.sqrMagnitude < m_speedToStop * m_speedToStop)
        {
            if (m_timerToStop.Update(Time.fixedDeltaTime))
            {
                m_timerToStop.Stop();
                StopBall();
            }
        }
        else
        {
            m_timerToStop.Stop();
        }
    }

    private void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + m_additionnalForce);
    }


    private void OnGroundTouched()
    {
        m_grounded = true;
        m_touchedGround = true;
        m_jumpsLeft = m_jumpsCount;
    }

    private void StopBall()
    {
        m_ballStopped = true;
        OnBallStopped?.Invoke();
    }

    public void ResetBall()
    {
        m_touchedGround = false;
        m_ballStopped = false;
    }

    #region IPoolOperationCallbackReciever Implementation
    public void OnObjectReleased()
    {
        ResetBall();
    }

    public void OnObjectRequested()
    {
        ResetBall();
    }
    #endregion
}
