using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility.Singletons;
using Random = UnityEngine.Random;

public class SpriteRandom : MonoBehaviourSingleton<SpriteRandom>
{
    [SerializeField]
    private List<Image> m_spriteList;

    [SerializeField] 
    private Sprite m_spriteToShow;

    
    private Sprite m_lastSprite;
    private float m_spriteNumber;

    // Start is called before the first frame update

    protected override void Start()
    {
        base.Start();

        m_spriteNumber = m_spriteList.Count;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void FlashSprite()
    {
        _ = StartCoroutine(SpriteFlash());
    }

    private IEnumerator SpriteFlash()
    {
        Image actualSpot;
        actualSpot = m_spriteList[(int) Random.Range(0, m_spriteNumber - 1)];
        while (actualSpot == m_lastSprite)
        {
            actualSpot = m_spriteList[(int) Random.Range(0, m_spriteNumber - 1)];
        }

        actualSpot.sprite = m_spriteToShow;
        actualSpot.color = Color.white;
        
        yield return new WaitForSeconds(0.5f);

        actualSpot.sprite = null;
        actualSpot.color = Color.clear;
    }
}
