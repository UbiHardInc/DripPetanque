using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestObjective
{
    [SerializeField] private string m_questId;

    private bool m_isComplete;

    public void CompleteObjective()
    {
        m_isComplete = true;
    }

    public bool IsComplete()
    {
        return m_isComplete;
    }

    public string GetQuestId()
    {
        return m_questId;
    }
}
