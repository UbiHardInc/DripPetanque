using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DissolvableRenderer : MonoBehaviour
{
    [NonSerialized] private MaterialPropertyBlock m_propertyBlock;
    [NonSerialized] private Renderer m_renderer;

    private void Awake()
    {
        DissolvableRenderersManager.RegisterRenderer(this);
        m_propertyBlock = new MaterialPropertyBlock();
        m_renderer = GetComponent<Renderer>();
    }

    private void OnDestroy()
    {
        DissolvableRenderersManager.UnregisterRenderer(this);
    }

    public void SetDissolveAmount(float amount, int dissolvePropertyID)
    {
        m_renderer.GetPropertyBlock(m_propertyBlock);
        
        m_propertyBlock.SetFloat(dissolvePropertyID, amount);

        m_renderer.SetPropertyBlock(m_propertyBlock);
    }
}
