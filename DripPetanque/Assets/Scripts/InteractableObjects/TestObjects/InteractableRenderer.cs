using System;
using UnityEngine;

public class InteractableRenderer : InteractableObject
{
    [SerializeField] private Renderer m_renderer;
    [SerializeField] private Color[] m_colorList;

    [NonSerialized] private MaterialPropertyBlock m_rendererBlock;
    [NonSerialized] private int m_colorIndex;

    private void Awake()
    {
        m_rendererBlock = new MaterialPropertyBlock();
        m_colorIndex = 0;
    }

    public override string GetInteractionMessage()
    {
        return "Press Y to change color";
    }

    public override void Interact(PlayerController playerController)
    {
        m_renderer.GetPropertyBlock(m_rendererBlock);
        m_rendererBlock.SetColor("_BaseColor", m_colorList[m_colorIndex++]);
        m_colorIndex %= m_colorList.Length;
        m_renderer.SetPropertyBlock(m_rendererBlock);
    }
}
