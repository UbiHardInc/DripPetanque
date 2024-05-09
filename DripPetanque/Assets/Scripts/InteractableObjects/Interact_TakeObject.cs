using UnityEngine;
using DG.Tweening;

public class Interact_TakeObject : InteractableObject
{
    [SerializeField] private string m_objectName;
    [SerializeField] private Quest m_attachedQuest;
    [SerializeField] private string m_questObjectiveId;
    [SerializeField] private Material m_material;
    [SerializeField] private GameObject m_objectToDeactivateWhenPickedUp;

    private readonly float m_dissolveDuration = 2;
    private string m_messageToShow;

    public override string GetInteractionMessage()
    {
        m_messageToShow = $"Take {m_objectName}";
        return m_messageToShow;
    }

    public override void Interact(PlayerController playerController)
    {
        MakeObjectDisapear();
    }

    private void MakeObjectDisapear()
    {
        if(m_material.HasProperty("_DissolveAmount"))
        {
            float dissolveAmount = m_material.GetFloat("_DissolveAmount");

            Tween tween = DOVirtual.Float(0, 1, m_dissolveDuration, x => m_material.SetFloat("_DissolveAmount", dissolveAmount = x));

            _ = tween.OnComplete(() =>
            {
                m_objectToDeactivateWhenPickedUp.SetActive(false);
            });

            _ = tween.Play();
        }

        m_attachedQuest.QuestObjective[m_questObjectiveId].CompleteObjective();
    }
}
