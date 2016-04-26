using System;
using System.Collections.Generic;
namespace MTB
{
	public class HandBlockInputConditionScript : BaseInputConditionScript
	{
		private List<Block> needBlocks;
		private List<Block> noNeedBlocks;
		private GOPlayerController _playerController;
		public HandBlockInputConditionScript (GameObjectController gameObjectController)
			:base(gameObjectController)
		{
			_playerController = gameObjectController as GOPlayerController;
		}

		public override void SetParam (System.Collections.Generic.Dictionary<string, string> param)
		{
			needBlocks = new List<Block>();
			if(param.ContainsKey("needBlocks"))
			{
				string[] blockStrs = param["needBlocks"].Split('#');
				for (int i = 0; i < blockStrs.Length; i++) {
					string[] blockStr = blockStrs[i].Split(',');
					BlockType type = (BlockType)Convert.ToInt32(blockStr[0]);
					byte extendId = Convert.ToByte(blockStr[1]);
					needBlocks.Add(new Block(type,extendId));
				}
			}
			noNeedBlocks = new List<Block>();
			if(param.ContainsKey("noNeedBlocks"))
			{
				string[] blockStrs = param["noNeedBlocks"].Split('#');
				for (int i = 0; i < blockStrs.Length; i++) {
					string[] blockStr = blockStrs[i].Split(',');
					BlockType type = (BlockType)Convert.ToInt32(blockStr[0]);
					byte extendId = Convert.ToByte(blockStr[1]);
					noNeedBlocks.Add(new Block(type,extendId));
				}
			}
		}

		public override bool MeetCondition ()
		{
			int id = _playerController.playerAttribute.handMaterialId;
			if(id >= 1000)return false;
			BlockType t = (BlockType)id;
			for (int i = 0; i < needBlocks.Count; i++) {
				if(needBlocks[i].BlockType == t)
				{
					for (int j = 0; j < noNeedBlocks.Count; j++) {
						if(noNeedBlocks[i].BlockType == t)return false;
					}
					return true;
				}
			}
			return false;
		}
	}
}

