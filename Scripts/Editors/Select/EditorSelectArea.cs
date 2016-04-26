using UnityEngine;
using System;
using System.Collections.Generic;
using MTB;

public class EditorSelectArea : Singleton<EditorSelectArea>
{
    public GameObject selectViewGo;
    private GameObject lastSelectViewGo;
    private bool startSelectMark;
    public bool cutMark;
    private bool cutDragMark;
    public bool firstDragMoveMark { get; private set; }
    private byte[] blockTypes;
    private byte[] extendIds;
    private int totalBlocks;

    private Dictionary<Vector3, selectBlock> originalBlocks;
    private Dictionary<Vector3, selectBlock> oldOriginalBlocks;

    private Transform _playerTrans;

    private Vector3 startPoint;
    private Vector3 endPoint;

    private Vector3 centerPoint;
    private int proportionIndex;

    private float minx;
    private float miny;
    private float minz;

    private float maxx;
    private float maxy;
    private float maxz;

    private float oldminx;
    private float oldminy;
    private float oldminz;

    private int width;
    private int depth;
    private int height;

    private float rayDis;

    private int xzScale;

    private float mousey;
    private float mousex;
    private SelectAreaVDragType volumeDragType;


    public void Init()
    {
        if (selectViewGo == null)
        {
            selectViewGo = GameObject.Instantiate(Resources.Load("Prefabs/EditorPrefabs/SelectColorMask") as GameObject) as GameObject;
            selectViewGo.transform.name = "SelectArea";
            selectViewGo.AddComponent<MeshCollider>();
            enableDrag(false);
        }
        if (lastSelectViewGo == null)
        {
            lastSelectViewGo = GameObject.Instantiate(Resources.Load("Prefabs/EditorPrefabs/LastSelectMask") as GameObject) as GameObject;
            lastSelectViewGo.transform.name = "lastSelectArea";
        }
        rayDis = 10f;
        proportionIndex = 1;
        _playerTrans = HasActionObjectManager.Instance.playerManager.getMyPlayer().transform;
        firstDragMoveMark = true;
    }

    //拉选选择区域
    public void updateArea(Vector3 point)
    {
        if (!startSelectMark)
        {
            startPoint = point;
            startSelectMark = true;
        }
        else
        {
            UpdateSurface();
            endPoint = point;
        }
    }

    private void enableDrag(bool b)
    {
        selectViewGo.GetComponent<MeshCollider>().enabled = b;
    }

    public bool hasData()
    {
        return blockTypes != null;
    }

    //根据点击面确定拖拽方向
    public bool setAreaDragType(Vector3 point)
    {
        if (point.Equals(Vector3.zero))
            return false;

        if (point.x == minx) volumeDragType = SelectAreaVDragType.XMIN;
        else if (point.x == maxx) volumeDragType = SelectAreaVDragType.XMAX;
        else if (point.y == miny) volumeDragType = SelectAreaVDragType.YMIN;
        else if (point.y == maxy) volumeDragType = SelectAreaVDragType.YMAX;
        else if (point.z == minz) volumeDragType = SelectAreaVDragType.ZMIN;
        else if (point.z == maxz) volumeDragType = SelectAreaVDragType.ZMAX;

        mousey = Input.mousePosition.y;
        mousex = Input.mousePosition.x;
        enableDrag(false);
        return true;
    }

    //拖拽更新选择区域范围
    public void updateDragVolume(float deltaTime)
    {
        xzScale = 1;
        if (volumeDragType == SelectAreaVDragType.XMIN)
        {
            if (_playerTrans.eulerAngles.y >= 90f && _playerTrans.eulerAngles.y <= 270f)
                xzScale = -1;
            minx += xzScale * (Input.mousePosition.x - mousex) * deltaTime * 3;
            minx = minx >= maxx - 1 ? maxx - 1 : minx;
        }
        else if (volumeDragType == SelectAreaVDragType.XMAX)
        {
            if (_playerTrans.eulerAngles.y >= 90f && _playerTrans.eulerAngles.y <= 270f)
                xzScale = -1;
            maxx += xzScale * (Input.mousePosition.x - mousex) * deltaTime * 3;
            maxx = maxx <= minx + 1 ? minx + 1 : maxx;
        }
        else if (volumeDragType == SelectAreaVDragType.YMIN)
        {
            miny += (Input.mousePosition.y - mousey) * deltaTime * 3;
            miny = miny >= maxy - 1 ? maxy - 1 : miny;
        }
        else if (volumeDragType == SelectAreaVDragType.YMAX)
        {
            maxy += (Input.mousePosition.y - mousey) * deltaTime * 3;
            maxy = maxy <= miny + 1 ? miny + 1 : maxy;
        }
        else if (volumeDragType == SelectAreaVDragType.ZMIN)
        {
            if (_playerTrans.eulerAngles.y <= 180f)
                xzScale = -1;
            minz += xzScale * (Input.mousePosition.x - mousex) * deltaTime * 3;
            minz = minz >= maxz - 1 ? maxz - 1 : minz;
        }
        else if (volumeDragType == SelectAreaVDragType.ZMAX)
        {
            if (_playerTrans.eulerAngles.y <= 180f)
                xzScale = -1;
            maxz += xzScale * (Input.mousePosition.x - mousex) * deltaTime * 3;
            maxz = maxz <= minz + 1 ? minz + 1 : maxz;
        }
        centerPoint = new Vector3((float)((int)minx + (int)maxx) / 2, (float)((int)miny + (int)maxy) / 2, (float)((int)minz + (int)maxz) / 2);
        updateSelectView();
        mousey = Input.mousePosition.y;
        mousex = Input.mousePosition.x;
    }

    public void selectBigArea()
    {
        width = 100;
        depth = 61;
        height = 61;
    }

    //旋转选区
    public void changeAreaProportion(int proportionInc = 1)
    {
        int temp = width;
        width = depth;
        depth = temp;

        minx = (int)centerPoint.x - width / 2;
        miny = (int)centerPoint.y - height / 2;
        minz = (int)centerPoint.z - depth / 2;
        maxx = minx + width;
        maxy = miny + height;
        maxz = minz + depth;
        updateSelectView();
        if (hasData())
        {
            changeBlocksProportion(proportionInc);
        }
    }

    //旋转选区
    private void changeBlocksProportion(int proportionInc)
    {
        proportionIndex += proportionIndex;
        saveOriginalData();
        updateSelectBlocksView(cutMark, proportionIndex);
        proportionIndex = 1;
    }

    public void endDrag()
    {
        enableDrag(true);
        minx = (int)minx;
        miny = (int)miny;
        minz = (int)minz;
        maxx = (int)maxx;
        maxy = (int)maxy;
        maxz = (int)maxz;
        width = (int)maxx - (int)minx;
        depth = (int)maxz - (int)minz;
        height = (int)maxy - (int)miny;
        rayDis = 10f;
        saveOriginalData();
    }

    public void startDragMoveArea()
    {
        oldminx = (int)minx;
        oldminy = (int)miny;
        oldminz = (int)minz;
        updateLastSelectView();
    }

    //更新拖拽选中区域位置
    public void updateDragMoveArea()
    {
        enableDrag(false);
        rayDis += Input.mouseScrollDelta.y;
        rayDis = rayDis <= Math.Max(width, depth) / 2 ? Math.Max(width, depth) / 2 : rayDis;

        minx = (int)EditorScreenRay.Instance.getMouseRayPositionByDistance(rayDis).x - width / 2;
        miny = (int)EditorScreenRay.Instance.getMouseRayPositionByDistance(rayDis).y - height / 2;
        minz = (int)EditorScreenRay.Instance.getMouseRayPositionByDistance(rayDis).z - depth / 2;
        maxx = minx + width;
        maxy = miny + height;
        maxz = minz + depth;
        centerPoint = new Vector3((float)((int)minx + (int)maxx) / 2, (float)((int)miny + (int)maxy) / 2, (float)((int)minz + (int)maxz) / 2);
        updateSelectView();
        mousey = Input.mousePosition.y;
        mousex = Input.mousePosition.x;
    }

    public void moveAreaFinish()
    {
        endDrag();
        updateSelectBlocksView(cutMark);
    }

    //拉选区域时更新选择框位置大小
    private void UpdateSurface()
    {
        minx = Math.Min(startPoint.x, endPoint.x + 1);
        miny = Math.Min(startPoint.y, endPoint.y + 1);
        minz = Math.Min(startPoint.z, endPoint.z + 1);

        maxx = Math.Max(startPoint.x, endPoint.x + 1);
        maxy = Math.Max(startPoint.y, endPoint.y + 1);
        maxz = Math.Max(startPoint.z, endPoint.z + 1);

        minx = minx >= maxx - 1 ? maxx - 1 : minx;
        miny = miny >= maxy - 1 ? maxy - 1 : miny;
        minz = minz >= maxz - 1 ? maxz - 1 : minz;

        centerPoint = new Vector3((float)((int)minx + (int)maxx) / 2, (float)((int)miny + (int)maxy) / 2, (float)((int)minz + (int)maxz) / 2);
        updateSelectView();
    }

    //更新选择框位置
    private void updateSelectView()
    {
        selectViewGo.SetActive(true);
        selectViewGo.transform.position = new Vector3((float)((int)minx + (int)maxx) / 2, (float)((int)miny + (int)maxy) / 2, (float)((int)minz + (int)maxz) / 2);
        selectViewGo.transform.localScale = new Vector3((int)maxx - (int)minx, (int)maxy - (int)miny, (int)maxz - (int)minz);
    }

    //更新上个选区位置/载入文件时也需要保留上个选区位置
    public void updateLastSelectView()
    {
        lastSelectViewGo.SetActive(true);
        lastSelectViewGo.transform.position = new Vector3((float)(oldminx + (float)width / 2), (float)(oldminy + (float)height / 2), (float)(oldminz + (float)depth / 2));
        lastSelectViewGo.transform.localScale = new Vector3(width, height, depth);
    }

    //刷新选中区域的数据,主要用于拖拽和剪切选中块
    private void updateSelectBlocksView(bool cutmark, int proportion = 1)
    {
        List<RefreshChunk> updateChunkList = new List<RefreshChunk>();
        List<Chunk> chunklist = new List<Chunk>();
        Chunk chunk;
        if (cutmark)
        {
            if (oldOriginalBlocks != null)
            {
                foreach (selectBlock b in oldOriginalBlocks.Values)
                {

                    chunk = World.world.GetChunk((int)b.pos.x, (int)b.pos.y, (int)b.pos.z);
                    if (chunklist.Contains(chunk))
                    {
                        if (firstDragMoveMark)
                            updateChunkList[chunklist.IndexOf(chunk)].refreshList.Add(new UpdateBlock(BlockType.Air, 0, new WorldPos((int)b.pos.x, (int)b.pos.y, (int)b.pos.z)));
                        else
                            updateChunkList[chunklist.IndexOf(chunk)].refreshList.Add(new UpdateBlock((BlockType)b.type, b.exid, new WorldPos((int)b.pos.x, (int)b.pos.y, (int)b.pos.z)));
                    }
                    else
                    {
                        chunklist.Add(chunk);
                        updateChunkList.Add(new RefreshChunk(chunk));
                        if (firstDragMoveMark)
                            updateChunkList[0].refreshList.Add(new UpdateBlock(BlockType.Air, 0, new WorldPos((int)b.pos.x, (int)b.pos.y, (int)b.pos.z)));
                        else
                            updateChunkList[0].refreshList.Add(new UpdateBlock((BlockType)b.type, b.exid, new WorldPos((int)b.pos.x, (int)b.pos.y, (int)b.pos.z)));
                    }
                }
            }
        }
        firstDragMoveMark = false;
        for (int x = (int)minx; x < (int)maxx; x++)
        {
            for (int y = (int)miny; y < (int)maxy; y++)
            {
                for (int z = (int)minz; z < (int)maxz; z++)
                {
                    int index = ((x - (int)minx) * depth + z - (int)minz) * height + (y - (int)miny);
                    if (proportion == 2)
                        index = (((int)maxz - z - 1) * width + x - (int)minx) * height + (y - (int)miny);
                    else if (proportion == 0)
                        index = ((z - (int)minz) * width + (int)maxx - x - 1) * height + (y - (int)miny);
                    if (index >= extendIds.Length || index < 0) continue;
                    byte extendId = (byte)extendIds[index];
                    chunk = World.world.GetChunk(x, y, z);
                    if (chunklist.Contains(chunk))
                    {
                        updateChunkList[chunklist.IndexOf(chunk)].refreshList.Add(new UpdateBlock((BlockType)blockTypes[index], extendId, new WorldPos(x, y, z)));
                    }
                    else
                    {
                        chunklist.Add(chunk);
                        updateChunkList.Add(new RefreshChunk(chunk));
                        updateChunkList[0].refreshList.Add(new UpdateBlock((BlockType)blockTypes[index], extendId, new WorldPos(x, y, z)));
                    }
                }
            }
        }
        RefreshChunkArea chunkArea = new RefreshChunkArea(updateChunkList);
        World.world.WorlderLoader.updateArea(chunkArea);
        if (proportion != 1)
        {
            MTBEditorModeController.Instance.changeMode(MTBEditorModeType.EDITOR);
            blockTypes = null;
        }
    }

    public void cancel()
    {
        firstDragMoveMark = true;
        proportionIndex = 1;
        enableDrag(false);
        blockTypes = null;
        extendIds = null;
        originalBlocks = null;
        oldOriginalBlocks = null;
        if (selectViewGo == null)
            return;
        if (selectViewGo.activeSelf)
        {
            selectViewGo.SetActive(false);
        }
        if (lastSelectViewGo != null && lastSelectViewGo.activeSelf)
        {
            lastSelectViewGo.SetActive(false);
        }
        startSelectMark = false;
        startPoint = Vector3.zero;
        endPoint = Vector3.zero;
    }

    //保存选区原本数据
    private void saveOriginalData()
    {
        if (oldOriginalBlocks == null)
            firstDragMoveMark = true;
        if (originalBlocks != null)
            oldOriginalBlocks = originalBlocks;
        totalBlocks = width * depth * height;
        originalBlocks = new Dictionary<Vector3, selectBlock>(new SelectBlockComparer());
        for (int x = (int)minx; x < (int)minx + width; x++)
        {
            for (int y = (int)miny; y < (int)miny + height; y++)
            {
                for (int z = (int)minz; z < (int)minz + depth; z++)
                {
                    Vector3 posKey = new Vector3(x, y, z);
                    if (oldOriginalBlocks != null && oldOriginalBlocks.ContainsKey(posKey))
                    {
                        originalBlocks.Add(posKey, oldOriginalBlocks[posKey]);
                    }
                    else
                    {
                        originalBlocks.Add(posKey, new selectBlock((byte)World.world.GetBlock(x, y, z).BlockType, (byte)World.world.GetBlock(x, y, z).ExtendId, new Vector3(x, y, z)));
                    }
                }
            }
        }
    }

    public void unDo()
    {
        minx = (int)oldminx;
        miny = (int)oldminy;
        minz = (int)oldminz;
        maxx = minx + width;
        maxy = miny + height;
        maxz = minz + depth;
        saveOriginalData();
        updateSelectBlocksView(true);
    }

    public SelectAreaParams save()
    {
        firstDragMoveMark = true;
        if (lastSelectViewGo != null && lastSelectViewGo.activeSelf)
        {
            lastSelectViewGo.SetActive(false);
        }
        totalBlocks = width * depth * height;
        blockTypes = new byte[totalBlocks];
        extendIds = new byte[totalBlocks];
        for (int x = (int)minx; x < (int)maxx; x++)
        {
            for (int y = (int)miny; y < (int)maxy; y++)
            {
                for (int z = (int)minz; z < (int)maxz; z++)
                {
                    int index = ((x - (int)minx) * depth + (z - (int)minz)) * height + y - (int)miny;
                    blockTypes[index] = (byte)World.world.GetBlock(x, y, z).BlockType;
                    extendIds[index] = World.world.GetBlock(x, y, z).ExtendId;
                }
            }
        }
        return new SelectAreaParams(width, depth, height, blockTypes, extendIds);
    }

    public void load(SelectAreaParams paras)
    {
        blockTypes = paras.blockTypes;
        extendIds = paras.extendIds;
        minx = (int)EditorScreenRay.Instance.getMouseRayPositionByDistance(rayDis).x;
        miny = (int)EditorScreenRay.Instance.getMouseRayPositionByDistance(rayDis).y;
        minz = (int)EditorScreenRay.Instance.getMouseRayPositionByDistance(rayDis).z;
        width = paras.Width;
        height = paras.Height;
        depth = paras.Depth;
        maxx = minx + width;
        maxy = miny + height;
        maxz = minz + depth;
    }

    public void fill(BlockType type,byte exid)
    {
        for (int i = 0; i < blockTypes.Length; i++)
        {
            blockTypes[i] = (byte)type;
            extendIds[i] = exid;
        }
        moveAreaFinish();
    }
}

public class SelectAreaParams
{
    public int Width;
    public int Depth;
    public int Height;
    public byte[] blockTypes;
    public byte[] extendIds;

    public SelectAreaParams(int w, int d, int h, byte[] btypes, byte[] eIds)
    {
        Width = w;
        Depth = d;
        Height = h;
        blockTypes = btypes;
        extendIds = eIds;
    }
}

public class selectBlock
{
    public byte type;
    public byte exid;
    public Vector3 pos;

    public selectBlock(byte type, byte exid, Vector3 pos)
    {
        this.type = type;
        this.exid = exid;
        this.pos = pos;
    }
}

public enum SelectAreaVDragType
{
    XMIN,
    XMAX,
    YMIN,
    YMAX,
    ZMIN,
    ZMAX
}

public class SelectBlockComparer : IEqualityComparer<Vector3>
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