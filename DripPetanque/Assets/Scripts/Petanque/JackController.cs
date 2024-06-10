using UnityEngine;
using UnityUtility.Utils;

/// <summary>
/// Moves jack at the start of every round
/// </summary>
public class JackController : MonoBehaviour
{
    [SerializeField] private Transform m_jack;

    [SerializeField] private float m_range = 5.0f;

    private void Start()
    {
        GameManager.Instance.PetanqueSubGameManager.OnNextRound += MoveJack;
    }
    private void OnDestroy()
    {
        GameManager.Instance.PetanqueSubGameManager.OnNextRound -= MoveJack;
    }

    public void MoveJack()
    {
        m_jack.position = transform.position + Random.insideUnitCircle.X0Y() * m_range;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_range);
    }
}
