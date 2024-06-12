using System;
using UnityEngine;
using UnityUtility.CustomAttributes;

[RequireComponent(typeof(Collider))]
public class EffectZone : MonoBehaviour
{
    [SerializeField] private Vector3 m_effectDirection;
    [SerializeField] private float m_effectForce;
    [SerializeField] private float m_maxBallSpeed;
    [SerializeField, Layer] private int m_ballLayer;

    [NonSerialized] private Rigidbody m_ballRigibody;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != m_ballLayer)
        {
            return;
        }

        if (other.gameObject.TryGetComponent(out Rigidbody ballRB))
        {
            m_ballRigibody = ballRB;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_ballRigibody == null)
        {
            return;
        }

        if (other.gameObject.TryGetComponent(out Rigidbody ballRB) && m_ballRigibody == ballRB)
        {
            m_ballRigibody = null;
        }
    }


    private void FixedUpdate()
    {
        if (m_ballRigibody == null)
        {
            return;
        }

        if (m_ballRigibody.velocity.sqrMagnitude > m_maxBallSpeed * m_maxBallSpeed)
        {
            return;
        }

        m_ballRigibody.AddForce(m_effectDirection * m_effectForce);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, transform.position + m_effectDirection * m_effectForce);
    }
}
