using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityUtility.CustomAttributes;
using UnityUtility.Timer;
using UnityUtility.Utils;

public class PetanqueSceneStartup : MonoBehaviour
{
    [Title("Petanque Scene Datas")]
    [SerializeField] private PetanqueSceneDatas m_petanqueSceneDatas;

    [SerializeField] private List<BasePetanquePlayer> m_petanquePlayers;
    [SerializeField] private PetanqueField m_field;

    [Title("Terrain Dissolve")]
    [SerializeField] private Vector2 m_dissolveHeightRange;
    [SerializeField] private float m_dissolveTime;
    [SerializeField] private float m_textureDissolveTime;
    [SerializeField] private Material m_dissolveMaterial;

    [SerializeField] private PlayableDirector m_playableDirector;

    private bool m_cinematicPlayed;

    private void Awake()
    {
        _ = StartCoroutine(Dissolve());
    }

    private void Start()
    {
        m_playableDirector.stopped += (_) => m_cinematicPlayed = true;
    }

    private IEnumerator Dissolve()
    {
        int materialDissolveAmountID = Shader.PropertyToID("_DissolveAmount");
        int materialTextureDissolveAmountID = Shader.PropertyToID("_TextureDissolveAmount");

        // Initialize the dissolve height to the minimum value of the range
        DissolvableRenderersManager.SetDissolveAmount(m_dissolveHeightRange.x, materialDissolveAmountID);
        DissolvableRenderersManager.SetDissolveAmount(m_dissolveHeightRange.x, materialTextureDissolveAmountID);

        m_playableDirector.Play();
        m_cinematicPlayed = false;

        yield return DissolveProperty(materialDissolveAmountID, m_dissolveTime);
        yield return DissolveProperty(materialTextureDissolveAmountID, m_textureDissolveTime);
        yield return new WaitUntil(() => m_cinematicPlayed);

        m_playableDirector.gameObject.SetActive(false);

        OnSceneReady();
    }

    private IEnumerator DissolveProperty(int propertyID, float dissolveTime)
    {
        Timer dissolveTimer = new Timer(dissolveTime, false);

        dissolveTimer.Start();
        float newAmount;
        while (!dissolveTimer.Update(Time.deltaTime))
        {
            newAmount = dissolveTimer.Progress.RemapFrom01(m_dissolveHeightRange);
            DissolvableRenderersManager.SetDissolveAmount(newAmount, propertyID);
            yield return null;
        }
        dissolveTimer.Stop();
    }

    private void OnSceneReady()
    {
        m_petanqueSceneDatas.PetanquePlayers = m_petanquePlayers;
        m_petanqueSceneDatas.Field = m_field;

        m_petanqueSceneDatas.NotifyDatasFilled();
    }
}
