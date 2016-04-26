using System;
using UnityEngine;
namespace MTB
{
    public class GameConfig : Singleton<GameConfig>
    {
        public string defaultWorldConfigStr = "WorldConfig";
        public string defaultWorldName = "default";
        public int defalutSeed;
        public string WorldSavedPath { get; private set; }
        public string NewBieWorldDataPath { get; private set; }
        void Awake()
        {
            Instance = this;
            WorldSavedPath = Application.persistentDataPath + "/MTBWorld";
            NewBieWorldDataPath = Application.dataPath + "/Resources/Data/Plot/SceneData/Newbie";
        }
    }
}

