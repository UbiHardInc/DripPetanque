using TMPro;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text m_text;
    [SerializeField] private RectTransform m_objectToDeactivate;

    public void SetText(string newtext)
    {
        m_text.text = newtext;
    }

    public void Activate(bool activate)
    {
        m_objectToDeactivate.gameObject.SetActive(activate);
    }

}
