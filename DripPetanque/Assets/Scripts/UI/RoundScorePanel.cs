using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

public class RoundScorePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text m_roundNumber;
    [SerializeField] private TMP_Text[] m_scoreTexts;
    
    [SerializeField] private Color m_colorWinText;

    public void InitializeRoundScorePanel(int roundNumber, Span<int> scores, int winnerIndex)
    {
        m_roundNumber.text = roundNumber.ToString();
        int scoresCount = scores.Length;
        int scoresTextsCount = m_scoreTexts.Length;

        if (scoresCount != scoresTextsCount)
        {
            Debug.LogError($"Invalid number of scores given ({scoresCount} given, expected {scoresTextsCount})", this);
            return;
        }

        for (int i = 0; i < scoresTextsCount; i++)
        {
            TMP_Text scoreText = m_scoreTexts[i];
            scoreText.text = scores[i].ToString();
            if (i == winnerIndex)
            {
                scoreText.color = m_colorWinText;
            }
        }

        _ = GetComponent<RectTransform>().DORotate(new Vector3(0f,0f,0f), 1f).From(new Vector3(0f, -90f, 0f));
    }
}
