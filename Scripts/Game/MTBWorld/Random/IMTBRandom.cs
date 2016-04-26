using System;
namespace MTB
{
	public interface IMTBRandom
	{
		int Range(int min,int max);
		long seed{get;set;}
	}
}

