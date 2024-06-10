using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private LayerMask m_whatIsBall;

    [Header("Cameras")]
    [SerializeField] protected CinemachineVirtualCamera m_pintanqueOverviewCam;
    [SerializeField] protected CinemachineVirtualCamera m_embutOverviewCam;

    private void Awake()
    {
        VirtualCamerasManager.RegisterCamera(m_pintanqueOverviewCam);
        VirtualCamerasManager.RegisterCamera(m_embutOverviewCam);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == m_whatIsBall)
        {
            VirtualCamerasManager.SwitchToCamera(m_embutOverviewCam);
        }
    }
}
