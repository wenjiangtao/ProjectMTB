using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace MTB
{
    public class MonsterDataManager : IDataManager
    {
        private static string MIDDLE_PATH = "Data/Monster/";
        private static Dictionary<DataTypes, string> DATAPATH = new Dictionary<DataTypes, string>() { 
          {DataTypes.Basic,"Monster"},
          {DataTypes.AI,"MonsterAI"},
          {DataTypes.Action,"MonsterAction"},
          {DataTypes.Breeding,"MonsterAIBreed"}
        };

        public IData getData(int id, DataTypes type)
        {
            return MonsterDataFactory.getData(id, type);
        }

        public IData getData(GameObject obj, DataTypes type)
        {
            int id = obj.GetComponent<BaseAttributes>().objectId;
            MonsterBasicData basicData = getData(id, DataTypes.Basic) as MonsterBasicData;
            if (type == DataTypes.Basic)
                return basicData as IData;
            if (type == DataTypes.AI)
                id = basicData.aiId;
            else if (type == DataTypes.Action)
                id = basicData.actionId;
            else if (type == DataTypes.Breeding)
                id = basicData.breedId;
            return getData(id, type);
        }

        public MonsterAIData getAIData(GameObject obj)
        {
            return getData(obj, DataTypes.AI) as MonsterAIData;
        }

        public MonsterActionData getActionData(GameObject obj)
        {
            return getData(obj, DataTypes.Action) as MonsterActionData;
        }

        public MonsterBasicData getBasicData(GameObject obj)
        {
            return getData(obj, DataTypes.Basic) as MonsterBasicData;
        }

        public MonsterBreedData getBreedDate(GameObject obj)
        {
            return getData(obj, DataTypes.Breeding) as MonsterBreedData;
        }

        public void loadData()
        {
            string aipath = MIDDLE_PATH + DATAPATH[DataTypes.AI];
            string basicpath = MIDDLE_PATH + DATAPATH[DataTypes.Basic];
            string actionpath = MIDDLE_PATH + DATAPATH[DataTypes.Action];
            string breedpath = MIDDLE_PATH + DATAPATH[DataTypes.Breeding];
            XmlDocument basedata = new XmlDocument();
            XmlDocument aidata = new XmlDocument();
            XmlDocument actiondata = new XmlDocument();
            XmlDocument breeddata = new XmlDocument();
            basedata.LoadXml(Resources.Load(basicpath).ToString());
            aidata.LoadXml(Resources.Load(aipath).ToString());
            actiondata.LoadXml(Resources.Load(actionpath).ToString());
            breeddata.LoadXml(Resources.Load(breedpath).ToString());
            MonsterDataFactory.SaveBasicData(basedata);
            MonsterDataFactory.SaveAIData(aidata);
            MonsterDataFactory.SaveActionData(actiondata);
            MonsterDataFactory.SaveBreedData(breeddata);
        }

        public void dispose()
        {
            MonsterDataFactory.dispose();
        }
    }
}
