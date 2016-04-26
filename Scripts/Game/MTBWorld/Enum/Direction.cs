using System;
namespace MTB
{
	//x轴向右，y轴向上，z轴向外(使用这样的坐标轴去看一个立方体）
	public enum Direction
	{
		front = 3,
		back = -3,
		left = -1,
		right = 1,
		up = 2,
		down = -2
	}
}

