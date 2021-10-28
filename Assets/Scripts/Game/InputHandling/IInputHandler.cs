using System;
using UnityEngine;

namespace Game.InputHandling
{
	public interface IInputHandler
	{
		event Action OnLeftClickPerformed;
		event Action OnDoubleLeftClickPerformed;
		event Action OnRightClickPerformed;
		event Action<Rect?> OnSelectionRectChanged;
		event Action OnDragEndedEvent;
		public event Action<byte> OnSelectionGroupSaved;
		public event Action<byte> OnSelectionGroupRestored;

		bool ModifySelection { get; }
	}
}