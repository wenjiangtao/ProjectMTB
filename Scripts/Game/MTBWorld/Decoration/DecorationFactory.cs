using System;
namespace MTB
{
    public class DecorationFactory
    {
        private static IDecoration[] map = GetMap();

        private static IDecoration[] GetMap()
        {
            IDecoration[] arrMap = new IDecoration[256];
            foreach (var item in Enum.GetValues(typeof(DecorationType)))
            {
                string className = "MTB.Decoration_" + ((DecorationType)item).ToString();
                Type t = Type.GetType(className);
                arrMap[(byte)item] = Activator.CreateInstance(t) as IDecoration;
            }
            return arrMap;
        }
        public DecorationFactory()
        {
        }

        public static IDecoration GetDecoration(DecorationType type)
        {
            return map[(byte)type];
        }

        public static IDecoration GetDecorationInstance(DecorationType type)
        {
            string className = "MTB.Decoration_" + type.ToString();
            Type t = Type.GetType(className);
            return Activator.CreateInstance(t) as IDecoration; 
        }
    }
}

