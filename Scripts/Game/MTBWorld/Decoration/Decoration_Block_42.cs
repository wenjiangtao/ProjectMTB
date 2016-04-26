using System;
namespace MTB
{
	public class Decoration_Block_42 : Decoration_SingleBlock
	{
		#region implemented abstract members of Decoration_SingleBlock

		public override Block GetDecoration ()
		{
			return new Block(BlockType.Block_42);
		}

		#endregion

		public Decoration_Block_42 ()
		{
            _decorationType = DecorationType.Block_42;
		}
	}
}

