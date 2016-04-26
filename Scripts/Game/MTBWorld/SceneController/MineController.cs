using System;
using UnityEngine;
namespace MTB
{
	public class MineController : Singleton<MineController>
	{
		private const string MINE_SPLIT_PATH = "Prefabs/MineSplitBox";
		private SceneBlock sceneBlock;
		private bool needCheckMinePower;
		private GameObject mineSplitObj;
		private Renderer mineSplitRender;

		public void Init()
		{
			needCheckMinePower = !WorldConfig.Instance.isCreateMode;
			mineSplitObj = GameObject.Instantiate(Resources.Load(MINE_SPLIT_PATH) as GameObject) as GameObject;
			mineSplitObj.transform.parent = this.transform;
			mineSplitRender = mineSplitObj.GetComponent<Renderer>();
			HideMineSplitObj();
		}

		public void StartMine()
		{
			sceneBlock = null;
		}

		public bool Mine(int handId,WorldPos pos,Block block)
		{
			if(needCheckMinePower)
			{
				if(sceneBlock == null || sceneBlock.blockType != (byte)block.BlockType || sceneBlock.extendId != block.ExtendId
				   || !sceneBlock.pos.EqualOther(pos))
				{
					sceneBlock = new SceneBlock(pos,(byte)block.BlockType,block.ExtendId);
				}
				Item item  = ItemManager.Instance.GetItem(handId);
				if(item != null)
				{
					SpecialIDMinePower iDPower = sceneBlock.blockData.GetSpecialIDMinePower(item.id);
					if(iDPower != null)
					{
						return MineSceneBlock(pos,iDPower.power);
					}
					SpecialTypeMinePower typePower = sceneBlock.blockData.GetSpecialTypeMinePower(item.type);
					if(typePower != null)
					{
						return MineSceneBlock(pos,typePower.power);
					}
				}
				return MineSceneBlock(pos,sceneBlock.blockData.normalMinePower);
			}
			else
			{
				return true;
			}
		}

		private bool MineSceneBlock(WorldPos pos,int power)
		{
			sceneBlock.curHardness -= power;
			if(sceneBlock.curHardness <= 0)
			{
				StopMine();
				HideMineSplitObj();
				return true;
			}
			int minedHardness = sceneBlock.blockData.hardness - sceneBlock.curHardness;
			float harnessPerPic = sceneBlock.blockData.hardness / 4f;
			int picIndex = Mathf.FloorToInt(minedHardness / harnessPerPic);
			ShowMineSplitObj(pos,picIndex);
			return false;
		}

		private void ShowMineSplitObj(WorldPos pos,int picIndex)
		{
			if(BlockMaskController.Instance.IsBlockMasking())
			{
				BlockMaskController.Instance.StopDo();
			}
			mineSplitObj.transform.position = new Vector3(pos.x + 0.5f,pos.y + 0.5f,pos.z + 0.5f);
			if(!mineSplitObj.activeSelf)
			{
				mineSplitObj.SetActive (true);
			}
			int sunLight = World.world.GetSunLight(pos.x,pos.y,pos.z);
			int blockLight = World.world.GetBlockLight(pos.x,pos.y,pos.z);
			float lightIntensity = MTBSkyBox.Instance.GetLightIntensity(sunLight,blockLight);
			mineSplitRender.material.SetFloat("_lightIntensity",lightIntensity);
			mineSplitRender.material.SetFloat("_TexIndex",(float)picIndex);
		}
		
		private void HideMineSplitObj()
		{
			if(mineSplitObj.activeSelf)
			{
				mineSplitObj.SetActive (false);
			}
			mineSplitRender.material.SetFloat("_TexIndex",0);
		}

		public void StopMine()
		{
			sceneBlock = null;
			HideMineSplitObj();
		}

		public bool IsMining()
		{
			return mineSplitObj.activeSelf;
		}

		void OnDestroy()
		{
			mineSplitObj = null;
			mineSplitRender = null;
		}
	}
}

