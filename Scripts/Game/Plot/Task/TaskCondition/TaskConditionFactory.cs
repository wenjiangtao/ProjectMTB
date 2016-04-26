using System;
namespace MTB
{
    public class TaskConditionFactory
    {
        public static BaseTaskCondition GetStartTriggerCondition(string name)
        {
            string className = "MTB." + name;
            Type t = Type.GetType(className);
            if (t == null) throw new Exception("不存在名字为:" + name + "的StartTriggerCondition");
            return Activator.CreateInstance(t) as BaseTaskCondition;
        }

        public static BaseTaskCondition GetFinishTriggerCondition(string name)
        {
            string className = "MTB." + name;
            Type t = Type.GetType(className);
            if (t == null) throw new Exception("不存在名字为:" + name + "的FinishTriggerCondition");
            return Activator.CreateInstance(t) as BaseTaskCondition;
        }

        public static BaseTaskCondition GetFinishCondition(string name)
        {
            string className = "MTB." + name;
            Type t = Type.GetType(className);
            if (t == null) throw new Exception("不存在名字为:" + name + "的FinishCondition");
            return Activator.CreateInstance(t) as BaseTaskCondition;
        }

        public static BaseTaskCondition GetTipsCondition(string name)
        {
            string className = "MTB." + name;
            Type t = Type.GetType(className);
            if (t == null) throw new Exception("不存在名字为:" + name + "的TipsCondition");
            return Activator.CreateInstance(t) as BaseTaskCondition;
        }
    }
}
