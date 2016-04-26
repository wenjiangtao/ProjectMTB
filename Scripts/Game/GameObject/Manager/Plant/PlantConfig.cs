using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class PlantConfig
    {
        //load数据的时候动态加载  用于种植时候的判断
        public static Dictionary<int, DecorationType> SeedlingList = new Dictionary<int, DecorationType>(new SeedlingComparer());

        public class SeedlingComparer : IEqualityComparer<int>
        {
            #region IEqualityComparer implementation
            bool IEqualityComparer<int>.Equals(int a, int b)
            {
                return a == b;
            }
            int IEqualityComparer<int>.GetHashCode(int obj)
            {
                return (int)obj;
            }
            #endregion
        }
    }
}
