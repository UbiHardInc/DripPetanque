using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BonusRoulette : MonoBehaviour
{
    [SerializeField] private List<BonusBase> m_bonuses = new List<BonusBase>();
    [SerializeField] private Image m_bonusRingSpriteRenderer;
    [SerializeField] private float m_timeBetweenBonus;

    private BonusBase m_bonusToAttachToBall;
    private bool m_ballLaunched;

    private void Awake()
    {
        GameManager.Instance.PetanqueSubGameManager.OnBallLauched += BonusRouletteStarter;
        GameManager.Instance.PetanqueSubGameManager.OnNextTurn += BonusRouletteStarter;
    }

    private async void BonusCycle()
    {
        int randomStartCycleIndex = UnityEngine.Random.Range(0, m_bonuses.Count);

        while (!m_ballLaunched)
        {
            m_bonusRingSpriteRenderer.sprite = m_bonuses[randomStartCycleIndex].BonusSprite;

            await Task.Delay(TimeSpan.FromSeconds(m_timeBetweenBonus));

            if (randomStartCycleIndex < m_bonuses.Count - 1)
            {
                randomStartCycleIndex++;
            }
            else
            {
                randomStartCycleIndex = 0;
            }
        }

        m_bonusToAttachToBall = m_bonuses[randomStartCycleIndex];
    }

    private void BonusRouletteStarter(bool rouletteShouldStart)
    {
        m_ballLaunched = !rouletteShouldStart;

        if(rouletteShouldStart)
        {
            BonusCycle();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        _ = Instantiate(m_bonusToAttachToBall, other.transform);
    }
}
