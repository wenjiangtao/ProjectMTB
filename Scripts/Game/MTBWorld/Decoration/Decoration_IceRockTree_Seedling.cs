using System;
namespace MTB
{
    public class Decoration_IceRockTree_Seedling : Decoration_SingleBlock
    {
         #region implemented abstract members of Decoration_SingleBlock

		public override Block GetDecoration ()
		{
            return new Block(BlockType.Block_86);
		}

		#endregion

        public Decoration_IceRockTree_Seedling()
		{
            _decorationType = DecorationType.IceRockTree_Seedling;
		}
    }
}
