namespace Game.Selection
{
	public interface ISelectable
	{
		bool IsSelected { get; }
		void Select(bool select);
	}
}