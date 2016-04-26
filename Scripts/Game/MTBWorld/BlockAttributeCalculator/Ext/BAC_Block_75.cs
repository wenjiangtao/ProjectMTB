using System;
namespace MTB
{
	public class BAC_Block_75 : BlockAttributeCalculator
	{
		#region implemented abstract members of BlockAttributeCalculator

		public override BlockType BlockType {
			get {
				return BlockType.Block_75;
			}
		}

//		protected override UnityEngine.Rect GetUVRect (byte extendId, Direction direction)
//		{
//			Direction resultDirection;
//			switch(extendId)
//			{
//			case 1:
//				if(direction == Direction.front)
//				{
//					resultDirection = Direction.right;
//				}
//				else if(direction == Direction.right)
//				{
//					resultDirection = Direction.back;
//				}
//				else if(direction == Direction.back)
//				{
//					resultDirection = Direction.left;
//				}
//				else if(direction == Direction.left)
//				{
//					resultDirection = Direction.front;
//				}
//				else
//				{
//					resultDirection  = direction;
//				}
//				break;
//			case 2:
//				if(direction == Direction.front)
//				{
//					resultDirection = Direction.back;
//				}
//				else if(direction == Direction.right)
//				{
//					resultDirection = Direction.left;
//				}
//				else if(direction == Direction.back)
//				{
//					resultDirection = Direction.front;
//				}
//				else if(direction == Direction.left)
//				{
//					resultDirection = Direction.right;
//				}
//				else
//				{
//					resultDirection  = direction;
//				}
//				break;
//			case 3:
//				if(direction == Direction.front)
//				{
//					resultDirection = Direction.left;
//				}
//				else if(direction == Direction.right)
//				{
//					resultDirection = Direction.front;
//				}
//				else if(direction == Direction.back)
//				{
//					resultDirection = Direction.right;
//				}
//				else if(direction == Direction.left)
//				{
//					resultDirection = Direction.back;
//				}
//				else
//				{
//					resultDirection  = direction;
//				}
//				break;
//			default:
//				resultDirection = direction;
//				break;
//			}
//			return base.GetUVRect (0, resultDirection);
//		}

		public override Direction GetFaceDirection (byte extendId)
		{
			if(extendId == 1)return Direction.right;
			else if(extendId == 2) return Direction.back;
			else if(extendId == 3) return Direction.left;
			return Direction.front;
		}

		public override byte GetResourceExtendId (byte extendId)
		{
			return 0;
		}

		#endregion

		public BAC_Block_75 ()
		{
		}
	}
}

