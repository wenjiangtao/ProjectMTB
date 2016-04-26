using System;
namespace MTB
{
    public interface ICave
    {
        CaveValue GetValue(float x,float y, float z,int heightLineHeight);
        float RateInMap { get; }
    }
}