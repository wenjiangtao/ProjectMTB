using System;
namespace MTB
{
	public class Decoration_Block_43 : Decoration_SingleBlock
	{
		#region implemented abstract members of Decoration_SingleBlock

		public override Block GetDecoration ()
		{
			return new Block(BlockType.Block_43);
		}

		#endregion

		public Decoration_Block_43 ()
		{
		}
	}
}

