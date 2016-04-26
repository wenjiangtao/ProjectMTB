using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace MTB
{
    public class PackTexture
    {

//        string name;
		WorldTextureType textureType;
        public Rect[] uvs;
        Texture2D atlas;
        List<Texture2D> subTextureList;
//        Dictionary<string, int> subTextureIdxMap;
		List<int> keyList;
		bool useOptimize = false;
		private int maxAtlasSize;
		public PackTexture(WorldTextureType textureType, int width = 1024, int height = 1024,TextureFormat format = TextureFormat.DXT5,bool useOptimize = false)
        {
//            this.name = name;
			this.textureType = textureType;
			this.useOptimize = useOptimize;
            subTextureList = new List<Texture2D>();
//            subTextureIdxMap = new Dictionary<string, int>();
			keyList = new List<int>();
			atlas = new Texture2D(width, height,format,true);
			atlas.anisoLevel = 16;
			atlas.wrapMode = TextureWrapMode.Clamp;
			maxAtlasSize = width > height ? width : height;
        }

		private Texture2D GetOptimizeTex(Texture2D tex)
		{
			int width = tex.width * 2;
			int height = tex.height * 2;
			int selfWidth = tex.width / 2;
			int selfHeight = tex.height / 2;
			Texture2D result = new Texture2D(width,height,tex.format,false);
			result.anisoLevel = tex.anisoLevel;
			result.filterMode = tex.filterMode;
			result.wrapMode = tex.wrapMode;
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					result.SetPixel(i,j,tex.GetPixel((i + selfWidth) % tex.width,(j + selfHeight) % tex.height));
				}
			}
			result.Apply(false);
			return result;
		}

        public void AddTexture(Texture2D tex, int key)
        {
//			if(useOptimize)
//			{
//				tex = GetOptimizeTex(tex);
//			}
			if(KeyIndexOf(key) != -1){
				return;
			}
				
            subTextureList.Add(tex);
			keyList.Add(key);
        }

		public float GetTileWidth()
		{
			if(uvs.Length <= 0)return atlas.width;
			return uvs[0].width;
		}

		public float GetTileHeight()
		{
			if(uvs.Length <= 0)return atlas.height;
			return uvs[0].height;
		}

        public void Pack()
        {
			this.uvs = this.atlas.PackTextures(subTextureList.ToArray(),0, maxAtlasSize, true);
			if(useOptimize)
			{
				for (int i = 0; i < uvs.Length; i++) {
					float width = uvs[i].width / 2f;
					float height = uvs[i].height / 2f;
					uvs[i] = new Rect(uvs[i].x + width / 2f,uvs[i].y + height/2f,width,height);
				}
			}
			subTextureList.Clear();
			subTextureList = null;
        }

        public Rect GetSubTextureUV(int key)
        {
			int index = KeyIndexOf(key);
			if(index != -1)
			{
				return uvs[index];
			}
			throw new Exception("未命名为" + key +"的贴图资源");
        }

//		public Rect GetSubTextureUV(int index)
//		{
//			return uvs[index];
//		}

		public int KeyIndexOf(int key)
		{
			for (int i = 0; i < keyList.Count; i++) {
				if(keyList[i] == key)
				{
					return i;
				}
			}
			return -1;
		}

        public Texture2D Texture
        {
            get{
                return this.atlas;
            }
           
        }

		public WorldTextureType TextureType
		{
			get{
				return textureType;
			}
		}
       
    }
}