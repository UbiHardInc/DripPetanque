using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.SerializedDictionary;
using UnityUtility.Singletons;

public class CanvasManager : MonoBehaviourSingleton<CanvasManager>
{
    [SerializeField] private SerializedDictionary<GameState, RectTransform> m_gameStateSpecificUIs;

    protected override void Start()
    {
        base.Start();
        GameManager gameManager = GameManager.Instance;
        gameManager.OnGameStateEntered += OnGameStateEntered;
        gameManager.OnGameStateExited += OnGameStateExited;

        if (gameManager.CurrentSubGameManager != null)
        {
            OnGameStateEntered(gameManager.CurrentSubGameManager.CorrespondingState);
        }
    }

    private void OnGameStateEntered(GameState state)
    {
        if (m_gameStateSpecificUIs.TryGetValue(state, out RectTransform panel))
        {
            panel.gameObject.SetActive(true);
        }
    }

    private void OnGameStateExited(GameState state)
    {
        if (m_gameStateSpecificUIs.TryGetValue(state, out RectTransform panel))
        {
            panel.gameObject.SetActive(false);
        }
    }
}
