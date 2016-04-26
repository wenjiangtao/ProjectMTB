using System;
namespace MTB
{
    public class Decoration_EyeFruit_Seedling : Decoration_SingleBlock
    {
           #region implemented abstract members of Decoration_SingleBlock

		public override Block GetDecoration ()
		{
            return new Block(BlockType.Block_86);
		}

		#endregion

        public Decoration_EyeFruit_Seedling()
		{
            _decorationType = DecorationType.EyeFruit_Seedling;
		}
    }
}
