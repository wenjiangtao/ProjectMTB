using System;
using UnityEngine;
namespace MTB
{
    public class NPCInfo : GameObjectInfo
    {
        public int NPCId { get { return objectId; } set { objectId = value; } }
        public int taskId { get; set; }
        public int stepId { get; set; }

        public NPCType NPCType { get; set; }
        public NPCInfo()
        {
            NPCId = 1;
            taskId = 0;
            this.NPCType = NPCType.TASKNPC;
            groupId = 1;
            aoId = 0;
            position = Vector3.zero;
            isNetObj = false;
        }
    }
}

