using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Log", menuName = "Scriptables/Quests/Quest log")]
public class QuestLog : ScriptableObject
{
    public List<Quest> Quests = new List<Quest>();
}
