using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game.InputHandling
{
	/// <summary>
	/// This component handles mainly the mouse input of the player
	/// It checks for single left and right mouse button clicks as well as double left button clicks and invokes events
	/// when they happen.
	/// </summary>

	public class InputHandler : IInputHandler, ILateTickable
	{
		// TODO: make these customizable
		/// <summary>
		/// The time in seconds which may elapse at max between 2 clicks to consider it a double-click
		/// </summary>
		private const float DoubleClickThreshold = 0.5f;
		/// <summary>
		/// KeyCode of the modify selection key. Should be customised later via the GameControl settings
		/// </summary>
		private const KeyCode ModifySelectionKey = KeyCode.LeftShift;
		private const KeyCode SaveSelectionGroupKey = KeyCode.LeftControl;

		/// <summary>
		/// The distance in pixels which the user needs to overcome with the mouse button held down before we consider it a 'drag'
		/// </summary>
		private const float DragRectThreshold = 10;
		public event Action OnLeftClickPerformed; 
		public event Action OnDoubleLeftClickPerformed;

		public event Action OnRightClickPerformed;
		public event Action<Rect?> OnSelectionRectChanged;
		public event Action OnDragEndedEvent;

		public event Action<byte> OnSelectionGroupSaved;
		public event Action<byte> OnSelectionGroupRestored;

		/// <summary>
		/// When this is true the current selection will be added to the previous selection, otherwise the selection will be replaced
		/// </summary>
		public bool ModifySelection => _isModifySelectionKeyPressed;
		// has the key been pressed this frame?
		private bool _isModifySelectionKeyPressed = false;
		
		private bool _leftMouseButtonClickPerformed;
		private bool _rightMouseButtonClickPerformed;

		private float _doubleClickTimer;
		private bool _leftMouseButtonDoubleClickPerformed;

		private bool _leftMouseButtonWasDown;
		private Vector3? _mouseDownPosition;
		private Vector3? _currentMousePosition;
		private bool _isDragging;
		private Vector3? _lastFrameMousePosition;
		private Rect? _lastDragRect;

		public void LateTick()
		{
			CheckForKeyboardInput();

			CheckLeftClick();
			CheckLeftDoubleClick();
			CheckRightClick();

			// Order here is important. Check for MouseDrags only after checking for LeftMouseClicks
			CheckForMouseDrag();

			if (_rightMouseButtonClickPerformed)
			{
				// TODO: Check if hit is terrain to give a move command
			}

			InvokeEvents();
			ResetMouseInputs();
		}

		private void InvokeEvents()
		{
			if (_leftMouseButtonClickPerformed)
			{
				OnLeftClickPerformed?.Invoke();
			}
			if (_leftMouseButtonDoubleClickPerformed)
			{
				OnDoubleLeftClickPerformed?.Invoke();
			}
			if (_rightMouseButtonClickPerformed)
			{
				OnRightClickPerformed?.Invoke();
			}
			if (_isDragging && _lastFrameMousePosition != _currentMousePosition)
			{
				OnSelectionRectChanged?.Invoke(_lastDragRect);
			}
		}

		private void ResetMouseInputs()
		{
			_leftMouseButtonDoubleClickPerformed = false;
			_leftMouseButtonClickPerformed = false;
			_rightMouseButtonClickPerformed = false;
		}

		private void CheckForKeyboardInput()
		{
			_isModifySelectionKeyPressed = Input.GetKey(ModifySelectionKey);

			var groupId = GetSelectionGroupKeypress();
			if (groupId >= 0 && Input.GetKey(SaveSelectionGroupKey))
			{
				OnSelectionGroupSaved?.Invoke((byte)groupId);
			}
			else if (groupId >= 0)
			{
				OnSelectionGroupRestored?.Invoke((byte)groupId);
			}
		}

		private static sbyte GetSelectionGroupKeypress()
		{
			sbyte groupId = -1;
			if (Input.GetKeyDown(KeyCode.Alpha0)) groupId = 0;
			else if (Input.GetKeyDown(KeyCode.Alpha1)) groupId = 1;
			else if (Input.GetKeyDown(KeyCode.Alpha2)) groupId = 2;
			else if (Input.GetKeyDown(KeyCode.Alpha3)) groupId = 3;
			else if (Input.GetKeyDown(KeyCode.Alpha4)) groupId = 4;
			else if (Input.GetKeyDown(KeyCode.Alpha5)) groupId = 5;
			else if (Input.GetKeyDown(KeyCode.Alpha6)) groupId = 6;
			else if (Input.GetKeyDown(KeyCode.Alpha7)) groupId = 7;
			else if (Input.GetKeyDown(KeyCode.Alpha8)) groupId = 8;
			else if (Input.GetKeyDown(KeyCode.Alpha9)) groupId = 9;
			return groupId;
		}

		private void CheckForMouseDrag()
		{
			if (Input.GetMouseButtonDown(0))
			{
				_mouseDownPosition = Input.mousePosition;
			}

			if (_mouseDownPosition.HasValue)
			{
				_lastFrameMousePosition = _currentMousePosition;
				_currentMousePosition = Input.mousePosition;
			}

			if (_mouseDownPosition.HasValue && _currentMousePosition.HasValue)
			{
				// Calculate the distance of those positions so that we can skip this if the rectanle is too small
				var distance = Vector3.Distance(_mouseDownPosition.Value, _currentMousePosition.Value);
				// DragRectThreshold is a arbitrary value that defines the minimum size of the rectangle
				_isDragging = distance >= DragRectThreshold;
				if (_isDragging)
				{
					_lastDragRect = GetScreenRect(_mouseDownPosition.Value, _currentMousePosition.Value);
				}
			}
			else
			{
				_isDragging = false;
			}
			// If the mouse button goes up, we need to end the drag
			if (Input.GetMouseButtonUp(0))
			{
				_mouseDownPosition = null;
				_currentMousePosition = null;
				_lastFrameMousePosition = null;
				_isDragging = false;
				_lastDragRect = null;
				OnDragEndedEvent?.Invoke();
			}
		}
		private Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
		{
			// Move origin from bottom left to top left
			screenPosition1.y = Screen.height - screenPosition1.y;
			screenPosition2.y = Screen.height - screenPosition2.y;
			// Calculate corners
			var topLeft = Vector3.Min(screenPosition1, screenPosition2);
			var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
			// Create Rect
			return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
		}

		private void CheckLeftClick()
		{
			if (!_isDragging && Input.GetMouseButtonUp(0) && !IsCursorAboveUi())
			{
				_leftMouseButtonClickPerformed = true;
				_leftMouseButtonWasDown = true;
			}
		}

		private void CheckRightClick()
		{
			if (Input.GetMouseButtonUp(1) && !IsCursorAboveUi())
			{
				_rightMouseButtonClickPerformed = true;
			}
		}

		private void CheckLeftDoubleClick()
		{
			if (!_isDragging && _leftMouseButtonClickPerformed)
			{
				// When the timer is already running and the timer is below the threshold, the user performed a double-click
				if (_doubleClickTimer > 0f && _doubleClickTimer <= DoubleClickThreshold)
				{
					// we need to reset our data to listen for the next double click
					_leftMouseButtonDoubleClickPerformed = true;
					_doubleClickTimer = 0f;
				}
			}
			if (_leftMouseButtonWasDown)
			{
				// increase the timer. When the mouse button goes down the first time, the _doubleClickTimer is equal to 0f
				_doubleClickTimer += Time.deltaTime;
			}
			
			if (_doubleClickTimer > DoubleClickThreshold)
			{
				// Time since the last click exceeded the threshold -> Reset the timer
				_doubleClickTimer = 0f;
				_leftMouseButtonWasDown = false;
			}
		}
		
		private static bool IsCursorAboveUi()
		{
			// Check whether the cursor is above a UI GameObject because we want to prevent clicking through UI
			return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
		}
	}
}