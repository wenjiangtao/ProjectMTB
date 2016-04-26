using System;
namespace MTB
{
	public class LayerFactory
	{
		public LayerFactory ()
		{
		}

		public static Layer GetBaseLayer()
		{
			Layer mainLayer = new LayerEmpty(2);
			for (int depth = 0; depth <= WorldConfig.Instance.GenerateDepth; depth++) {
				mainLayer = new LayerZoom(2001+depth,mainLayer);
				if(depth == WorldConfig.Instance.LandSize)
				{
					mainLayer = new LayerLand(1,mainLayer,WorldConfig.Instance.LandRarity);
					mainLayer = new LayerZoomFuzzy(1000,mainLayer);
				}

				//使陆地失真（即有可能产生的陆地，也有可能将陆地变为水）
				if( depth >= WorldConfig.Instance.LandSize &&
				   depth < WorldConfig.Instance.LandSize + WorldConfig.Instance.LandFuzzy)
				{
					mainLayer = new LayerLandRandom(depth,mainLayer);
				}

				if(WorldConfig.Instance.HasGroupConfigInDepth(depth))
				{
					mainLayer = new LayerBiomeGroups(100,mainLayer,depth);
				}

				if(WorldConfig.Instance.HasBiomeConfigInDepth(depth))
				{
					mainLayer = new LayerBiome(depth,mainLayer,depth);
				}

//				if(WorldConfig.Instance.riverRarity == depth)
//				{
//					mainLayer = new LayerRiverInit(132,mainLayer);
//				}
//
//				if(WorldConfig.Instance.riverSize == depth)
//				{
//					mainLayer = new LayerRiver(5 + depth,mainLayer);
//				}

				LayerBiomeBorder layerBiomeBorder = new LayerBiomeBorder(200 + depth);
				bool hasBorder = false;
				for (int i = 0; i < WorldConfig.Instance.biomeConfigs.Count; i++) {
					BiomeConfig biomeConfig = WorldConfig.Instance.biomeConfigs[i];

					if(biomeConfig.biomeSizeWhenIsle == depth &&
					   WorldConfig.Instance.isleBiomeIds.Contains(biomeConfig.biomeId) &&
					   biomeConfig.IsleInBiome.Count > 0)
					{
						LayerBiomeInBiome layerBiomeInBiome = new LayerBiomeInBiome(400 + biomeConfig.biomeId,mainLayer);
						layerBiomeInBiome.biomeConfig = biomeConfig;
						for (int j = 0; j < biomeConfig.IsleInBiome.Count; j++) {
							int isleBiomeId = biomeConfig.IsleInBiome[j];
							if(isleBiomeId == DefaultBiome.Ocean.id)
							{
								layerBiomeInBiome.inOcean = true;
							}
							else
							{
								layerBiomeInBiome.biomeIsles[isleBiomeId] = true;
							}
						}
						layerBiomeInBiome.chance = biomeConfig.biomeRarityWhenIsle;
						mainLayer = layerBiomeInBiome;
					}

					if(biomeConfig.biomeSizeWhenBorder == depth &&
					   WorldConfig.Instance.borderBiomeIds.Contains(biomeConfig.biomeId) &&
					   biomeConfig.biomeIsBorder.Count > 0)
					{
						hasBorder = true;
						for (int j = 0; j < biomeConfig.biomeIsBorder.Count; j++) {
							int replaceBiomeId = biomeConfig.biomeIsBorder[j];
							layerBiomeBorder.addBiome(biomeConfig,replaceBiomeId);
						}
					}
				}
				if(hasBorder)
				{
					layerBiomeBorder.child = mainLayer;
					mainLayer = layerBiomeBorder;
				}
			}
			mainLayer = new LayerOutputDefault(1000,mainLayer);
			mainLayer = new LayerSmooth(400,mainLayer);
			return mainLayer;
		}

		public static Layer GetVoronoiLayer(Layer baseLayer)
		{
			Layer voronoiLayer = new LayerZoomVoronoi(10,baseLayer);
			voronoiLayer.initWorldGenSeed(WorldConfig.Instance.seed);
			return voronoiLayer;
		}

	}
}

