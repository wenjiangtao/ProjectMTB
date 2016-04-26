using System;
namespace MTB
{
	public class ScriptFactory
	{
		public static BaseActionScript GetActionScript(string name,GameObjectController gameObjectController)
		{
			string className = "MTB." + name;
			Type t = Type.GetType(className);
			if(t == null)throw new Exception("不存在名字为:" + name + "的ActionScript");
			return Activator.CreateInstance(t,new object[]{gameObjectController}) as BaseActionScript;
		}

		public static BaseCancelConditionScript GetCancelScript(string name,GameObjectController gameObjectController)
		{
			string className = "MTB." + name;
			Type t = Type.GetType(className);
			if(t == null)throw new Exception("不存在名字为:" + name + "的CancelConditionScript");
			return Activator.CreateInstance(t,new object[]{gameObjectController}) as BaseCancelConditionScript;
		}

		public static BaseDoConditionScript GetDoScript(string name,GameObjectController gameObjectController)
		{
			string className = "MTB." + name;
			Type t = Type.GetType(className);
			if(t == null)throw new Exception("不存在名字为:" + name + "的DoConditionScript");
			return Activator.CreateInstance(t,new object[]{gameObjectController}) as BaseDoConditionScript;
		}

		public static BaseInputConditionScript GetInputConditionScript(string name,GameObjectController gameObjectController)
		{
			string className = "MTB." + name;
			Type t = Type.GetType(className);
			if(t == null)throw new Exception("不存在名字为:" + name + "的BaseInputConditionScript");
			return Activator.CreateInstance(t,new object[]{gameObjectController}) as BaseInputConditionScript;
		}
	}
}

