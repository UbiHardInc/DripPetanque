using UnityEngine;

public class QuestObject : MonoBehaviour
{
    private void Start()
    {
        Quest.OnQuestTaken += OnQuestTaken;
        gameObject.SetActive(false);
    }

    private void OnQuestTaken()
    {
        gameObject.SetActive(true);
    }
}
