using System;
using System.Collections.Generic;
using System.Collections;	
namespace MTB
{
	public class ArraysCache
	{
		private static int[][] smallArrays = new int[128][];
		private int smallArraysNext = 0;
		private static List<int[]> bigArrays = new List<int[]>();
		private int bigArraysNext = 0;
		
		public bool isFree = true;
		
		
		public ArraysCache()
		{
			
		}
		
		public void release()
		{
			smallArraysNext = 0;
			bigArraysNext = 0;
			isFree = true;
		}
		
		public int[] getArray(int size)
		{
			
			if (size <= 256)
			{
				int[] array = smallArrays[smallArraysNext];
				if (array == null)
				{
					array = new int[256];
					smallArrays[smallArraysNext] = array;
				}
				smallArraysNext++;
				
				return array;
			}
			
			int[] bigArray;
			if (bigArraysNext == bigArrays.Count)
			{
				bigArray = new int[size];
				bigArrays.Add(bigArray);
			} 
			else
			{
				bigArray = bigArrays[bigArraysNext];
				if (bigArray.Length < size)
				{
					bigArray = new int[size];
					bigArrays[bigArraysNext] = bigArray;
				}
			}
			
			bigArraysNext++;
			return bigArray;
		}
	}
}

