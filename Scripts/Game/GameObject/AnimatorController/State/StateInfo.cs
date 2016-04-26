using System;
namespace MTB
{
	public class StateInfo
	{
		public readonly int fullNameHash;
		public readonly int layerIndex;
		public StateInfo (int fullNameHash,int layerIndex)
		{
			this.fullNameHash = fullNameHash;
			this.layerIndex = layerIndex;
		}
	}
}

