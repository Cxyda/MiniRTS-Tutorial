namespace Game.AssetLoading
{
	/// <summary>
	/// In this enum we can add all Asset Types we have in our game.
	/// An AssetType represents a group or folder which is used as a path for the Addressables.
	/// </summary>
	public enum AssetType : byte
	{
		// If we create a new assetType without defining it, it will have the `Invalid` value to signal
		// that we need to define it.
		Invalid = 0,
		GameObject = 1,
		Config = 50,
		UiIcon = 100,
	}
}