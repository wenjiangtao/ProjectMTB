
using System;
namespace MTB
{
    public enum CaveType : byte
    {
        //横向洞穴
        Horizontal = 1,
        //纵向洞穴
        Vertical = 2,
        //空间洞穴
        Space = 3,
        //峡谷
        Canyon = 4,
        //竖井
        Shaft = 5,
        //巨型洞口
        Hole = 6
    }
}
