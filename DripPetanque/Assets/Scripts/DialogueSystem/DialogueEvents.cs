using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DialogueEvents : MonoBehaviour
{
    private CinemachineVirtualCamera m_lastActivatedVirtualCam;

    public CinemachineVirtualCamera LastActivatedVirtualCam
    {
        get { return m_lastActivatedVirtualCam; }
        set { m_lastActivatedVirtualCam = value;}
    }

    public void SwitchToCam(CinemachineVirtualCamera virtualCameraToActivate)
    {
        //TODO : Gérer le skip de la phrase par le joueur : Interrompre le blend et faire un cut direct si le joueur appuie 2 fois sur A avant la fin de la phrase)
        if (m_lastActivatedVirtualCam != null)
        {
            m_lastActivatedVirtualCam.gameObject.SetActive(false);
        }

        virtualCameraToActivate.gameObject.SetActive(true);

        m_lastActivatedVirtualCam = virtualCameraToActivate;
    }
}
