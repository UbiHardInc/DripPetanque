using UnityEngine;
using UnityEngine.Playables;
using UnityUtility.CustomAttributes;

public class StartBossCinematicBehaviour : MonoBehaviour
{
    [SerializeField] private PlayableDirector m_cinematicToLaunch;
    [SerializeField, Layer] private int m_layer;
    [SerializeField] private bool m_disableAfterTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == m_layer)
        {
            m_cinematicToLaunch.gameObject.SetActive(true);
        }

        if (m_disableAfterTrigger)
        {
            gameObject.SetActive(false);
        }
    }
}
