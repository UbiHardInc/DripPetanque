using UnityEngine;

[System.Serializable]
public struct DialogueIDWraper
{
    [SerializeField] private string _iD;
    DialogueIDWraper(string iD)
    {
        _iD = iD;
    }

    public static implicit operator string (DialogueIDWraper dialogueEventId)
    {
        return dialogueEventId._iD;
    }

    public static implicit operator DialogueIDWraper(string iD)
    {
        return new DialogueIDWraper(iD);
    }
}
