using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtility.CustomAttributes;

namespace StarterAssets
{
    [Serializable]
	public class PlayerInput
    {
        public Vector2 Move => m_move;
        public Vector2 Look => m_look;
        public bool Jump { get => m_jump; set => m_jump = value; }
        public bool Sprint => m_sprint;
        public bool AnalogMovement => m_analogMovement;

        [Title("Inputs")]
        [SerializeField] private InputActionReference m_moveAction;
        [SerializeField] private InputActionReference m_lookAction;
        [SerializeField] private InputActionReference m_jumpAction;
        [SerializeField] private InputActionReference m_sprintAction;

		[Title("Movement Settings")]
        [SerializeField] private bool m_analogMovement;

		[Title("Mouse Cursor Settings")]
        [SerializeField] private bool m_cursorLocked = true;
        [SerializeField] private bool m_cursorInputForLook = true;

        [NonSerialized] private Vector2 m_move = Vector2.zero;
        [NonSerialized] private Vector2 m_look = Vector2.zero;
        [NonSerialized] private bool m_jump = false;
        [NonSerialized] private bool m_sprint = false;

        public void Init()
        {
            Application.focusChanged += OnApplicationFocus;

            m_moveAction.action.performed += OnMoveActionPerformed;
            m_lookAction.action.performed += OnLookActionPerformed;
            m_jumpAction.action.performed += OnJumpActionPerformed;
            m_sprintAction.action.performed += OnSprintActionPerformed;
        }

        public void Dispose()
        {
            Application.focusChanged -= OnApplicationFocus;

            m_moveAction.action.performed -= OnMoveActionPerformed;
            m_lookAction.action.performed -= OnLookActionPerformed;
            m_jumpAction.action.performed -= OnJumpActionPerformed;
            m_sprintAction.action.performed -= OnSprintActionPerformed;
        }

		public void OnMoveActionPerformed(InputAction.CallbackContext value)
		{
			MoveInput(value.ReadValue<Vector2>());
		}

		public void OnLookActionPerformed(InputAction.CallbackContext value)
        {
			if (m_cursorInputForLook)
			{
				LookInput(value.ReadValue<Vector2>());
			}
		}

		public void OnJumpActionPerformed(InputAction.CallbackContext value)
        {
			JumpInput(value.ReadValueAsButton());
		}

		public void OnSprintActionPerformed(InputAction.CallbackContext value)
        {
			SprintInput(value.ReadValueAsButton());
		}


		public void MoveInput(Vector2 newMoveDirection)
		{
			m_move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			m_look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			m_jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			m_sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(m_cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}