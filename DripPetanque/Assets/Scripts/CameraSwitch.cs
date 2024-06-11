using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityUtility.CustomAttributes;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField, Layer] private int m_whatIsBall;

    [Header("Cameras")]
    [SerializeField] protected CinemachineVirtualCamera m_pintanqueOverviewCam;
    [SerializeField] protected CinemachineVirtualCamera m_embutOverviewCam;

    private void Awake()
    {
        VirtualCamerasManager.RegisterCamera(m_pintanqueOverviewCam);
        VirtualCamerasManager.RegisterCamera(m_embutOverviewCam);
    }

    private void OnDestroy()
    {
        VirtualCamerasManager.UnRegisterCamera(m_pintanqueOverviewCam);
        VirtualCamerasManager.UnRegisterCamera(m_embutOverviewCam);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("DETECTION TRIGGER");
        if (other.gameObject.layer == m_whatIsBall)
        {
            VirtualCamerasManager.SwitchToCamera(m_embutOverviewCam);
        }
    }
}
