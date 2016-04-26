using System;
namespace MTB
{
	public class BAC_Air : BlockAttributeCalculator
	{

		public BAC_Air ()
		{
		}

		public override BlockType BlockType {
			get {
				return BlockType.Air;
			}
		}

		public override MeshColliderType GetMeshColliderType (byte extendId)
		{
			return MeshColliderType.none;
		}
		
		public override BlockRenderType GetBlockRenderType (byte extendId)
		{
			return BlockRenderType.None;
		}

		public override int LightDamp (byte extendId)
		{
			return 0;
		}
	}
}

