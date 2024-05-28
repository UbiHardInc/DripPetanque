using System;
using TMPro;
using UnityEngine;
using UnityUtility.Utils;

[Serializable]
public class ComputerShootStep : BaseShootStep
{
    [SerializeField] private TMP_Text m_infoPanelText;
    
    public override void Start()
    {
        m_stepOutputValue = UnityEngine.Random.value.RemapFrom01(m_data.Range);
        
        m_infoPanelText.transform.parent.gameObject.SetActive(true);
        m_infoPanelText.text = "Opponent's turn.";
    }

    public override void Update(float deltaTime)
    {
    }

    public override bool IsFinished()
    {
        m_infoPanelText.transform.parent.gameObject.SetActive(false);
        return true;
    }

    public override bool HasTempValue()
    {
        return false;
    }
}
