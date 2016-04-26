using System;
namespace MTB
{
    public class Decoration_Block_42_Seedling : Decoration_SingleBlock
    {
        #region implemented abstract members of Decoration_SingleBlock

        public override Block GetDecoration()
        {
            return new Block(BlockType.Block_86);
        }

        #endregion

        public Decoration_Block_42_Seedling()
        {
            _decorationType = DecorationType.Block_42_Seedling;
        }
    }
}
