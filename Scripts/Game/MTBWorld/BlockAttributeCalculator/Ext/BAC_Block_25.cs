using System;
namespace MTB
{
	public class BAC_Block_25 : BlockAttributeCalculator
	{
		#region implemented abstract members of BlockAttributeCalculator

		public override BlockType BlockType {
			get {
				return BlockType.Block_25;
			}
		}

		#endregion

		public BAC_Block_25 ()
		{
		}
	}
}

