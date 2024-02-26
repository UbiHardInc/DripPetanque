using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SpriteRandom : MonoBehaviour
{
    [SerializeField]
    private List<Image> spriteList;

    [SerializeField] 
    private Sprite spriteToShow;

    
    private Sprite m_lastSprite;
    private float m_spriteNumber;

    private void Awake()
    {
        throw new NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_spriteNumber = spriteList.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FlashSprite()
    {
        StartCoroutine(SpriteFlash());
    }

    private IEnumerator SpriteFlash()
    {
        Image actualSpot;
        actualSpot = spriteList[(int) Random.Range(0, m_spriteNumber - 1)];
        while (actualSpot == m_lastSprite)
        {
            actualSpot = spriteList[(int) Random.Range(0, m_spriteNumber - 1)];
        }

        actualSpot.sprite = spriteToShow;
        actualSpot.color = Color.white;
        
        yield return new WaitForSeconds(0.5f);

        actualSpot.sprite = null;
        actualSpot.color = Color.clear;
    }
}
