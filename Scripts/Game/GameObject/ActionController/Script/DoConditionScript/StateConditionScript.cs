using System;
using System.Collections.Generic;
namespace MTB
{
	public class StateConditionScript : BaseDoConditionScript
	{
		//0表示不关注是否在地面或空中，-1表示需要在地面，1表示需要在空中
		private int curGroundState;
		private List<Block> standBlocks;
		private List<Block> inBlocks;
		public StateConditionScript (GameObjectController gameObjectController)
			:base(gameObjectController)
		{
		}

		public override void SetParam (System.Collections.Generic.Dictionary<string, string> param)
		{
			curGroundState = 0;
			standBlocks = new List<Block>();
			inBlocks = new List<Block>();

			if(param.ContainsKey("needGround"))
			{
				bool needGround = Convert.ToBoolean(param["needGround"]);
				if(needGround)curGroundState = -1;
				else curGroundState = 1;
			}

			if(param.ContainsKey("standBlocks"))
			{
				string[] blockStrs = param["standBlocks"].Split('#');
				for (int i = 0; i < blockStrs.Length; i++) {
					string[] blockStr = blockStrs[i].Split(',');
					BlockType type = (BlockType)Convert.ToInt32(blockStr[0]);
					byte extendId = Convert.ToByte(blockStr[1]);
					standBlocks.Add(new Block(type,extendId));
				}
			}

			if(param.ContainsKey("inBlocks"))
			{
				string[] blockStrs = param["inBlocks"].Split('#');
				for (int i = 0; i < blockStrs.Length; i++) {
					string[] blockStr = blockStrs[i].Split(',');
					BlockType type = (BlockType)Convert.ToInt32(blockStr[0]);
					byte extendId = Convert.ToByte(blockStr[1]);
					inBlocks.Add(new Block(type,extendId));
				}
			}
		}

		public override bool MeetCondition ()
		{
			if(curGroundState == -1 && !_gameObjectController.gameObjectState.IsGround)return false;
			if(curGroundState == 1 && _gameObjectController.gameObjectState.IsGround)return false;
			if(standBlocks.Count > 0)
			{
				int i = 0;
				for (i = 0; i < standBlocks.Count; i++) {
					if(standBlocks[i].EqualOther(_gameObjectController.gameObjectState.StandBlock))break;
				}
				if(i >= standBlocks.Count)
				{
					return false;
				}
			}
			if(inBlocks.Count > 0)
			{
				int i = 0;
				for (i = 0; i < inBlocks.Count; i++) {
					if(inBlocks[i].EqualOther(_gameObjectController.gameObjectState.InBlock))break;
				}
				if(i >= inBlocks.Count)
				{
					return false;
				}
			}
			return true;
		}
	}
}

