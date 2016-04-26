using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MTB
{
	[Serializable]
	public struct Block
	{
		public static Block NullBlock = new Block(BlockType.Null);
		public static Block AirBlock = new Block(BlockType.Air);
		public BlockType BlockType;
		public byte ExtendId;
	    public Block(BlockType type)
	    {
			BlockType = type;
			ExtendId = 0;
	    }

		public Block(BlockType type,byte extendId)
		{
			BlockType = type;
			ExtendId = extendId;
		}

		public override bool Equals (object obj)
		{
			if(obj is Block)
			{
				Block other = (Block)obj;
				return (BlockType == other.BlockType && ExtendId == other.ExtendId);
			}
			return false;
		}

		public bool EqualOther (Block other)
		{
			return (BlockType == other.BlockType && ExtendId == other.ExtendId);
		}
	}
}
