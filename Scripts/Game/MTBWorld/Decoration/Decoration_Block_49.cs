using System;
namespace MTB
{
	public class Decoration_Block_49 : Decoration_SingleBlock
	{
		#region implemented abstract members of Decoration_SingleBlock

		public override Block GetDecoration ()
		{
			return new Block(BlockType.Block_49);
		}

		#endregion

		public Decoration_Block_49 ()
		{
            _decorationType = DecorationType.Block_49;
		}
	}
}

