using System;
using UnityEngine;
namespace MTB
{
    public class NPCAttributes : BaseAttributes
    {
        private int _taskId;
        private int _stepId;
        private NPCType _npcType;

        public int taskId { get { return _taskId; } set { _taskId = value; } }

        public int stepId { get { return _stepId; } set { _stepId = value; } }

        public int NPCId { get { return objectId; } set { objectId = value; } }

        public NPCType NPCType { get { return _npcType; } set { _npcType = value; } }
    }

    public enum NPCType
    {
        TASKNPC,
        DAILYNPC
    }
}
