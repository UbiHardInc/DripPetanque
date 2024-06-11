using System;
using UnityEngine;
using UnityUtility.CustomAttributes;

[Serializable]
public class NextStateDatas
{
    public GameState NextGameState => m_nextGameState;

    [SerializeField] private GameState m_nextGameState;

    //[SerializeField, ShowIf(nameof(m_nextGameState), GameState.Dialogue)]
    [SerializeField] private DialogueData m_dialogueToStart;
    //[SerializeField, ShowIf(nameof(m_nextGameState), GameState.Petanque)]
    [SerializeField] private PetanqueGameSettings m_petanqueGameToStart;

    public GameState ApplyDatas(GameManagersSharedDatas sharedDatas)
    {
        switch (m_nextGameState)
        {
            case GameState.None:
                break;
            case GameState.Dialogue:
                sharedDatas.NextDialogueToStart = m_dialogueToStart;
                break;
            case GameState.Petanque:
                sharedDatas.NextPetanqueGameSettings = m_petanqueGameToStart;
                break;
            case GameState.Cinematics:
                break;
            case GameState.Exploration:
                break;
            case GameState.MainMenu:
                break;
            default:
                break;
        }
        return m_nextGameState;
    }
}