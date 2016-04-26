using System;
namespace MTB
{
    public class Decoration_SporeTree_Seedling : Decoration_SingleBlock
    {
        #region implemented abstract members of Decoration_SingleBlock

        public override Block GetDecoration()
        {
            return new Block(BlockType.Block_86);
        }

        #endregion

        public Decoration_SporeTree_Seedling()
        {
            _decorationType = DecorationType.SporeTree_Seedling;
        }
    }
}
