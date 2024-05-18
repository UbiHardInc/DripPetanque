using UnityEngine;

public class Interact_TalkQuestGiver : InteractableObject
{
    [SerializeField] private string m_messageToShow;
    [SerializeField] private DialogueData m_giveQuestDialogue;
    [SerializeField] private DialogueData m_validateQuestDialogue;
    [SerializeField] private DialogueData m_unCompleteQuestDialogue;
    [SerializeField] private DialogueData m_adLibDialogue; //Dialogue pour quand on a rendu la quete et qu'on reparle au personnage (non obligatoire)
    [SerializeField] private Quest m_attachedQuest;


    public override string GetInteractionMessage()
    {
        return m_messageToShow;
    }

    public override void Interact(PlayerController playerController)
    {
        if (!m_attachedQuest.IsActive)
        {
            if (m_giveQuestDialogue)
            {
                GameManager.Instance.ExplorationSubGameManager.StartDialogue(m_giveQuestDialogue);
            }
            //Pas obligatoire de mettre un dialogue (d'où le check). On pourrait par exemple vouloir mettre un Playable Director
            //sur le Quest Object qui s'activera a l'activation de l'objet (qui a lieu a l'appel du SetQuestAsActive())


            //Si on veut faire une animation d'UI on peut vouloir lancer cette méthode depuis un event de dialogue ou un event sur la timeline
            m_attachedQuest.SetQuestAsActive();

            return;
        }


        if(!m_attachedQuest.IsComplete())
        {
            GameManager.Instance.ExplorationSubGameManager.StartDialogue(m_unCompleteQuestDialogue);
            return;
        }

        GameManager.Instance.ExplorationSubGameManager.StartDialogue(m_validateQuestDialogue);

        if(m_adLibDialogue)
        {
            m_validateQuestDialogue = m_adLibDialogue;
        }
    }
}
