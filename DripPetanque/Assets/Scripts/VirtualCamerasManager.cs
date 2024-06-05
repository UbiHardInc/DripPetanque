using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityUtility.Singletons;
using UnityUtility.Utils;

public class VirtualCamerasManager : MonoBehaviourSingleton<VirtualCamerasManager>
{
    private enum BlendState
    {
        NotBlending,
        Blending,
    }

    private static readonly List<CinemachineVirtualCamera> s_allVirtualCameras = new List<CinemachineVirtualCamera>();
    private static CinemachineBrain s_brain = null;
    private static CinemachineVirtualCamera s_currentTarget;
    private static CinemachineBlendDefinition s_defaultBlend = default;

    public static void RegisterBrain(CinemachineBrain brain)
    {
        if (s_brain != null)
        {
            throw new Exception($"There should not be 2 {nameof(CinemachineBrain)} registered in the {nameof(VirtualCamerasManager)}");
        }
        s_brain = brain;
        s_defaultBlend = s_brain.m_DefaultBlend;
    }

    public static void UnregisterBrain()
    {
        if (s_brain == null)
        {
            Debug.LogError($"No {nameof(CinemachineBrain)} were previously registered");
        }
        s_brain = null;
    }

    public static bool IsBrainMoving()
    {
        if (s_brain == null)
        {
            Debug.LogError($"No {nameof(CinemachineBrain)} registered");
            return false;
        }
        return !s_brain.transform.position.ApproximatelyEqualsPoint(s_currentTarget.transform.position);
    }

    public static void SwitchToCamera(CinemachineVirtualCamera virtualCamera)
    {
        s_brain.m_DefaultBlend = s_defaultBlend;
        SwitchToCamera_Impl(virtualCamera);
    }

    public static void SwitchToCamera(CinemachineVirtualCamera virtualCamera, float blendTime)
    {
        CinemachineBlendDefinition newDefiniton = s_defaultBlend;
        newDefiniton.m_Time = blendTime;
        s_brain.m_DefaultBlend = newDefiniton;
        SwitchToCamera(virtualCamera);
    }

    private static void SwitchToCamera_Impl(CinemachineVirtualCamera virtualCamera)
    {
        s_allVirtualCameras.ForEach(cam => cam.gameObject.SetActive(false));
        foreach (CinemachineVirtualCamera cam in s_allVirtualCameras)
        {
            cam.gameObject.SetActive(false);
        }
        s_currentTarget = virtualCamera;
        virtualCamera.gameObject.SetActive(true);
    }

    public static void RegisterCamera(CinemachineVirtualCamera virtualCamera)
    {
        virtualCamera.gameObject.SetActive(false);
        if (s_allVirtualCameras.Contains(virtualCamera))
        {
            return;
        }
        s_allVirtualCameras.Add(virtualCamera);
    }

    public static void UnRegisterCamera(CinemachineVirtualCamera virtualCamera)
    {
        virtualCamera.gameObject.SetActive(false);
        _ = s_allVirtualCameras.Remove(virtualCamera);
    }
}
