using System;
using UnityEngine;
namespace MTB
{
	public class BiomeMapAjustTest : MonoBehaviour
	{
		public string worldConfigName = "WorldConfig";
		public int picWidth = 512;
		public int picHeight = 512;
		private MeshRenderer _render;

		private Layer mainLayer;
		protected const int LandBit = (1 << 10);  
		protected const int IceBit = (1 << 12);
		protected const int BiomeBits = 1023;
		
		protected const int BiomeGroupShift = 13;        //>>	Shift amount for biome group data
		protected const int BiomeGroupBits = (127 << BiomeGroupShift); 
		
		void Start () {
			GameObject.Instantiate(Resources.Load("Prefabs/" + worldConfigName) as GameObject);
			_render = GetComponent<MeshRenderer>();
			Render();

		}

		public void OnGUI()
		{
			if(GUI.Button(new Rect(0,0,100,50),"重新渲染"))
			{
				Render();
			}
		}

		public void Render()
		{
			mainLayer = LayerFactory.GetBaseLayer();
			mainLayer = LayerFactory.GetVoronoiLayer(mainLayer);
			mainLayer.initWorldGenSeed(WorldConfig.Instance.seed);
			_render.material.mainTexture = GetTexture();
		}
		

		public Texture2D GetTexture()
		{
			int xSize = 16;
			int zSize = 16;
			int chunkWidth =picWidth / 16;
			int chunkDepth = picHeight / 16;
			Texture2D tex = new Texture2D(chunkWidth * xSize,chunkDepth * zSize);
			ArraysCache cache = ArraysCacheManager.GetCache();
			for (int x = 0; x < chunkWidth; x++) {
				for (int z = 0; z < chunkDepth; z++) {
					int[] result = mainLayer.getInts(cache,x * xSize,z * zSize,xSize,zSize);
					cache.release();
					for (int i = 0; i < result.Length; i++) {
						int tempX = i % xSize;
						int tempZ = i / xSize;
						Color c;
						int biomeId = ((result[i] & BiomeBits));

							BiomeConfig biomeConfig = WorldConfig.Instance.GetBiomeConfigById(biomeId);
							c = biomeConfig.color;
						tex.SetPixel(x * xSize + tempX,z * zSize + tempZ,c);
					}
				}
			}
			tex.Apply();
			return tex;
		}
	}
}

