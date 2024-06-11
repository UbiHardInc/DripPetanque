using UnityEngine;

public class BuildingPropertyBlockAssigner : MonoBehaviour
{
    private MeshRenderer m_meshRenderer;
    private Material m_material;
    private UnityEngine.MaterialPropertyBlock m_block;

    public Color topColor;
    public Color bottomColor;
    public Texture2D textureMap;
    // Start is called before the first frame update
    private void Awake()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_material = GetComponent<Material>();
        m_block = new UnityEngine.MaterialPropertyBlock();
    }

    private void Start()
    {
        m_meshRenderer.GetPropertyBlock(m_block);
        m_block.SetColor("_TopColor", topColor);
        m_block.SetColor("_BottomColor", bottomColor);
        if (textureMap != null)
        {
            m_block.SetTexture("_HeightMapPattern", textureMap);
        }
        m_meshRenderer.SetPropertyBlock(m_block);
    }
}
