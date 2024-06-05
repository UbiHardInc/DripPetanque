using UnityEngine;

public class ExplosionBonus : BonusBase
{
    [SerializeField] private LayerMask m_whatIsBall;
    [SerializeField] private float m_explosionRadius;
    [SerializeField] private float m_explosionForce;

    public override void OnBallStop()
    {
        base.OnBallStop();

        Explose();
    }

    private void Explose()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_explosionRadius, m_whatIsBall);

        foreach (Collider collider in colliders)
        {
            collider.GetComponent<Rigidbody>().AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_explosionRadius);
    }
}
