using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace MTB
{
	public class WorldTextureProvider : Singleton<WorldTextureProvider>
	{
//		private Dictionary<int,Dictionary<Direction,string>> _worldTexMap;

		public static bool IsCanUseProvider = false;

		private Dictionary<WorldTextureType,string> _pathMap;
		private Dictionary<WorldTextureType,PackTexture> _packTextureMap;
		private Dictionary<int,WorldTextureType> _idToTextureTypeMap;

		private List<int> _idKeyList;
		private List<int> _picKeyList;
		private List<PackTexture> _listPackTexture;

		private PackTexture _opaqueTilePackTexture;
		private PackTexture _opaqueNoTilePackTexture;
		private PackTexture _transparentTilePackTexture;
		private PackTexture _transparentNoTilePackTexture;
		void Awake()
		{
			_pathMap = new Dictionary<WorldTextureType, string>();
			_pathMap.Add(WorldTextureType.OpaqueTileTex,"Texture/MTBWorld/Opaque/Tile/");
			_pathMap.Add(WorldTextureType.OpaqueNoTileTex,"Texture/MTBWorld/Opaque/NoTile/");
			_pathMap.Add(WorldTextureType.TransparentTileTex,"Texture/MTBWorld/Transparent/Tile/");
			_pathMap.Add(WorldTextureType.TransparentNoTileTex,"Texture/MTBWorld/Transparent/NoTile/");
			_packTextureMap = new Dictionary<WorldTextureType, MTB.PackTexture>();
//			_worldTexMap = new Dictionary<int,Dictionary<Direction, string>>();
			_idToTextureTypeMap = new Dictionary<int, WorldTextureType>();
			_listPackTexture = new List<MTB.PackTexture>();

			_idKeyList = new List<int>();
			_picKeyList = new List<int>();
		}

		private const int PicSize = 2048;
		public void Pack()
		{
			_opaqueTilePackTexture = new PackTexture(WorldTextureType.OpaqueTileTex,PicSize,PicSize,TextureFormat.ETC_RGB4,true);
			PackTexture(_opaqueTilePackTexture,_pathMap[WorldTextureType.OpaqueTileTex],WorldTextureType.OpaqueTileTex);

			_opaqueNoTilePackTexture = new PackTexture(WorldTextureType.OpaqueNoTileTex,PicSize,PicSize,TextureFormat.DXT1);
			PackTexture(_opaqueNoTilePackTexture,_pathMap[WorldTextureType.OpaqueNoTileTex],WorldTextureType.OpaqueNoTileTex);

			_transparentTilePackTexture = new PackTexture(WorldTextureType.TransparentTileTex,PicSize,PicSize,TextureFormat.DXT5,true);
			PackTexture(_transparentTilePackTexture,_pathMap[WorldTextureType.TransparentTileTex],WorldTextureType.TransparentTileTex);

			_transparentNoTilePackTexture = new PackTexture(WorldTextureType.TransparentNoTileTex,PicSize,PicSize,TextureFormat.DXT5);
			PackTexture(_transparentNoTilePackTexture,_pathMap[WorldTextureType.TransparentNoTileTex],WorldTextureType.TransparentNoTileTex);

			Resources.UnloadUnusedAssets();
			IsCanUseProvider = true;
		}
		//一个文件夹的图片数量不超过1024个
		private int GetPackTextureKey(int id,int picExtendKey)
		{
			return (id << 10) + (picExtendKey & 1023);
		}

		private int GetIdKey(int id,Direction direction)
		{
			//因为direction有负值，因此需要把结果变成正值
			return (id << 10)+ (((int)direction + 6) & 1023);
		}

		private void PackTexture(PackTexture packTexture,string path,WorldTextureType type)
		{
			var gameObjects = Resources.LoadAll<Texture2D>(path);
			int picExtendKey = 0;
			foreach (var item in gameObjects) {
				string[] result = item.name.Split('_');
				if(result.Length <= 3)continue;
				string[] idResult = result[2].Split('-');
				int id = (Convert.ToInt32(idResult[0]) << 4);
				if(idResult.Length == 2)
				{
					id += (Convert.ToInt32(idResult[1]) & 15);
				}

				int packTextureKey = GetPackTextureKey(id,picExtendKey);
				packTexture.AddTexture(item,packTextureKey);
				if(!_idToTextureTypeMap.ContainsKey(id))
					_idToTextureTypeMap.Add(id,type);
				for (int i = 3; i < result.Length; i++) {
					switch(result[i])
					{
					case "up":
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.up,packTextureKey,packTexture);
						break;
					case "down":
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.down,packTextureKey,packTexture);
						break;
					case "left":
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.left,packTextureKey,packTexture);
						break;
					case "right":
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.right,packTextureKey,packTexture);
						break;
					case "front":
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.front,packTextureKey,packTexture);
						break;
					case "back":
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.back,packTextureKey,packTexture);
						break;
					case "all":
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.up,packTextureKey,packTexture);
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.down,packTextureKey,packTexture);
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.left,packTextureKey,packTexture);
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.right,packTextureKey,packTexture);
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.front,packTextureKey,packTexture);
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.back,packTextureKey,packTexture);
						break;
					case "side":
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.left,packTextureKey,packTexture);
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.right,packTextureKey,packTexture);
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.front,packTextureKey,packTexture);
						SaveTexture(_idKeyList,_picKeyList,_listPackTexture,id,Direction.back,packTextureKey,packTexture);
						break;
					}
				}
				picExtendKey++;
			}
			packTexture.Pack();
//			packTexture.Texture.Compress(true);
			_packTextureMap.Add(type,packTexture);
		}

		void OnDestroy()
		{
			IsCanUseProvider = false;
		}

		public void SaveTexture(List<int> idKeyList,List<int> picKeyList,List<PackTexture> packTextureList,int id,Direction direction,int picKey,PackTexture packTexture)
		{
			int idKey = GetIdKey(id,direction);
			idKeyList.Add(idKey);
			picKeyList.Add(picKey);
			packTextureList.Add(packTexture);
		}

		private int GetIdKeyIndex(int idKey)
		{
			for (int i = 0; i < _idKeyList.Count; i++) {
				if(_idKeyList[i] == idKey)
				{
					return i;
				}
			}
			return -1;
		}

		public Rect GetBlockTextureRect(BlockType type,byte extendId,Direction direction)
		{
			int id = (((int)type) << 4) + (extendId & 15);
			int idKey = GetIdKey(id,direction);
			int index = GetIdKeyIndex(idKey);
			if(index == -1)throw new Exception("不存在类型为"+type + "拓展id为:" + extendId + "并且方向为" + direction + "的图集");
			int picKey = _picKeyList[index];
			PackTexture packTexture = _listPackTexture[index];
			return packTexture.GetSubTextureUV(picKey);
//			if(_worldTexMap.ContainsKey(id) && _worldTexMap[id].ContainsKey(direction))
//			{
//				string name = _worldTexMap[id][direction];
//				PackTexture packTexture = _packTextureMap[_idToTextureTypeMap[id]];
//				return packTexture.GetSubTextureUV(name);
//			}
//			throw new Exception("不存在类型为"+type + "拓展id为:" + extendId + "并且方向为" + direction);
		}

		public Texture2D GetAtlasTexture(WorldTextureType type)
		{
			PackTexture packTexture;
			if(_packTextureMap.TryGetValue(type,out packTexture))
			{
				return packTexture.Texture;
			}
			throw new Exception("不存在类型为" + type + "的纹理图集!");
		}

		public float GetTileWidth(WorldTextureType type)
		{
			PackTexture packTexture;
			if(_packTextureMap.TryGetValue(type,out packTexture))
			{
				return packTexture.GetTileWidth();
			}
			throw new Exception("不存在类型为" + type + "的纹理图集!");
		}

		public float GetTileHeight(WorldTextureType type)
		{
			PackTexture packTexture;
			if(_packTextureMap.TryGetValue(type,out packTexture))
			{
				return packTexture.GetTileHeight();
			}
			throw new Exception("不存在类型为" + type + "的纹理图集!");
		}

		public WorldTextureType GetTextureType(BlockType type,byte extendId)
		{
			int id = (((int)type) << 4) + (extendId & 15);
			WorldTextureType textureType;
			if(_idToTextureTypeMap.TryGetValue(id,out textureType))
			{
				return textureType;
			}
 			throw new Exception("不存在类型为"+type + "拓展id为:" + extendId + "的图集类型!");
		}
	}

	public enum WorldTextureType
	{
		OpaqueTileTex,
		OpaqueNoTileTex,
		TransparentTileTex,
		TransparentNoTileTex
	}

}

