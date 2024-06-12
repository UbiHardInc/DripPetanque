using UnityEngine;
using UnityUtility.SceneReference;

[CreateAssetMenu(fileName = nameof(PetanqueGameSettings), menuName = "Scriptables/Petanque/" + nameof(PetanqueGameSettings))]
public class PetanqueGameSettings : ScriptableObject
{
    public SceneReference PetanqueScene => m_petanqueScene;
    public int BallsPerRounds => m_ballsPerGame;
    public int PointsToWin => m_pointsToWin;

    public bool IsTutorial => m_isTutorial;

    [SerializeField] private SceneReference m_petanqueScene;
    [SerializeField] private int m_ballsPerGame = 3;
    [SerializeField] private int m_pointsToWin = 13;

    [SerializeField] private NextStateDatas m_humanWonNextStateDatas;
    [SerializeField] private NextStateDatas m_humanLostNextStateDatas;

    [SerializeField] private bool m_isTutorial;

    public NextStateDatas GetNextStateDatas(PetanquePlayerType winner)
    {
        return winner == PetanquePlayerType.Human ? m_humanWonNextStateDatas : m_humanLostNextStateDatas;
    }
}
