using System;
using UnityEngine;

public class ExplosionBonus : BonusBase
{
    [SerializeField] private LayerMask m_whatIsBall;
    [SerializeField] private float m_explosionRadius;
    [SerializeField] private float m_explosionForce;

    [NonSerialized] private Transform m_ballTransform;

    public override void OnBallStop()
    {
        base.OnBallStop();

        Explose();
    }

    public override void OnBonusAttached(Transform ballTransform)
    {
        base.OnBonusAttached(ballTransform);
        m_ballTransform = ballTransform;

    }

    private void Explose()
    {
        Collider[] colliders = Physics.OverlapSphere(m_ballTransform.position, m_explosionRadius, m_whatIsBall);

        foreach (Collider collider in colliders)
        {
            collider.GetComponent<Rigidbody>().AddExplosionForce(m_explosionForce, m_ballTransform.position, m_explosionRadius);
        }
    }

    private void OnDrawGizmos()
    {
        if (m_ballTransform == null)
        {
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(m_ballTransform.position, m_explosionRadius);
    }
}
