using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DistanceDissolveTarget : MonoBehaviour
{
    public Transform ObjectToTrack;

    private Material m_materialRef;
    private Renderer m_renderer;


    public Renderer Renderer
    {
        get
        {
            if(m_renderer == null)
            {
                m_renderer = GetComponent<Renderer>();
            }
            return m_renderer;
        }
    }

    public Material Material
    {
        get
        {
            if(m_materialRef == null)
            {
                m_materialRef = Renderer.material;
            }
            return m_materialRef;
        }
    }

    private void Awake()
    {
        m_renderer = GetComponent<Renderer>();
        m_materialRef = m_renderer.material;
    }

    private void Update()
    {
        
    }
}
