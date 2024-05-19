using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    [SerializeField] private DialogueData m_testDialogue;
    private DialogueManager m_dialogueManager;

    private void Awake()
    {
        m_dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            m_dialogueManager.StartDialogue(m_testDialogue);
        }
    }

}
