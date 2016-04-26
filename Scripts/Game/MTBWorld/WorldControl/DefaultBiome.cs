using System;
namespace MTB
{
	public class DefaultBiome
	{
		public static DefaultBiomeInfo Ocean = new DefaultBiomeInfo(0,"Ocean");
	}

	public class DefaultBiomeInfo
	{
		public int id;
		public string name;

		public DefaultBiomeInfo(int id,string name)
		{
			this.id = id;
			this.name = name;
		}
	}
}

