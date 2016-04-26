using System;
using System.Collections.Generic;
namespace MTB
{
	public class BlockAttributeCalculatorFactory
	{
		private static BlockAttributeCalculator[] _arrMap = GetInitArr();

		private static BlockAttributeCalculator[] GetInitArr()
		{
			BlockAttributeCalculator[] arrMap = new BlockAttributeCalculator[256];
			foreach (var item in Enum.GetValues(typeof(BlockType))) {
				string className = "MTB.BAC_" + ((BlockType)item).ToString();
				Type t = Type.GetType(className);
				arrMap[(byte)item] = Activator.CreateInstance(t) as BlockAttributeCalculator;
			}
			return arrMap;
		}

		public static BlockAttributeCalculator GetCalculator(BlockType type)
		{
			return _arrMap[(byte)type];
		}
	}
}

