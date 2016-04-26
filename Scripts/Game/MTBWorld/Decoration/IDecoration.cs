using System;
namespace MTB
{
	public interface IDecoration
	{
		bool Decorade(Chunk chunk,int x,int y,int z,IMTBRandom random);
	}
}

