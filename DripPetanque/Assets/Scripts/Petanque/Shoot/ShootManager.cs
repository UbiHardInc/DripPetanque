using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootManager : BaseShootManager<ShootStep, ControllableBall>
{
    protected override PetanqueSubGameManager.PetanquePlayers Owner => PetanqueSubGameManager.PetanquePlayers.Human;

    [SerializeField] private Transform m_arrow;
    [SerializeField] private Transform m_arrowPivot;
    [SerializeField] private InputActionReference m_startShootInput;


    protected override void Init()
    {
        base.Init();

        m_leftRightStep.Init(m_arrowPivot);
        m_forceStep.Init(m_arrow);
        m_upDownStep.Init(m_arrowPivot);

        //m_startShootInput.action.performed += StartShoot;
    }

    public void StartShoot(InputAction.CallbackContext _)
    {
        StartShoot();
    }

    protected override void StartSteps()
    {
        base.StartSteps();

        m_arrow.gameObject.SetActive(true);
    }

    protected override void LaunchBall()
    {
        base.LaunchBall();

        m_arrow.gameObject.SetActive(false);
        for (int i = m_allSteps.Length - 1; i >= 0; i--)
        {
            m_allSteps[i].ResetArrow();
        }
    }
}
