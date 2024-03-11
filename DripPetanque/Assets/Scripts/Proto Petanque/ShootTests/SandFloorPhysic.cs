using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SandFloorPhysic : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)] private float m_velocityMultiplier = 0.9f;
    [SerializeField, Range(0.0f, 1.0f)] private float m_angularVelocityMultiplier = 0.9f;


    private void OnCollisionStay(Collision collision)
    {
        Rigidbody otherRB = collision.rigidbody;
        otherRB.velocity *= m_velocityMultiplier;
        otherRB.angularVelocity *= m_angularVelocityMultiplier;
    }
}
