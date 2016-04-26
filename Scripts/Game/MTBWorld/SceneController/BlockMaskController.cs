using System;
using UnityEngine;
namespace MTB
{
	public class BlockMaskController : Singleton<BlockMaskController>
	{
		private GameObject maskObj;
		public void Init()
		{
			maskObj = GameObject.Instantiate(Resources.Load("Prefabs/BlockColorMask") as GameObject) as GameObject;
			maskObj.transform.parent = this.transform;
			HideMaskObj();
			StopDo();
		}

		public void Do(float screenX,float screenY,Vector3 position,float distance)
		{
			RaycastHit hit;
			int mask = LayerMask.GetMask(new string[]{"TerrainMesh","SupportColliderMesh"});
			if(Terrain.RayToWorld(screenX,screenY,position,distance,out hit,mask))
			{
				WorldPos pos = Terrain.GetWorldPos(hit,false);
				if(hit.collider.GetComponentInParent<ChunkObj>() == null)return;
				ShowMaskObj(pos);
			}
			else
			{
				HideMaskObj();
			}
		}

		public void StopDo()
		{
			HideMaskObj();
		}

		private void ShowMaskObj(WorldPos pos)
		{
			if(MineController.Instance.IsMining())
			{
				HideMaskObj();
				return;
			}
			maskObj.transform.position = new Vector3(pos.x + 0.5f,pos.y + 0.5f,pos.z + 0.5f);
			if(!maskObj.activeSelf)
			{
				maskObj.SetActive (true);
			}
		}

		private void HideMaskObj()
		{
			if(maskObj.activeSelf)
			{
				maskObj.SetActive (false);
			}
		}

		public bool IsBlockMasking()
		{
			return maskObj.activeSelf;
		}

		void OnDestroy()
		{
			maskObj = null;
		}
	}
}

