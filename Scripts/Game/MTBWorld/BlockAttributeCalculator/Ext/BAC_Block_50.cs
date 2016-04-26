using System;
namespace MTB
{
	public class BAC_Block_50 : BlockAttributeCalculator
	{
		#region implemented abstract members of BlockAttributeCalculator

		public override BlockType BlockType {
			get {
				return BlockType.Block_50;
			}
		}

		#endregion

		public BAC_Block_50 ()
		{
		}
	}
}

