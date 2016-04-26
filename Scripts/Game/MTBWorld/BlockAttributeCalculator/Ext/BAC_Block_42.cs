using System;
namespace MTB
{
	public class BAC_Block_42 : BAC_CrossBlock
	{
		#region implemented abstract members of BlockAttributeCalculator

		public override BlockType BlockType {
			get {
				return BlockType.Block_42;
			}
		}

		#endregion

		public BAC_Block_42 ()
		{
		}
	}
}

