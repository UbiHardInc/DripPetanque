using UnityEngine;

[CreateAssetMenu(fileName = nameof(PetanqueGameSettings), menuName = "Scriptables/" + nameof(PetanqueGameSettings))]
public class PetanqueGameSettings : ScriptableObject
{
    public int BallsPerGame => m_ballsPerGame;
    public int PointsToWin => m_pointsToWin;

    [SerializeField] private int m_ballsPerGame = 3;
    [SerializeField] private int m_pointsToWin = 13;
}
