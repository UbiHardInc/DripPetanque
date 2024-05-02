using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPropoertyBlockAssigner : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private Material _material;
    private UnityEngine.MaterialPropertyBlock _block;

    public Color topColor;
    public Color bottomColor;
    public Texture2D textureMap;
    // Start is called before the first frame update
    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _material = GetComponent<Material>();
        _block = new UnityEngine.MaterialPropertyBlock();
    }
    void Start()
    {
        _meshRenderer.GetPropertyBlock(_block);
        _block.SetColor("_TopColor", topColor);
        _block.SetColor("_BottomColor", bottomColor);
        _block.SetTexture("_HeightMapPattern", textureMap);
        _meshRenderer.SetPropertyBlock(_block);
    }
}
