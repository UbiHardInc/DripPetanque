using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class VisualEffectTimeScale : MonoBehaviour
{
    [Range(0.0f, 10.0f)] public float SimulationTimeScale = 1.0f;
    private VisualEffect m_vfx;
    // Start is called before the first frame update
    void Start()
    {
        m_vfx = GetComponent<VisualEffect>();
        m_vfx.playRate = SimulationTimeScale;
    }

    
}
