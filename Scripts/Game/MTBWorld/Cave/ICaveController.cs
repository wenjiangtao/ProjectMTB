using System;
namespace MTB
{
    public interface ICaveController
    {
        //返回一个0-1的浮点数
        float GetValue(float x, float z);
        float Frequency { get; set; }
    }
}
