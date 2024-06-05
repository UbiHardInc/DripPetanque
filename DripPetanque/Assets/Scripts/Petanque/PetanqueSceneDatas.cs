using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(PetanqueSceneDatas), menuName = "Scriptables/Petanque/" + nameof(PetanqueSceneDatas))]
public class PetanqueSceneDatas : ScriptableObject
{
    public event Action OnDatasFilled;

    public List<BasePetanquePlayer> PetanquePlayers;
    public PetanqueField Field;

    public void NotifyDatasFilled()
    {
        OnDatasFilled?.Invoke();
    }
}
