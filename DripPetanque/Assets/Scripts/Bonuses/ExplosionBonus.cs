using System;
using UnityEngine;
using UnityEngine.VFX;

public class ExplosionBonus : BonusBase
{
    [SerializeField] private LayerMask m_whatIsBall;
    [SerializeField] private float m_explosionRadius;
    [SerializeField] private float m_explosionForce;
    [SerializeField] private VisualEffect m_explosionEffect;
    [SerializeField] private float m_explosionDelay = 2.0f;

    [NonSerialized] private Transform m_ballTransform;

    public override float OnBallStop()
    {
        _ = base.OnBallStop();

        Explose();
        return m_explosionDelay;
    }

    public override void OnBonusAttached(Transform ballTransform)
    {
        base.OnBonusAttached(ballTransform);
        m_ballTransform = ballTransform;
    }

    private void Explose()
    {
        m_explosionEffect.transform.position = m_ballTransform.position;

        Collider ballCollider = m_ballTransform.GetComponent<Collider>();

        Collider[] colliders = Physics.OverlapSphere(m_ballTransform.position, m_explosionRadius, m_whatIsBall);

        foreach (Collider collider in colliders)
        {
            if (collider == ballCollider) { continue; }
            collider.GetComponent<Rigidbody>().AddExplosionForce(m_explosionForce, m_ballTransform.position, m_explosionRadius);
        }
        //m_explosionEffect.SendEvent("PlayExplosionRing");
        //m_explosionEffect.SendEvent("PlayExplosionCone");
        //m_explosionEffect.Play();
        m_explosionEffect.SendEvent("PlayExplosion");
        SoundManager.Instance.PlayUISFX("bomb");
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
