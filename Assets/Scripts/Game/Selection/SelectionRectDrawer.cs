using Game.InputHandling;
using UnityEngine;
using Zenject;

namespace Game.Selection
{
	/// <summary>
	/// This class handles the drawing of the a selection rectangle when the player drags the mouse across the screen
	/// </summary>
	public class SelectionRectDrawer : MonoBehaviour
	{
		// We use Zenject to inject the InputHandler class
		[Inject] private IInputHandler _inputHandler;

		[Tooltip("The texture of the selection rect")]
		public Texture2D DragRectTexture;
		
		private Rect? _rect;
		private GUIStyle _dragRectStyle;

		private void Awake()
		{
			// register for the OnSelectionRectChanged event and call UpdateSelectionRect() when it happens
			_inputHandler.OnSelectionRectChanged += UpdateSelectionRect;
			_inputHandler.OnDragEndedEvent += ResetRect;

			_dragRectStyle = new GUIStyle("box")
			{
				normal =
				{
					// assign the rect texture
					background = DragRectTexture
				},
				// define 9-slicing borders
				border = new RectOffset(4, 4, 4, 4)
			};
		}
		private void UpdateSelectionRect(Rect? dragRect)
		{
			_rect = dragRect;
		}
		private void OnGUI()
		{
			if (_rect != null)
			{
				GUI.Box(_rect.Value, "", _dragRectStyle);
			}
		}
		private void ResetRect()
		{
			// reset the rectangle every frame to not draw the texture on the next frame anymore when the player has stopped dragging
			_rect = null;
		}
	}
}