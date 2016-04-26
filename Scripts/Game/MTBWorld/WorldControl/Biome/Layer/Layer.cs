using System;
namespace MTB
{
	public abstract class Layer
	{
		protected long baseSeed;
		
		protected long worldGenSeed;
		
		private long chunkSeed;
		
		private long groupSeed;
		
		public Layer child;
	
		protected const int entropy = 10000;
		
		// [ Biome Data ]
		protected const int BiomeBits = 1023;            //>>	1st-10th Bits           // 255 63
		
		// [ Flags ]
		protected const int LandBit = (1 << 10);         //>>	11th Bit, 1024          // 256 64
		protected const int IslandBit = (1 << 11);       //>>	12th Bit, 2048          // 4096 1024
		protected const int IceBit = (1 << 12);          //>>	13th Bit, 4096
		
		// [ Biome Group Data ]
		protected const int BiomeGroupShift = 13;        //>>	Shift amount for biome group data
		protected const int BiomeGroupBits = (127 << BiomeGroupShift);   //>>	14th-20th Bits, 1040384
		
		// [ River Data ]
		protected const int RiverShift = 20;
		protected const int RiverBits = (3 << RiverShift);               //>>	21st-22nd Bits, 3145728  //3072 768
		protected const int RiverBitOne = (1 << RiverShift);             //>>	21st Bit, 1048576
		protected const int RiverBitTwo = (1 << (RiverShift + 1));       //>>	22nd Bit, 2097152
		
	
		protected static int GetBiomeFromLayer(int selection)
		{
			return (selection & LandBit) != 0 ? (selection & BiomeBits) : 0;
		}
		
		protected Layer(long seed)
		{
			this.baseSeed = seed;
			this.baseSeed *= (this.baseSeed * 6364136223846793005L + 1442695040888963407L);
			this.baseSeed += seed;
			this.baseSeed *= (this.baseSeed * 6364136223846793005L + 1442695040888963407L);
			this.baseSeed += seed;
			this.baseSeed *= (this.baseSeed * 6364136223846793005L + 1442695040888963407L);
			this.baseSeed += seed;
		}
		
		public Layer()
		{
		}
		
		public void initWorldGenSeed(long seed)
		{
			this.worldGenSeed = seed;
			if (this.child != null)
				this.child.initWorldGenSeed(seed);
			this.worldGenSeed *= (this.worldGenSeed * 6364136223846793005L + 1442695040888963407L);
			this.worldGenSeed += this.baseSeed;
			this.worldGenSeed *= (this.worldGenSeed * 6364136223846793005L + 1442695040888963407L);
			this.worldGenSeed += this.baseSeed;
			this.worldGenSeed *= (this.worldGenSeed * 6364136223846793005L + 1442695040888963407L);
			this.worldGenSeed += this.baseSeed;
		}
		
		protected void initChunkSeed(long x, long z)
		{
			this.chunkSeed = this.worldGenSeed;
			this.chunkSeed *= (this.chunkSeed * 6364136223846793005L + 1442695040888963407L);
			this.chunkSeed += x;
			this.chunkSeed *= (this.chunkSeed * 6364136223846793005L + 1442695040888963407L);
			this.chunkSeed += z;
			this.chunkSeed *= (this.chunkSeed * 6364136223846793005L + 1442695040888963407L);
			this.chunkSeed += x;
			this.chunkSeed *= (this.chunkSeed * 6364136223846793005L + 1442695040888963407L);
			this.chunkSeed += z;
		}
		
		protected void initGroupSeed(long x, long z)
		{
			this.groupSeed = this.chunkSeed;
			this.groupSeed *= (this.groupSeed * 6364136223846793005L + 1442695040888963407L);
			this.groupSeed += x;
			this.groupSeed *= (this.groupSeed * 6364136223846793005L + 1442695040888963407L);
			this.groupSeed += z;
			this.groupSeed *= (this.groupSeed * 6364136223846793005L + 1442695040888963407L);
			this.groupSeed += x;
			this.groupSeed *= (this.groupSeed * 6364136223846793005L + 1442695040888963407L);
			this.groupSeed += z;
		}
		
		protected int nextInt(int x)
		{
			int i = (int) ((this.chunkSeed >> 24) % x);
			if (i < 0)
				i += x;
			this.chunkSeed *= (this.chunkSeed * 6364136223846793005L + 1442695040888963407L);
			this.chunkSeed += this.worldGenSeed;
			return i;
		}
		
		protected int nextGroupInt(int x)
		{
			int i = (int) ((this.groupSeed >> 24) % x);
			if (i < 0)
				i += x;
			this.groupSeed *= (this.groupSeed * 6364136223846793005L + 1442695040888963407L);
			this.groupSeed += this.chunkSeed;
			return i;
		}
		
		public abstract int[] getInts(ArraysCache cache, int x, int z, int xSize, int zSize);
		
		protected int getRandomInArray(int[] rand)
		{
			return rand[this.nextInt(rand.Length)];
		}

		protected int[] randomArr = new int[4];
		protected virtual int getRandomOf4(int a, int b, int c, int d)
		{
			return b == c && c == d
				? b
					: (a == b && a == c
					   ? a
					   : (a == b && a == d
					   ? a
					   : (a == c && a == d
					   ? a
					   : (a == b && c != d
					   ? a
					   : (a == c && b != d
					   ? a
					   : (a == d && b != c
					   ? a
					   : (b == c && a != d
					   ? b
					   : (b == d && a != c
					   ? b
					   : (c == d && a != b
					   ? c
					   : this.getRandomInArray(
						a, b, c, d)
					)))))))));
		}

		protected int getRandomInArray(int a,int b,int c,int d)
		{
			randomArr[0] = a;
			randomArr[1] = b;
			randomArr[2] = c;
			randomArr[3] = d;
			return getRandomInArray(randomArr);
		}
	}

}