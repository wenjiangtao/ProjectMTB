using System;
namespace MTB
{
	public class BAC_Block_1 : BlockAttributeCalculator
	{
		#region implemented abstract members of BlockAttributeCalculator

		public override BlockType BlockType {
			get {
				return BlockType.Block_1;
			}
		}

		#endregion

		public BAC_Block_1 ()
		{
		}
	}
}

