using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(PetanqueSceneDatas), menuName = "Scriptables/Petanque/" + nameof(PetanqueSceneDatas))]
public class PetanqueSceneDatas : ScriptableObject
{
    public event Action OnDatasFilled;
        
    public PetanqueGameSettings GameSettings;
    public ShootManager PlayerShootManager;
    public ComputerShootManager ComputerShootManager;
    public PetanqueField Field;

    public void NotifyDatasFilled()
    {
        OnDatasFilled?.Invoke();
    }
}
