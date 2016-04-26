using UnityEngine;
using System.Collections;

public class EventMacro
{
    public const string SELECT_WORLD = "SelectWorld";
    //开始产生第一次进入世界的地形
    public const string START_GENERATE_FIRST_WORLD = "StartGenerateFirstWorld";
    //区块产生完成(这里的产生完成是区块渲染完成)
    public const string CHUNK_GENERATE_FINISH = "ChunkGenerateFinish";

    //区块数据(地形数据)产生完成（这里是在线程中抛出来的，监听该事件切记勿调用unity的API,否则会报错）
    public const string CHUNK_DATA_GENERATE_FINISH = "ChunkGenerateDataFinish";
    //区块数据移除完成（这里是在线程中抛出来的，监听该事件切记勿调用unity的API,否则会报错）
    public const string CHUNK_DATA_REMOVE_FINISH = "ChunkRemoveDataFinish";
    //区块数据保存，只为保存服务
    public const string CHUNK_DATA_SAVE = "ChunkDataSave";

    //产生生物群落之后的物块更新(这里就是认为的修改了，数量不会很多，可以每次提交一个)
    public const string BLOCK_DATA_UPDATE_AFTER_POPULATION = "BlockDataUpdateAfterPopulation";

    //第一次进入世界的地形产生完成
    public const string GENERATE_FIRST_WORLD_FINISH = "GenerateFirstWorldFinish";
    //块开始渲染
    public const string CHUNK_RENDER_START = "ChunkRenderStart";

    //进入场景成功
    public const string ON_JOIN_SCENE_SUCCESS = "On_Join_Scene_Success";

    public const string ON_CLICK_JUMP = "OnClickJump";

    public const string ON_CLICK_JUMP_DOWN = "OnClickJumpDown";

    public const string ON_CLICK_JUMP_UP = "OnClickJumpUp";
    public const string AUTO_JUMP_START = "AutoJumpStart";

    public const string AUTO_JUMP_END = "AutoJumpEnd";

    public const string ON_CHANGE_HANDCUBE = "OnChangHandCube";
    public const string ON_CLICK_SWITCH_VIEW = "OnClickSwitchView";

    public const string ON_CLICK_SWITCH_PROPMODEL = "OnClickSwitchPropModel";
    public const string ON_SWITCH_PROPMODEL = "OnSwitchPropModel";

    public const string AUTO_MOVE = "AutoMove";
    public const string AUTO_MOVE_END = "AutoMoveEnd";

    public const string MOVE_TO_DESPOINT = "MoveToDespoint";


    public const string SHOW_UI = "ShowUI";
    public const string CLOSE_UI = "CloseUI";

    public const string LOADING_UI_FINISH = "Loading_UI_Finish";
    public const string ON_BREED = "onBreed";
    public const string BREED_FINISH = "BreedFinish";

    public const string TASK_FINISH_STEP = "TaskFinishStep";
    public const string FIRST_INIT_TASK = "TaskChacheDataFirst_Init_Task";

    public const string NPC_INITED = "npc_Inited";

    public const string TASK_AUTO_START = "taskAutoStart";

}
