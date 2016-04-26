using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
    public class Decoration_PurpleEraserTree_Seedling : Decoration_SingleBlock
    {
         #region implemented abstract members of Decoration_SingleBlock

		public override Block GetDecoration ()
		{
            return new Block(BlockType.Block_86);
		}

		#endregion

        public Decoration_PurpleEraserTree_Seedling()
		{
            _decorationType = DecorationType.PurpleEraserTree_Seedling;
		}
    }
}
