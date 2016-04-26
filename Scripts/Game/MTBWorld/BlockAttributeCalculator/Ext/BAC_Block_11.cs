using System;
namespace MTB
{
	public class BAC_Block_11 : BlockAttributeCalculator
	{
		#region implemented abstract members of BlockAttributeCalculator

		public override BlockType BlockType {
			get {
				return BlockType.Block_11;
			}
		}

		#endregion

		public BAC_Block_11 ()
		{
		}
	}
}

