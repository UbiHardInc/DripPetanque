using System;
using UnityUtility.CustomAttributes;

[Serializable]
public class GameManagersSharedDatas
{
    [Title("Dialogues")]
    public DialogueData NextDialogueToStart;

    [Title("Petanque")]
    public PetanqueGameSettings NextPetanqueGameSettings;

}
