using System;
using LibNoise;
namespace MTB
{
	public interface IMTBNoise
	{
		float GetValue(float x,float y);
		float GetValue(float x, float y, float z);

		//现在使用的是libnoise，如果以后修改，请删除该接口(请不要使用该方法)
		ModuleBase ModuleBase{get;}
	}
}

