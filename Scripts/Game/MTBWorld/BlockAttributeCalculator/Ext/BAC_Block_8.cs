using System;
namespace MTB
{
	public class BAC_Block_8 : BlockAttributeCalculator
	{
		#region implemented abstract members of BlockAttributeCalculator

		public override BlockType BlockType {
			get {
				return BlockType.Block_8;
			}
		}

		#endregion

		public BAC_Block_8 ()
		{
		}
	}
}

