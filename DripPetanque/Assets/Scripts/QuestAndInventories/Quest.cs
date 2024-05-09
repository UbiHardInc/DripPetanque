using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Scriptables/Quests/Quest")]
public class Quest : ScriptableObject
{
    [SerializeField] private List<QuestObjective> m_questObjectives = new List<QuestObjective>();
    private bool m_isInitialized;
    private bool m_isActive;

    public Dictionary<string, QuestObjective> QuestObjective = new Dictionary<string, QuestObjective>();
    public static Action OnQuestTaken;
    public bool IsActive
    {
        get => m_isActive;
        private set => m_isActive = value;
    }

    public void Initialize()
    {
        if(m_isInitialized)
        {
            return;
        }

        m_isInitialized = true;

        for(int i = 0; i < m_questObjectives.Count; i++)
        {
            QuestObjective.Add(m_questObjectives[i].GetQuestId(), m_questObjectives[i]);
        }
    }

    public void SetQuestAsActive()
    {
        m_isActive = true;

        if (!m_isInitialized)
        {
            Initialize();
        }

        OnQuestTaken?.Invoke();
    }

    public bool IsComplete()
    {
        bool isComplete = true;

        foreach(QuestObjective objective in QuestObjective.Values)
        {
            if(!objective.IsComplete())
            {
                isComplete = false;
                break;
            }
        }

        return isComplete;
    }

    public void ResetQuestData()
    {
        m_isActive = false;
        m_isInitialized = false;
        QuestObjective.Clear();
    }
}
