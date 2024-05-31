using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreenController : MonoBehaviour
{
    [SerializeField] private float m_fadeInTime;
    [SerializeField] private float m_fadeOutTime;

    private Image m_blackScreenImage;

    private void Awake()
    {
        m_blackScreenImage = GetComponent<Image>();
        m_blackScreenImage.color = new Color(0, 0, 0, 0); //Black with no alpha
    }

    private void OnEnable()
    {
        Sequence seq = DOTween.Sequence();

        _ = seq.Append(m_blackScreenImage.DOFade(1, m_fadeInTime).From(0));
        _ = seq.Append(m_blackScreenImage.DOFade(0, m_fadeOutTime).From(1));

        _ = seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });

        _ = seq.Play();
    }
}
