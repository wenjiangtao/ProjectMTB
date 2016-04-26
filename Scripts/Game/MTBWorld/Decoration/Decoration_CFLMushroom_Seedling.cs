using System;
namespace MTB
{

    public class Decoration_CFLMushroom_Seedling : Decoration_SingleBlock
    {
        #region implemented abstract members of Decoration_SingleBlock

        public override Block GetDecoration()
        {
            return new Block(BlockType.Block_86);
        }

        #endregion

        public Decoration_CFLMushroom_Seedling()
        {
            _decorationType = DecorationType.CFLMushroom_Seedling;
        }
    }
}
