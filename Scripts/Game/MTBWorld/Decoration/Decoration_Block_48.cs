using System;
namespace MTB
{
	public class Decoration_Block_48 : Decoration_SingleBlock
	{
		#region implemented abstract members of Decoration_SingleBlock

		public override Block GetDecoration ()
		{
			return new Block(BlockType.Block_48);
		}

		#endregion

		public Decoration_Block_48 ()
		{
		}
	}
}

