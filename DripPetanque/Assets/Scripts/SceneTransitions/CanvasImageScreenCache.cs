using UnityEngine;
using UnityEngine.UI;

public class CanvasImageScreenCache : ScreenCache
{
    [SerializeField] private Color m_cacheColor;
    [SerializeField] private Image m_image;

    public override void Show(bool show)
    {
        m_image.gameObject.SetActive(show);
    }

    public override void UpdateCache(float progress)
    {
        Color tempColor = m_cacheColor;
        tempColor.a = progress;
        m_image.color = tempColor;
    }
}
