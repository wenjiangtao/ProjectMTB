using System;
namespace MTB
{
	public class TerrainControlGenerator
	{

		private const int NOISE_MAX_X = Chunk.chunkWidth / 4 + 1;
		private const int NOISE_MAX_Z = Chunk.chunkDepth / 4 + 1;

		private const int OneOfTwoHeight =Chunk.chunkHeight / 2;

		private readonly Block air = new Block(BlockType.Air);
		private readonly Block water = new Block(BlockType.StillWater);

		private LayeredBiomeGenerator _biomeGenerator;

		private int maxSmoothDiameter;
		private int maxSmoothRadius;
	
		private float[] nearBiomeWeightArray;
		private int[] biomeArray;
		private float[] rawTerrain;
		private float[] terrain;

		private IMTBNoise _heightTerrainNoiseGen;
		private IMTBNoise _fractureNoiseGen1;
		private IMTBNoise _fractureNoiseGen2;
		private IMTBNoise _fractureNoiseSelect;
		private IMTBNoise _surfaceNoiseGen;

		private float heightFactor;
		private float volatilityFactor;

		private byte[] waterLevelRaw;
		private byte[] waterLevel;

		private int heightCap;

		public TerrainControlGenerator ()
		{
			_biomeGenerator = new LayeredBiomeGenerator();

			this.maxSmoothDiameter = WorldConfig.Instance.GetMaxSmoothRadius() * 2 + 1;
			this.maxSmoothRadius = WorldConfig.Instance.GetMaxSmoothRadius();

			this.nearBiomeWeightArray = new float[maxSmoothDiameter * maxSmoothDiameter];
			
			for (int x = -maxSmoothRadius; x <= maxSmoothRadius; x++)
			{
				for (int z = -maxSmoothRadius; z <= maxSmoothRadius; z++)
				{
					float f1 = (float)(10.0f / Math.Sqrt(x * x + z * z + 0.2f));
					this.nearBiomeWeightArray[(x + maxSmoothRadius) + maxSmoothDiameter * (z + maxSmoothRadius)] = f1;
				}
			}

			float xzScale = WorldConfig.Instance.fractureHorizontal;
			float yScale =  WorldConfig.Instance.fractureVertical;

			var perlinHeight = new MTBPerlin(WorldConfig.Instance.seed);
			perlinHeight.Frequency = 0.1f;
			perlinHeight.OctaveCount = 10;
			var heightScale = new MTBScale(perlinHeight,0.1f,0.1f);
			var heightScaleBias = new MTBScaleBias(heightScale,0,0.5f);
			_heightTerrainNoiseGen = heightScaleBias;

			var perlinFracture1 = new MTBPerlin(WorldConfig.Instance.seed + 100);
			perlinFracture1.OctaveCount = 10;
			perlinFracture1.Frequency = 0.02f;
			var fracture1Scale = new MTBScale(perlinFracture1,xzScale,yScale,xzScale);
			_fractureNoiseGen1 = fracture1Scale;

			var perlinFracture2 = new MTBPerlin(WorldConfig.Instance.seed + 200);
			perlinFracture2.OctaveCount = 10;
			perlinFracture2.Frequency = 0.03f;
			var fracture2Scale = new MTBScale(perlinFracture2,xzScale,yScale,xzScale);
			_fractureNoiseGen2 = fracture2Scale;

			var perlinFractureSelect = new MTBPerlin(WorldConfig.Instance.seed + 301);
			perlinFractureSelect.OctaveCount = 3;
			var fractureSelectScale = new MTBScale(perlinFractureSelect,xzScale,yScale,xzScale);
			_fractureNoiseSelect = fractureSelectScale;

			var perlinSurface = new MTBPerlin(WorldConfig.Instance.seed + 200);
			perlinSurface.OctaveCount = 6;
			var surfaceScale = new MTBScale(perlinSurface,0.02f,0.02f);
			var surfaceScaleBias = new MTBScaleBias(surfaceScale,2,4);
			_surfaceNoiseGen = surfaceScaleBias;

			waterLevelRaw = new byte[NOISE_MAX_X * NOISE_MAX_Z];
			waterLevel = new byte[Chunk.chunkWidth * Chunk.chunkDepth];

			heightCap = WorldConfig.Instance.heightCap;
			terrain = new float[Chunk.chunkWidth * heightCap * Chunk.chunkDepth];
		}

		public void Generate(Chunk chunk)
		{
			GenerateBiomeAndTerrain(chunk);

			AddBiomeBlocks(chunk);

			for (int x = 0; x < Chunk.chunkWidth; x++) {
				for (int z = 0; z < Chunk.chunkDepth; z++) {
					chunk.SetBiomeId(x,z,this.biomeArray[x + Chunk.chunkDepth * z],true);
				}
			}
		}

		private void GenerateBiomeAndTerrain(Chunk chunk)
		{
			int chunkX = chunk.worldPos.x;
			int chunkZ = chunk.worldPos.z;

			int maxYSections =heightCap / 8 + 1;
			int oneEightOfHeight = heightCap / 8;

			this.biomeArray = _biomeGenerator.GetBiomesUnZoomed(this.biomeArray,chunkX / 4 - this.maxSmoothRadius,chunkZ / 4 - this.maxSmoothRadius,
			                                  NOISE_MAX_X + this.maxSmoothDiameter,NOISE_MAX_Z + this.maxSmoothDiameter);

			GenerateTerrainNoise(chunkX / 4,0,chunkZ / 4,maxYSections);


			this.biomeArray = _biomeGenerator.GetBiomes(this.biomeArray,chunkX,chunkZ,Chunk.chunkWidth,Chunk.chunkDepth);

			float oneOfEight = 0.125f;
			float oneOfFourth = 0.25f;
			for (int x = 0; x < NOISE_MAX_X - 1; x++) {
				for (int z = 0; z < NOISE_MAX_Z - 1; z++) {
					float waterLevel_x0z0 = this.waterLevelRaw[x * NOISE_MAX_Z +z] & 0xff;
					float waterLevel_x0z1 = this.waterLevelRaw[x * NOISE_MAX_Z + (z + 1)] & 0xff;
					float waterLevel_x1z0 = ((this.waterLevelRaw[(x + 1)* NOISE_MAX_Z + z] & 0xff) - waterLevel_x0z0) * oneOfFourth;
					float waterLevel_x1z1 = ((this.waterLevelRaw[(x + 1)* NOISE_MAX_Z + (z + 1)] & 0xff) - waterLevel_x0z1) * oneOfFourth;
					for (int x_piece = 0; x_piece < 4; x_piece++) {
						float waterLevelForArray = waterLevel_x0z0;
						float subZ1_0 = (waterLevel_x0z1 - waterLevel_x0z0) * oneOfFourth;
						for (int z_piece = 0; z_piece < 4; z_piece++) {
							this.waterLevel[(x * 4 + x_piece) * Chunk.chunkDepth + (z * 4 + z_piece)] = (byte)waterLevelForArray;
							waterLevelForArray += subZ1_0;
						}
						waterLevel_x0z0 += waterLevel_x1z0;
						waterLevel_x0z1 += waterLevel_x1z1;
					}

					for (int y = 0; y < oneEightOfHeight; y++) {
						float x0z0 = this.rawTerrain[(x * NOISE_MAX_Z + z) * maxYSections + y];
						float x0z1 = this.rawTerrain[(x * NOISE_MAX_Z + (z + 1)) * maxYSections + y];
						float x1z0 = this.rawTerrain[((x + 1) * NOISE_MAX_Z + z) * maxYSections + y];
						float x1z1 = this.rawTerrain[((x + 1) * NOISE_MAX_Z + (z + 1)) * maxYSections + y];

						float x0z0y1 = (this.rawTerrain[(x * NOISE_MAX_Z + z) * maxYSections + (y + 1)] - x0z0) * oneOfEight;
						float x0z1y1 = (this.rawTerrain[(x * NOISE_MAX_Z + (z + 1)) * maxYSections + (y + 1)] - x0z1) * oneOfEight;
						float x1z0y1 = (this.rawTerrain[((x + 1) * NOISE_MAX_Z + z) * maxYSections + (y + 1)] - x1z0) * oneOfEight;
						float x1z1y1 = (this.rawTerrain[((x + 1) * NOISE_MAX_Z + (z + 1)) * maxYSections + (y + 1)] - x1z1) * oneOfEight;

						for (int y_piece = 0; y_piece < 8; y_piece++) {
							float d11 = x0z0;
							float d12 = x0z1;
							float d13 = (x1z0 - x0z0) * oneOfFourth;
							float d14 = (x1z1 - x0z1) * oneOfFourth;
							for (int x_piece = 0; x_piece < 4; x_piece++) {
								float result = d11;
								float subZ1_0 = (d12 - d11) * oneOfFourth;
								for (int z_piece = 0; z_piece < 4; z_piece++) {
									int tx = x * 4 + x_piece;
									int ty = y * 8 + y_piece;
									int tz = z * 4 + z_piece;
									terrain[(tx * Chunk.chunkDepth + tz) * heightCap + ty] = result;
//									terrain[x * 4 + x_piece,y * 8 + y_piece,z * 4 + z_piece] = result;
									result += subZ1_0;
								}
								d11 += d13;
								d12 += d14;
							}
							x0z0 += x0z0y1;
							x0z1 += x0z1y1;
							x1z0 += x1z0y1;
							x1z1 += x1z1y1;
						}
					}
				}
			}


			for (int x = 0; x < Chunk.chunkWidth; x++) {
				for (int z = 0; z < Chunk.chunkDepth; z++) {
					BiomeConfig biomeConfig = WorldConfig.Instance.GetBiomeConfigById(this.biomeArray[x + Chunk.chunkDepth * z]);
					int curWaterLevel = waterLevel[x * Chunk.chunkDepth + z] & 0xff;
					for (int y = Chunk.chunkHeight - 1; y >= 0 ; y--) {
						Block b = air;
						if(y < heightCap)
						{
							if(y < curWaterLevel && y > biomeConfig.waterLevelMin)
							{
								b = water;
							}

							if(terrain[(x * Chunk.chunkDepth + z) * heightCap + y] > 0f)
							{
								b = biomeConfig.stoneBlock;
							}

						}
						chunk.SetBlock(x,y,z,b,true);
					}
				}
			}


		}

		private void GenerateTerrainNoise(int xOffset,int yOffset,int zOffset,int maxYSections)
		{
			if(this.rawTerrain == null || (this.rawTerrain.Length != NOISE_MAX_X * maxYSections * NOISE_MAX_Z))
			{
				this.rawTerrain = new float[NOISE_MAX_X * maxYSections * NOISE_MAX_Z];
			}

			for (int x = 0; x < NOISE_MAX_X; x++) {
				for (int z = 0; z < NOISE_MAX_Z; z++) {
					int biomeId = this.biomeArray[(x + this.maxSmoothRadius) + (NOISE_MAX_Z + this.maxSmoothDiameter) * (z + this.maxSmoothRadius)];
					BiomeConfig biomeConfig = WorldConfig.Instance.GetBiomeConfigById(biomeId);
					//输出值为-0.5~0.5f
					float noiseHeight = _heightTerrainNoiseGen.GetValue(xOffset + x,zOffset + z);

					//将noiseHeight转换到-1~1f
					if(noiseHeight < 0f)
					{
						if(noiseHeight < -1f)
						{
							noiseHeight = -1f;
						}
						noiseHeight -= biomeConfig.maxAverageDepth;
					}else
					{
						if(noiseHeight > 1f)
						{
							noiseHeight = 1f;
						}
						noiseHeight += biomeConfig.maxAverageHeight;
					}
					this.waterLevelRaw[x * NOISE_MAX_Z + z] = (byte)biomeConfig.waterLevelMax;
					CalculateBiomeFactor(x,z,maxYSections - 1,noiseHeight,biomeConfig);

					for (int y = 0; y < maxYSections; y++) {
						float output = 0;

						float selectNoise = _fractureNoiseSelect.GetValue((xOffset + x),(yOffset + y),(zOffset + z));
						selectNoise = (selectNoise + 1f) / 2f;

						if(selectNoise < biomeConfig.volatilityWeight1)
						{
							output = _fractureNoiseGen1.GetValue(xOffset + x,yOffset + y,zOffset + z) * biomeConfig.volatility1;
						}
						else if(selectNoise > biomeConfig.volatilityWeight2)
						{
							output = _fractureNoiseGen2.GetValue(xOffset + x,yOffset + y,zOffset + z) * biomeConfig.volatility2;
						}
						else
						{
							float fracture1 = _fractureNoiseGen1.GetValue(xOffset + x,yOffset + y,zOffset + z) * biomeConfig.volatility1;
							float fracture2 = _fractureNoiseGen2.GetValue(xOffset + x,yOffset + y,zOffset + z) * biomeConfig.volatility2;
							output = fracture1 + (fracture2 - fracture1) * selectNoise;
						}

						if(!biomeConfig.disableNotchHeightControl)
						{
							float output_notchHeight;
							//越到地平线形变越严重
							output_notchHeight =((this.heightFactor - y) / this.volatilityFactor);

							if(output_notchHeight > 0)output_notchHeight += output_notchHeight * 10f + 1;
							output += output_notchHeight;

							if (y > maxYSections - 4)
							{
								float subMaxLastThreeLayerY = (y - (maxYSections - 4)) / 3.0f;
								output = output * (1.0f - subMaxLastThreeLayerY) - subMaxLastThreeLayerY;
							}
						}

						output += biomeConfig.heightMatrix[y];
						this.rawTerrain[(x * NOISE_MAX_Z + z) * maxYSections + y] = output;
					}
				}
			}
		}

		private void CalculateBiomeFactor(int x,int z,int ySections,float noiseHeight,BiomeConfig centerBiomeConfig)
		{
			float volatilitySum = 0f;
			float heightSum = 0f;
			float biomeWeightSum = 0f;

			float nextBiomeHeight,nextBiomeWeight;
			int lookRadius = centerBiomeConfig.smoothRadius;

			for (int nextX = -lookRadius; nextX <= lookRadius; nextX++) {
				for (int nextZ = -lookRadius; nextZ <= lookRadius; nextZ++) {
					int biomeId = this.biomeArray[(x + nextX + this.maxSmoothRadius) + (NOISE_MAX_Z + this.maxSmoothDiameter) * (z + nextZ + this.maxSmoothRadius)];
					BiomeConfig nextBiomeConfig = WorldConfig.Instance.GetBiomeConfigById(biomeId);
					nextBiomeHeight = nextBiomeConfig.biomeHeight;
					//当前高度越高，权重会越小(即表示挥发性和高度都会越小)
					nextBiomeWeight = this.nearBiomeWeightArray[(nextX + this.maxSmoothRadius) + maxSmoothDiameter * (nextZ + this.maxSmoothRadius)] / (Math.Abs(nextBiomeHeight) + 1f);
					if(nextBiomeHeight > centerBiomeConfig.biomeHeight)nextBiomeWeight /= 2.0f;
					volatilitySum += nextBiomeConfig.biomeVolatility * nextBiomeWeight;
					heightSum += nextBiomeConfig.biomeHeight * nextBiomeWeight;
					biomeWeightSum += nextBiomeWeight;
				}
			}
			volatilitySum /= biomeWeightSum;
			heightSum /= biomeWeightSum;

			//防止最终值等于0
			volatilitySum = volatilitySum * 0.9f + 0.1f;
			//将-10~10f转换为-1f~1f
			heightSum /= 10f;

			this.volatilityFactor = volatilitySum * 4f;

			float rate = (float)WorldConfig.Instance.baseTerrainHeight / OneOfTwoHeight;
			heightSum += noiseHeight * 0.2f;
			if(heightSum < 0f)
			{
				heightSum *= rate;
			}
			heightSum = (heightSum + 1.2f) / 2.4f;

			float offset = (WorldConfig.Instance.baseTerrainHeight - OneOfTwoHeight) / (float)Chunk.chunkHeight;
			this.heightFactor = ySections *(heightSum + offset);
		}

		private void AddBiomeBlocks(Chunk chunk)
		{
			int dryBlocksOnSurface = 256;
			int chunkX = chunk.worldPos.x;
			int chunkZ = chunk.worldPos.z;
			for (int x = 0; x < Chunk.chunkWidth; x++) {
				for (int z = 0; z < Chunk.chunkDepth; z++) {
					BiomeConfig biomeConfig = WorldConfig.Instance.GetBiomeConfigById(this.biomeArray[x + Chunk.chunkDepth * z]);
//					int surfaceBlocksNoise = (int)_surfaceNoiseGen.GetValue(chunkX + x,chunkZ + z);
					int surfaceBlocksNoise = 1;
					Block currentSurfaceBlock = biomeConfig.surfaceBlock;
					Block currentGroundBlock = biomeConfig.groundBlock;
					int surfaceBlocksCount = int.MinValue;
					int currentWaterLevel = this.waterLevel[x * Chunk.chunkDepth + z];
					for (int y = heightCap - 1; y >= 0; y--) {
						Block curBlock = chunk.GetBlock(x,y,z,true);
						if(curBlock.BlockType == BlockType.Air)
						{
							surfaceBlocksCount = int.MinValue;
						}
						else if(curBlock.EqualOther(biomeConfig.stoneBlock))
						{
							if(surfaceBlocksCount == int.MinValue)
							{
								if(surfaceBlocksNoise <= 0 && biomeConfig.removeSurfaceStone)
								{
									currentSurfaceBlock = air;
									currentGroundBlock = biomeConfig.groundBlock;
								}
								/*else if((y > currentWaterLevel - 3) && (y < currentWaterLevel + 2))
								{
									currentSurfaceBlock = biomeConfig.surfaceBlock;
									currentGroundBlock = biomeConfig.groundBlock;
								}
								*/
								if ((y < currentWaterLevel) && (y > biomeConfig.waterLevelMin) && currentSurfaceBlock.BlockType == BlockType.Air)
								{
									currentSurfaceBlock = biomeConfig.waterBlock;
								}
								surfaceBlocksCount = surfaceBlocksNoise;
								if(surfaceBlocksNoise <= 0)surfaceBlocksCount += 3;
								if(y > currentWaterLevel - 2)
								{
									chunk.SetBlock(x,y,z,currentSurfaceBlock,true);
								}
								else
								{
									chunk.SetBlock(x,y,z,currentGroundBlock,true);
								}
							}
							else if(surfaceBlocksCount > 0)
							{
								surfaceBlocksCount--;
								chunk.SetBlock(x,y,z,currentGroundBlock,true);
							}
						}
					}
				}
			}
		}
	}
}

