using System.Collections;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Timer;
using UnityUtility.Utils;

public class PetanqueSceneStartup : MonoBehaviour
{
    [Title("Petanque Scene Datas")]
    [SerializeField] private PetanqueSceneDatas m_petanqueSceneDatas;

    [SerializeField] private PetanqueGameSettings m_gameSettings;
    [SerializeField] private ShootManager m_playerShootManager;
    [SerializeField] private ComputerShootManager m_computerShootManager;
    [SerializeField] private PetanqueField m_field;

    [Title("Terrain Dissolve")]
    [SerializeField] private Vector2 m_dissolveHeightRange;
    [SerializeField] private float m_dissolveTime;
    [SerializeField] private float m_textureDissolveTime;
    [SerializeField] private Material m_dissolveMaterial;

    private void Awake()
    {
        _ = StartCoroutine(Dissolve());
    }

    private IEnumerator Dissolve()
    {
        int materialDissolveAmountID = Shader.PropertyToID("_DissolveAmount");
        int materialTextureDissolveAmountID = Shader.PropertyToID("_TextureDissolveAmount");

        m_dissolveMaterial.SetFloat(materialDissolveAmountID, m_dissolveHeightRange.x);
        m_dissolveMaterial.SetFloat(materialTextureDissolveAmountID, m_dissolveHeightRange.x);
        // Initialize the dissolve height to the minimum value of the range
        //DissolvableRenderersManager.SetDissolveAmount(m_dissolveHeightRange.x, materialDissolveAmountID);
        //DissolvableRenderersManager.SetDissolveAmount(m_dissolveHeightRange.x, materialTextureDissolveAmountID);

        yield return DissolveProperty(materialDissolveAmountID, m_dissolveTime);
        yield return DissolveProperty(materialTextureDissolveAmountID, m_textureDissolveTime);

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
            m_dissolveMaterial.SetFloat(propertyID, newAmount);

            //DissolvableRenderersManager.SetDissolveAmount(newAmount, propertyID);
            yield return null;
        }
        dissolveTimer.Stop();
    }

    private void OnSceneReady()
    {
        m_petanqueSceneDatas.GameSettings = m_gameSettings;
        m_petanqueSceneDatas.PlayerShootManager = m_playerShootManager;
        m_petanqueSceneDatas.ComputerShootManager = m_computerShootManager;
        m_petanqueSceneDatas.Field = m_field;

        m_petanqueSceneDatas.NotifyDatasFilled();
    }
}
