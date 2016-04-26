using System;
namespace MTB
{
	public enum ItemType
	{
		//物块类
		Block = 1,
		//技术块
		Technology = 2,
		//采集工具类(武器)
		Hand = 3,
		//消耗品
		Consumable = 4,
		//材料
		Mat = 5,
		//装备
		Equip = 6,
		//其他
		Other = 9
	}
	//物块类型
	public enum ItemBlockType
	{
		//地质块
		Geology = 1,
		//液体块
		Liquid = 2,
		//植物块
		Plant = 3
	}

	public enum ItemTechnologyType
	{
		//摆件
		Display = 1,
		//挂件
		Hang = 2
	}

	//采集工具类型
	public enum ItemHandType
	{
		//钻头
		Bore = 1,
		//金属
		Metal = 2,
		//复合物
		Complex = 3,
		//切割工具
		Incision = 4,
		//液体容器类
		Container = 5,
		//锤类武器
		Hammer = 6,
		//弓弩武器
		Bow = 7,
		//激光武器
		Laser = 8

	}
	//消耗品类型
	public enum ItemConsumableType
	{
		//食品
		Foods = 1,
		//燃料
		Burn = 2,
		//颜色
		Colors = 3
	}
	//材料类型
	public enum ItemMatType
	{
		//元素
		Element = 1,
		//晶体
		Crystal = 2,
		//聚合物
		Polymer = 3,
		//中间件
		Middleware = 4
	}
	//装备类型
	public enum ItemEquipType
	{
		ToalEquip = 1
	}

	public enum ItemOtherType
	{
		Other = 1
	}
}

