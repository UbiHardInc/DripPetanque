using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Scriptables/Quests/Quest")]
public class Quest : ScriptableObject
{
    [SerializeField] private List<QuestObjective> m_questObjectives = new List<QuestObjective>();
    private bool m_isInitialized;

    public Dictionary<string, QuestObjective> QuestObjective = new Dictionary<string, QuestObjective>();



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
}
