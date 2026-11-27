using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook && Application.isFocused && Cursor.lockState == CursorLockMode.Locked && _ignoreLookFrames == 0)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		
        private int _ignoreLookFrames;

        private void Start()
        {
            SetCursorState(cursorLocked);
        }

        private void Update()
        {
            if (_ignoreLookFrames > 0)
            {
                _ignoreLookFrames--;
                look = Vector2.zero;
            }
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                SetCursorState(false);
                return;
            }
            if (Application.isFocused && Cursor.lockState != CursorLockMode.Locked &&
                Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                SetCursorState(true);
            }
#endif
            if (!Application.isFocused || Cursor.lockState != CursorLockMode.Locked)
                look = Vector2.zero;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            look = Vector2.zero;
            move = Vector2.zero;
            jump = false;
            sprint = false;
            if (!hasFocus) SetCursorState(false);
        }

        private void SetCursorState(bool newState)
        {
            cursorLocked = newState;
            look = Vector2.zero;
            _ignoreLookFrames = 2;
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !newState;
        }
	}
	
}