using System;
namespace MTB
{
	public class BAC_Block_48 : BAC_CrossBlock
	{
		#region implemented abstract members of BAC_CrossBlock

		public override BlockType BlockType {
			get {
				return BlockType.Block_48;
			}
		}

		public override int LightLevel (byte extendId)
		{
			return 14;
		}

		#endregion

		public BAC_Block_48 ()
		{
		}
	}
}

