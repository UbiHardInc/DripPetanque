using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Singletons;

public class VirtualCamerasManager : MonoBehaviourSingleton<VirtualCamerasManager>
{
    private enum BlendState
    {
        NotBlending,
        Blending,
    }

    public class SwitchCameraCallbackReciever
    {
        public event Action OnCameraSwitchOver;
    }

    private static List<CinemachineVirtualCamera> m_allVirtualCameras = new List<CinemachineVirtualCamera>();
    private static CinemachineBrain m_brain = null;
    private static CinemachineBlendDefinition m_defaultBlend = default;

    public static void RegisterBrain(CinemachineBrain brain)
    {
        if (m_brain != null)
        {
            throw new System.Exception($"There sould not be 2 {nameof(CinemachineBrain)} registered in the {nameof(VirtualCamerasManager)}");
        }
        m_brain = brain;
        m_defaultBlend = m_brain.m_DefaultBlend;
    }

    public static void UnregisterBrain()
    {
        if (m_brain == null)
        {
            Debug.LogError($"No {nameof(CinemachineBrain)} were previously registered");
        }
        m_brain = null;
    }

    private void Update()
    {
        
    }

    public static bool IsBrainBlending()
    {
        if (m_brain == null)
        {
            Debug.LogError($"No {nameof(CinemachineBrain)} registered");
            return false;
        }
        return m_brain.IsBlending;
    }

    public static void SwitchToCamera(CinemachineVirtualCamera virtualCamera)
    {
        m_brain.m_DefaultBlend = m_defaultBlend;
        SwitchToCamera_Impl(virtualCamera);
    }

    public static void SwitchToCamera(CinemachineVirtualCamera virtualCamera, float blendTime)
    {
        CinemachineBlendDefinition newDefiniton = m_defaultBlend;
        newDefiniton.m_Time = blendTime;
        m_brain.m_DefaultBlend = newDefiniton;
        SwitchToCamera(virtualCamera);
    }

    private static void SwitchToCamera_Impl(CinemachineVirtualCamera virtualCamera)
    {
        m_allVirtualCameras.ForEach(cam => cam.gameObject.SetActive(false));
        foreach (CinemachineVirtualCamera cam in m_allVirtualCameras)
        {
            cam.gameObject.SetActive(false);
        }
        virtualCamera.gameObject.SetActive(true);
    }

    public static void RegisterCamera(CinemachineVirtualCamera virtualCamera)
    {
        virtualCamera.gameObject.SetActive(false);
        if (m_allVirtualCameras.Contains(virtualCamera))
        {
            return;
        }
        m_allVirtualCameras.Add(virtualCamera);
    }

    public static void UnRegisterCamera(CinemachineVirtualCamera virtualCamera)
    {
        _ = m_allVirtualCameras.Remove(virtualCamera);
    }
}
