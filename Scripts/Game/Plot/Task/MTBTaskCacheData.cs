/****
 * 任务缓存数据
 * 用于本地维护当前任务状态
 * 初始化npc
 * ****/
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace MTB
{
    public class MTBTaskCacheData
    {
        /**
         * 当前可进行的任务列表
         * **/
        public Dictionary<int, MTBTaskData> canConductTaskList { get; set; }

        /**
         * 正在进行的任务
         * **/
        public MTBTaskData curConDuctTaskData { get; set; }

        public Dictionary<Vector3, NPCInfo> PrepareInitNpcMap = new Dictionary<Vector3, NPCInfo>(new PrepareInitNpcPosComparer());

        public int curStep { get; set; }

        private bool initMark;
        private bool firstInitMark;

        public MTBTaskCacheData()
        {
            firstInitMark = false;
            EventManager.RegisterEvent(EventMacro.FIRST_INIT_TASK, onFirstInitTask);
            initMark = false;
            if (!checkCanInitTask())
            {
                canConductTaskList = new Dictionary<int, MTBTaskData>();
                initMark = true;
                return;
            }
            taskCacheData data = MTBTaskCacheDataLoader.loadData();
            if (data.taskId <= 1 && data.stepId == 0)
            {
                int[] picIds = { 1, 2 };
                ComicInfo comicinfo = new ComicInfo(picIds, 7);
                MTBComicController.Instance.playComicByTime(comicinfo);
            }
            canConductTaskList = data.canConductTaskList;
            PrepareInitNpcMap = data.PrepareInitNpcMap;
            curStep = data.stepId;
            if (data.taskId != 0)
                curConDuctTaskData = canConductTaskList[data.taskId];
            initMark = true;
            if (firstInitMark)
                initNpc();
        }

        //单线任务
        public MTBTaskData nextMainlineData
        {
            get
            {
                MTBTaskData data;
                int nextkey = 0;
                foreach (int key in canConductTaskList.Keys)
                {
                    if (nextkey != 0)
                        nextkey = Math.Min(nextkey, key);
                    else
                        nextkey = key;
                }
                data = canConductTaskList[nextkey];
                return data;
            }
        }

        private void onFirstInitTask(params object[] paras)
        {
            EventManager.UnRegisterEvent(EventMacro.FIRST_INIT_TASK, onFirstInitTask);
            firstInitMark = true;
        }

        public bool startTask(int taskId, int step)
        {
            if (taskId == 0 || step == 0) return false;
            if (!initMark)
                throw new Exception("任务缓存数据没有初始化！");
            if (canConductTaskList.ContainsKey(taskId))
            {
                curConDuctTaskData = canConductTaskList[taskId];
                curStep = step;
                return true;
            }
            return false;
        }

        public bool finishStep(int taskId, int stepId)
        {
            if (taskId == 0 || stepId == 0) return false;
            if (canConductTaskList.ContainsKey(taskId))
            {
                taskCacheData data = MTBTaskCacheDataLoader.loadData();
                data.canConductTaskList = canConductTaskList;
                data.taskId = taskId;
                data.stepId = stepId + 1;
                MTBTaskCacheDataLoader.saveData(data);
                return true;
            }
            return false;
        }

        public bool finishTask(int taskId)
        {
            if (taskId == 0) return false;
            if (canConductTaskList.ContainsKey(taskId))
            {
                string[] openList = canConductTaskList[taskId].openTaskList;
                checkAndDisposeTask(taskId);
                foreach (string openId in openList)
                {
                    if (Convert.ToInt32(openId) != 0)
                        checkAndInitTask(Convert.ToInt32(openId));
                }
                return true;
            }
            return false;
        }

        public void resetTask(int taskId)
        {
            curStep = 0;
            curConDuctTaskData = null;
            canConductTaskList.Clear();
            checkAndInitTask(taskId);
        }

        public void addPrepareInitNpc(NPCInfo info)
        {
            if (!MTBTaskController.Instance.taskCacherData.PrepareInitNpcMap.ContainsKey(info.position))
                MTBTaskController.Instance.taskCacherData.PrepareInitNpcMap.Add(info.position, info);
            else
                MTBTaskController.Instance.taskCacherData.PrepareInitNpcMap[info.position] = info;
            taskCacheData data = MTBTaskCacheDataLoader.loadData();
            data.PrepareInitNpcMap = PrepareInitNpcMap;
            MTBTaskCacheDataLoader.saveData(data);
        }

        //仅在第一次初始化任务数据的时候进行
        private void initNpc()
        {
            foreach (int key in canConductTaskList.Keys)
            { 
                if (key == 0) continue;
                checkAndInitTask(canConductTaskList[key].id);
            }
        }
        /***
         * 附带刷每个任务接取npc
         * ***/
        private void checkAndInitTask(int taskId)
        {
            if (taskId == 0) return;
            if (!canConductTaskList.ContainsKey(taskId))
            {
                canConductTaskList.Add(taskId, MTBTaskDataManager.Instance.getData(taskId));
            }
            taskCacheData data = MTBTaskCacheDataLoader.loadData();
            data.canConductTaskList = canConductTaskList;
            data.taskId = taskId;
            data.stepId = 0;
            MTBTaskCacheDataLoader.saveData(data);
            HasActionObjectManager.Instance.npcManager.InitNPC(taskId, 1);
        }

        private void checkAndDisposeTask(int taskId)
        {
            if (taskId == 0) return;
            curStep = 0;
            curConDuctTaskData = null;
            canConductTaskList.Remove(taskId);
        }

        private bool checkCanInitTask()
        {
            return WorldConfig.Instance.savedPath.Contains("剧情模式");
        }
    }

    public class taskCacheData
    {
        public Dictionary<int, MTBTaskData> canConductTaskList { get; set; }
        public int taskId { get; set; }
        public int stepId { get; set; }
        public Dictionary<Vector3, NPCInfo> PrepareInitNpcMap { get; set; }

        public taskCacheData()
        {
            canConductTaskList = new Dictionary<int, MTBTaskData>();
            PrepareInitNpcMap = new Dictionary<Vector3, NPCInfo>();
        }
    }

    public class MTBTaskCacheDataLoader
    {
        private static string CACHEDATAPATH = WorldConfig.Instance.savedPath + "/TaskData";
        private static FileStream writeFs;
        private static FileStream readFs;

        public static void saveData(taskCacheData data)
        {
            FileInfo fi = new FileInfo(CACHEDATAPATH);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            writeFs = fi.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
            writeFs.Position = 0;
            Serialization.WriteIntToStream(writeFs, data.taskId);
            Serialization.WriteIntToStream(writeFs, data.stepId);
            Serialization.WriteIntToStream(writeFs, data.canConductTaskList.Values.Count);
            foreach (int key in data.canConductTaskList.Keys)
            {
                Serialization.WriteIntToStream(writeFs, key);
            }

            Serialization.WriteIntToStream(writeFs, data.PrepareInitNpcMap.Keys.Count);
            foreach (Vector3 key in data.PrepareInitNpcMap.Keys)
            {
                Serialization.WriteIntToStream(writeFs, (int)data.PrepareInitNpcMap[key].position.x);
                Serialization.WriteIntToStream(writeFs, (int)data.PrepareInitNpcMap[key].position.y);
                Serialization.WriteIntToStream(writeFs, (int)data.PrepareInitNpcMap[key].position.z);
                Serialization.WriteIntToStream(writeFs, data.PrepareInitNpcMap[key].NPCId);
                Serialization.WriteIntToStream(writeFs, data.PrepareInitNpcMap[key].taskId);
                Serialization.WriteIntToStream(writeFs, data.PrepareInitNpcMap[key].stepId);
            }
            writeFs.Flush();
            writeFs.Close();
            writeFs = null;
        }

        public static taskCacheData loadData()
        {
            taskCacheData data = new taskCacheData();
            FileInfo fi = new FileInfo(CACHEDATAPATH);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            readFs = fi.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
            readFs.Position = 0;

            if (readFs.Length == 0)
            {
                data.canConductTaskList.Add(1, MTBTaskDataManager.Instance.getData(1));
                readFs.Close();
                readFs.Dispose();
                readFs = null;
                data.taskId = 0;
                data.stepId = 0;
                saveData(data);
                EventManager.SendEvent(EventMacro.FIRST_INIT_TASK);
                return data;
            }
            int curTaskId = Serialization.ReadIntFromStream(readFs);
            int curStepId = Serialization.ReadIntFromStream(readFs);
            int count = Serialization.ReadIntFromStream(readFs);
            for (int i = 0; i < count; i++)
            {
                int key = Serialization.ReadIntFromStream(readFs);
                data.canConductTaskList.Add(key, MTBTaskDataManager.Instance.getData(key));
            }

            int count2 = Serialization.ReadIntFromStream(readFs);
            data.PrepareInitNpcMap.Clear();
            for (int i = 0; i < count2; i++)
            {
                Vector3 key = new Vector3(Serialization.ReadIntFromStream(readFs), Serialization.ReadIntFromStream(readFs), Serialization.ReadIntFromStream(readFs));
                NPCInfo info = new NPCInfo();
                info.NPCId = Serialization.ReadIntFromStream(readFs);
                info.taskId = Serialization.ReadIntFromStream(readFs);
                info.stepId = Serialization.ReadIntFromStream(readFs);
                data.PrepareInitNpcMap.Add(key, info);
            }
            readFs.Flush();
            readFs.Close();
            readFs = null;

            data.taskId = curTaskId;
            data.stepId = curStepId;
            return data;
        }
    }


    public class PrepareInitNpcPosComparer : IEqualityComparer<Vector3>
    {
        #region IEqualityComparer implementation
        bool IEqualityComparer<Vector3>.Equals(Vector3 a, Vector3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }
        int IEqualityComparer<Vector3>.GetHashCode(Vector3 obj)
        {
            return obj.ToString().GetHashCode();
        }
        #endregion
    }
}
