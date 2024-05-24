using UnityEngine;

[System.Serializable]
public struct DialogueIDWraper
{
    [UnityEngine.Serialization.FormerlySerializedAs("_iD")]
    [SerializeField] private string m_iD;

    private DialogueIDWraper(string iD)
    {
        m_iD = iD;
    }

    public static implicit operator string (DialogueIDWraper dialogueEventId)
    {
        return dialogueEventId.m_iD;
    }

    public static implicit operator DialogueIDWraper(string iD)
    {
        return new DialogueIDWraper(iD);
    }
}
