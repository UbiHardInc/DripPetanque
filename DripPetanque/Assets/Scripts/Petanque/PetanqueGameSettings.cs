using UnityEngine;
using UnityUtility.SceneReference;

[CreateAssetMenu(fileName = nameof(PetanqueGameSettings), menuName = "Scriptables/Petanque/" + nameof(PetanqueGameSettings))]
public class PetanqueGameSettings : ScriptableObject
{
    public SceneReference PetanqueScene => m_petanqueScene;
    public int BallsPerRounds => m_ballsPerGame;
    public int PointsToWin => m_pointsToWin;
    public GameState ExitGameState => m_exitState;

    [SerializeField] private SceneReference m_petanqueScene;
    [SerializeField] private int m_ballsPerGame = 3;
    [SerializeField] private int m_pointsToWin = 13;
    [SerializeField] private GameState m_exitState = GameState.Exploration;
}
