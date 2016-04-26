using UnityEngine;
using System.Collections;
namespace MTB
{
    public interface IDataManager
    {
        IData getData(int id, DataTypes type);
        IData getData(GameObject obj, DataTypes type);
        void loadData();
        void dispose();
    }
}
