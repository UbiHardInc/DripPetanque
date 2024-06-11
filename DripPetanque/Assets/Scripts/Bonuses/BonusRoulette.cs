using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Timer;
using UnityUtility.Utils;

using Random = UnityEngine.Random;

public class BonusRoulette : MonoBehaviour
{
    [SerializeField] private List<BonusBase> m_bonuses = new List<BonusBase>();
    [SerializeField] private Transform m_bonusesParent;
    [SerializeField] private float m_timeBetweenBonus;
    [SerializeField] private TriggerObject m_trigger;

    [NonSerialized] private BonusBase m_bonusToAttachToBall;
    [NonSerialized] protected bool m_ballLaunched;

    [NonSerialized] private BonusBase[] m_instanciatedBonuses;

    [NonSerialized] private int m_currentBonusIndex;
    [NonSerialized] private Timer m_cycleTimer;

    private void Awake()
    {
        int bonusesCount = m_bonuses.Count;
        m_instanciatedBonuses = new BonusBase[bonusesCount];
        for (int i = 0; i < bonusesCount; i++)
        {
            BonusBase bonus = m_bonuses[i];
            BonusBase instanciatedBonus = Instantiate(bonus, m_bonusesParent);
            instanciatedBonus.gameObject.SetActive(false);
            m_instanciatedBonuses[i] = instanciatedBonus;
        }
        m_instanciatedBonuses.Shuffle();

        PetanqueSubGameManager petanqueSubGameManager = GameManager.Instance.PetanqueSubGameManager;
        petanqueSubGameManager.OnBallLauched += BonusRouletteStarter;
        petanqueSubGameManager.OnNextTurn += BonusRouletteStarter;
        m_trigger.OnTriggerEnterEvent += OnEnteredTrigger;

        m_cycleTimer = new Timer(m_timeBetweenBonus, true, Random.value.RemapFrom01(0.0f, m_timeBetweenBonus * 0.5f));
    }

    private void OnDestroy()
    {
        PetanqueSubGameManager petanqueSubGameManager = GameManager.Instance?.PetanqueSubGameManager;
        if (petanqueSubGameManager != null)
        {
            petanqueSubGameManager.OnBallLauched -= BonusRouletteStarter;
            petanqueSubGameManager.OnNextTurn -= BonusRouletteStarter;
        }
        m_trigger.OnTriggerEnterEvent -= OnEnteredTrigger;
    }

    protected virtual void Update()
    {
        if (!m_ballLaunched)
        {
            _ = m_cycleTimer.Update(Time.deltaTime);
        }
    }

    private void BonusRouletteStarter(bool rouletteShouldStart)
    {
        m_ballLaunched = !rouletteShouldStart;

        if (rouletteShouldStart)
        {
            m_currentBonusIndex = UnityEngine.Random.Range(0, m_instanciatedBonuses.Length);
            m_bonusToAttachToBall = m_instanciatedBonuses[m_currentBonusIndex];

            m_cycleTimer.OnTimerEnds += OnCycleEnds;
            m_cycleTimer.Start();
        }
        else
        {
            m_cycleTimer.OnTimerEnds -= OnCycleEnds;
            m_cycleTimer.Stop();
        }
    }

    private void OnCycleEnds()
    {
        m_instanciatedBonuses.ForEach(bonus => bonus.gameObject.SetActive(false));

        m_currentBonusIndex++;
        m_currentBonusIndex %= m_instanciatedBonuses.Length;

        m_bonusToAttachToBall = m_instanciatedBonuses[m_currentBonusIndex];
        m_bonusToAttachToBall.gameObject.SetActive(true);
    }


    private void OnEnteredTrigger(Collider other)
    {
        if (other.TryGetComponent(out Ball ball))
        {
            ball.AttachBonus(Instantiate(m_bonusToAttachToBall, other.transform));
            SoundManager.Instance.PlayUISFX("bonus");
        }
    }
}
