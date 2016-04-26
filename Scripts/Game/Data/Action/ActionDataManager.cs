using System;
using System.Xml;
using UnityEngine;
namespace MTB
{
    public class ActionDataManager
    {
        private static string PlayerActionPath = "Data/Action/Player_Action_Data_Config";
        private static string MonsterActionPath = "Data/Action/Monster_Action_Data_Config";
        private static string NpcActionPath = "Data/Action/NPC_Action_Data_Config";
        private ActionDataConfig _playerActionConfig;
        private ActionDataConfig _monsterActionConfig;
        private ActionDataConfig _npcActionConfig;

        private static ActionDataManager _instance;
        public static ActionDataManager Instance
        {
            get
            {
                if (_instance == null) _instance = new ActionDataManager();
                return _instance;
            }
        }

        public ActionDataManager()
        {
        }

        public void Init()
        {
            XmlDocument playerXml = new XmlDocument();
            playerXml.LoadXml(Resources.Load(PlayerActionPath).ToString());
            _playerActionConfig = new ActionDataConfig(playerXml.DocumentElement);

            XmlDocument monsterXml = new XmlDocument();
            monsterXml.LoadXml(Resources.Load(MonsterActionPath).ToString());
            _monsterActionConfig = new ActionDataConfig(monsterXml.DocumentElement);


            XmlDocument npcXml = new XmlDocument();
            npcXml.LoadXml(Resources.Load(NpcActionPath).ToString());
            _npcActionConfig = new ActionDataConfig(npcXml.DocumentElement);
        }

        public ActionDataConfig PlayerActionConfig { get { return _playerActionConfig; } }
        public ActionDataConfig MonsterActionConfig { get { return _monsterActionConfig; } }
        public ActionDataConfig NpcActionConfig { get { return _npcActionConfig; } }
    }
}

