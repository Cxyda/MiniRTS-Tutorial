using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.BuildMode
{
	/// <summary>
	/// This struct allows to map a construction site model to a construction progress
	/// </summary>
	[Serializable]
	public struct ConstructionSiteViewData
	{
		/// <summary>
		/// The construction progress in percent (%)
		/// </summary>
		[Range(0, 100)] public int Progress;
		public AssetReference Asset;
	}
}