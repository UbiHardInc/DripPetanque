using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramGlitch : MonoBehaviour
{
    private MeshRenderer m_meshRenderer;
    private UnityEngine.MaterialPropertyBlock m_block;
    private float m_glitchOriginalStrenght;
    private float m_glitchEndStrenght;
    private float m_glitchDuration;
    private float m_timer;
    private float m_thresholdBetweenGlitches;
    private bool m_isPositive;
    private bool m_doubleGlitch;

    public float maxGlitchStrenght;
    public float maxthresholdBetweenGlitches;

    private void Awake()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_block = new UnityEngine.MaterialPropertyBlock();
    }
    // Start is called before the first frame update
    void Start()
    {
        m_glitchOriginalStrenght = 0f;
        m_timer = 0f;
        m_glitchDuration = 0.1f;    
        m_glitchEndStrenght = Random.Range(1f, maxGlitchStrenght);
        m_isPositive = Random.value < 0.5f;
        m_doubleGlitch = Random.value < 0.5f;
        m_thresholdBetweenGlitches = Random.Range(1f, maxthresholdBetweenGlitches);
        if (!m_isPositive) { m_glitchEndStrenght = -m_glitchEndStrenght; }
    }

    // Update is called once per frame
    void Update()
    {
        StartTheGlitch();
    }

    private void StartTheGlitch()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= m_thresholdBetweenGlitches)
        {
            StartCoroutine(Glitch(m_glitchDuration));
            m_timer = 0f;
        }        
    }

    private IEnumerator Glitch(float duration)
    {
        m_meshRenderer.GetPropertyBlock(m_block);
        m_block.SetFloat("_GlitchStrength", m_glitchOriginalStrenght);
        m_meshRenderer.SetPropertyBlock(m_block);

        float timePassed = 0f;
        float lerp = 0f;
        if (m_doubleGlitch && m_isPositive)
        {
            while (timePassed < duration)
            {
                m_meshRenderer.GetPropertyBlock(m_block);
                m_block.SetFloat("_GlitchStrength", lerp = Mathf.Lerp(-m_glitchEndStrenght, m_glitchEndStrenght, timePassed / duration));
                m_meshRenderer.SetPropertyBlock(m_block);

                timePassed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            while(timePassed < duration)
            {
                m_meshRenderer.GetPropertyBlock(m_block);
                m_block.SetFloat("_GlitchStrength", lerp = Mathf.Lerp(m_glitchOriginalStrenght, m_glitchEndStrenght, timePassed / duration));
                m_meshRenderer.SetPropertyBlock(m_block);

                timePassed += Time.deltaTime;
                yield return null;
            }
        }

        m_meshRenderer.GetPropertyBlock(m_block);
        m_block.SetFloat("_GlitchStrength", m_glitchOriginalStrenght);
        m_meshRenderer.SetPropertyBlock(m_block);

        m_glitchEndStrenght = Random.Range(1f, maxGlitchStrenght);
        m_isPositive = Random.value < 0.5f;
        m_doubleGlitch = Random.value < 0.5f;
        m_thresholdBetweenGlitches = Random.Range(1f, maxthresholdBetweenGlitches);
        if (!m_isPositive) { m_glitchEndStrenght = -m_glitchEndStrenght; }
    }
}
