using System;
namespace MTB
{
	public class BAC_Block_12 : BlockAttributeCalculator
	{
		#region implemented abstract members of BlockAttributeCalculator

		public override BlockType BlockType {
			get {
				return BlockType.Block_12;
			}
		}

		#endregion

		public BAC_Block_12 ()
		{
		}
	}
}

