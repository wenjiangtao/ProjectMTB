using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class WallOfAirManager : Singleton<WallOfAirManager>
	{
		private static string wallOfAirPrefabPath = "Prefabs/WallOfAir";
		private Dictionary<int,MovedWallOfAir> map;
		private Dictionary<int,GameObject> fixedMap;

		public void Init()
		{
			map = new Dictionary<int, MovedWallOfAir>();
			fixedMap = new Dictionary<int, GameObject>();
			InitBound();
		}

		private List<int> list = new List<int>();
		private void InitBound()
		{
			if(WorldConfig.Instance.hasBorder)
			{
				int left = WorldConfig.Instance.borderLeft;
				int right = WorldConfig.Instance.borderRight;
				int back = WorldConfig.Instance.borderBack;
				int front = WorldConfig.Instance.borderFront;
				int leftWall = InitFixedWallOfAir(new Vector3(left,128,0),new Vector3(1,256,(front - back)));
				int rightWall = InitFixedWallOfAir(new Vector3(right,128,0),new Vector3(1,256,(front - back)));
				int backWall = InitFixedWallOfAir(new Vector3(0,128,back),new Vector3((right - left) ,256,1));
				int frontWall = InitFixedWallOfAir(new Vector3(0,128,front),new Vector3((right - left),256,1));
				list.Add(leftWall);
				list.Add(rightWall);
				list.Add(backWall);
				list.Add(frontWall);
			}
		}

		public void RegisterMovedWallOfAir(Transform target,float height,Vector3 scale)
		{
			if(map.ContainsKey(target.GetInstanceID()))return;
			GameObject wallOfAirObj = NewWallOfAir(new Vector3(target.position.x,height,target.position.z));
			wallOfAirObj.transform.localScale = scale;
			MovedWallOfAir wallOfAir = wallOfAirObj.AddComponent<MovedWallOfAir>();
			wallOfAir.SetTarget(target);
			map.Add(target.GetInstanceID(),wallOfAir);
		}

		public void UnRegisterMovedWallOfAir(Transform target)
		{
			MovedWallOfAir wallOfAir;
			map.TryGetValue(target.GetInstanceID(),out wallOfAir);
			if(wallOfAir != null)
			{
				map.Remove(target.GetInstanceID());
				wallOfAir.Dispose();
				GameObject.Destroy(wallOfAir.gameObject);
			}
		}

		private GameObject NewWallOfAir(Vector3 position)
		{
			GameObject gameObject = Resources.Load<GameObject>(wallOfAirPrefabPath);
			GameObject wallOfAirObj = GameObject.Instantiate(gameObject,new Vector3(position.x,position.y,position.z),Quaternion.identity) as GameObject;
			wallOfAirObj.transform.parent = transform;
			return wallOfAirObj;
		}

		public int InitFixedWallOfAir(Vector3 position,Vector3 scale)
		{
			GameObject wallOfAirObj = NewWallOfAir(position);
			wallOfAirObj.transform.localScale = scale;
			fixedMap.Add(wallOfAirObj.GetInstanceID(),wallOfAirObj);
			return wallOfAirObj.GetInstanceID();
		}

		public void RemoveFixedWallOfAir(int id)
		{
			GameObject gameObject;
			fixedMap.TryGetValue(id,out gameObject);
			if(gameObject != null)
			{
				GameObject.Destroy(gameObject);
			}
		}

		void Destroy()
		{
			map.Clear();
			for (int i = 0; i < list.Count; i++) {
				RemoveFixedWallOfAir(list[i]);
			}
			fixedMap.Clear();
		}
	}
}

