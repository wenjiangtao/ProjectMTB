using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class NPCManager : BaseHasActionObjectManager
    {
        private string RESPATH = "Prefabs/Npc/";

        public NPCManager(GameObject parent)
            : base(parent)
        {
            EventManager.RegisterEvent(EventMacro.GENERATE_FIRST_WORLD_FINISH, onFirstWorldFinish);
        }

        //根据taskId，stepId刷npc
        public GameObject InitNPC(int taskId, int stepId)
        {
            MTBTaskData data = MTBTaskDataManager.Instance.getData(taskId);
            NPCInfo info = new NPCInfo();
            info.position = data.stepList[stepId].npcPosData.pos;
            info.NPCId = data.stepList[stepId].npcPosData.npcId;
            info.taskId = taskId;
            info.stepId = stepId;
            info.aoId = AoIdManager.instance.getAoId();
            return InitNPC(info);
        }

        //把npc加载到场景中
        public GameObject InitNPC(NPCInfo info, bool checkTask = true)
        {
            MTBNPCData npcdata = MTBNPCDataManager.Instance.getData(info.NPCId) as MTBNPCData;
            if (npcdata == null)
            {
                throw new Exception("当前npcId有误或者没有配置对应资源");
            }
            if (!checkRefreshNpc(info, checkTask))
                return null;
            string resPath = RESPATH + npcdata.resName;
            GameObject prefab = Resources.Load(resPath) as GameObject;
            GameObject npc = GameObject.Instantiate(prefab);
            npc.name = "NPC_" + info.NPCId;
            npc.transform.position = info.position;
            if (npc.GetComponent<AutoMoveController>() == null)
                npc.AddComponent<AutoMoveController>();
            addObj(npc, info);
            EventManager.SendEvent(EventMacro.NPC_INITED, info);
            return npc;
        }

        private bool checkRefreshNpc(NPCInfo info, bool checkTask = true)
        {
            if (checkTask)
            {
                if (getObjByNpcId(info.NPCId) != null)
                {
                    if (info.taskId != 0 && MTBTaskController.Instance.taskCacherData.canConductTaskList.ContainsKey(info.taskId))
                    {
                        GameObject oldNpc = getObjByNpcId(info.NPCId);
                        if (oldNpc.GetComponent<NPCAttributes>().taskId != 0)
                        {
                            return false;
                        }
                        removeObj(oldNpc);
                    }
                    else
                        return false;
                }
            }
            else
            {
                if (getObjByNpcId(info.NPCId) != null)
                {
                    GameObject oldNpc = getObjByNpcId(info.NPCId);
                    removeObj(oldNpc);
                }
            }
            Chunk chunk = World.world.GetChunk((int)info.position.x, (int)info.position.y, (int)info.position.z);
            if (chunk == null || !chunk.isGenerated)
            {
                MTBTaskController.Instance.taskCacherData.addPrepareInitNpc(info);
                return false;
            }
            return true;
        }

        private void onFirstWorldFinish(params object[] paras)
        {
            EventManager.UnRegisterEvent(EventMacro.GENERATE_FIRST_WORLD_FINISH, onFirstWorldFinish);
            EventManager.RegisterEvent(EventMacro.CHUNK_GENERATE_FINISH, onChunckGenerated);
        }

        private void onChunckGenerated(params object[] paras)
        {
            if (MTBTaskController.Instance.taskCacherData.PrepareInitNpcMap.Count <= 0)
                return;
            Chunk chunk = (Chunk)paras[0];
            List<Vector3> removeList = new List<Vector3>();
            foreach (Vector3 key in MTBTaskController.Instance.taskCacherData.PrepareInitNpcMap.Keys)
            {
                Chunk targetChunk = World.world.GetChunk((int)key.x, (int)key.y, (int)key.z);
                if (targetChunk != null && targetChunk.worldPos.Equals(chunk.worldPos))
                {
                    InitNPC(MTBTaskController.Instance.taskCacherData.PrepareInitNpcMap[key]);
                    removeList.Add(key);
                }
            }
            foreach (Vector3 removekey in removeList)
            {
                MTBTaskController.Instance.taskCacherData.PrepareInitNpcMap.Remove(removekey);
            }
        }

        protected override void addObj(GameObject obj, GameObjectInfo info)
        {
            NPCInfo npcInfo = info as NPCInfo;
            obj.GetComponent<NPCAttributes>().NPCType = npcInfo.NPCType;
            obj.GetComponent<NPCAttributes>().taskId = npcInfo.taskId;
            obj.GetComponent<NPCAttributes>().stepId = npcInfo.stepId;
            base.addObj(obj, info);
            if (info.objectId == 3)
            {
                obj.GetComponent<GONPCController>().HideAvatar();
            }
        }

        public override void removeObj(GameObject obj)
        {
            base.removeObj(obj);
        }

        public GameObject getObjByTaskId(int taskId, int stepId)
        {
            foreach (int key in _mapObj.Keys)
            {
                if (_mapObj[key].GetComponent<NPCAttributes>().taskId == taskId && _mapObj[key].GetComponent<NPCAttributes>().stepId == stepId)
                    return _mapObj[key];
            }
            return null;
        }

        public GameObject getObjByNpcId(int npcId)
        {
            foreach (int key in _mapObj.Keys)
            {
                if (_mapObj[key].GetComponent<NPCAttributes>().NPCId == npcId)
                    return _mapObj[key];
            }
            return null;
        }
    }
}
