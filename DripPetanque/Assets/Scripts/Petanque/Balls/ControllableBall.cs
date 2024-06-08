using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtility.CustomAttributes;

public class ControllableBall : Ball 
{
    public event Action<ControllableBall> OnBallControllable;

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

    [NonSerialized] private int m_jumpsLeft = 0;


    [NonSerialized] private Vector3 m_additionnalForce = Vector3.zero;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
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
    }

    protected override void OnGroundTouched()
    {
        base.OnGroundTouched();
        m_jumpsLeft = m_jumpsCount;
    }

    protected override void OnGroundFirstTouched()
    {
        base.OnGroundFirstTouched();
        OnBallControllable?.Invoke(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + m_additionnalForce);
    }
}
