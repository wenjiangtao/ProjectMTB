using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
    public class HandMountPointController : MonoBehaviour
    {
        private const string CUBEPATH = "Prefabs/Props/Cube";
        private const string SURFACECUBEPATH = "Prefabs/Props/SurfaceCube";
        private const string MODELPATH = "Prefabs/Props/Item_";
        public Transform mountPoint;
        private GameObject _curHandObj;
        public GameObject CurHandObj { get { return _curHandObj; } }
        private GameObject _cube;
        private GameObject _surfaceCube;
        private Dictionary<int, GameObject> _modelMap;

        void Awake()
        {
            _modelMap = new Dictionary<int, GameObject>();
        }

        public void ChangeHandObj(int id, bool isFirst)
        {
            if (id <= 0)
            {
                HideCurObj();
                return;
            }
            Item item = ItemManager.Instance.GetItem(id);
            if (item.itemType == (int)ItemType.Block)
            {
                if (BlockAttributeCalculatorFactory.GetCalculator((BlockType)item.sceneBlockType).GetBlockRenderType(item.sceneBlockExtendId) == BlockRenderType.Part)
                    ChangeSurfaceObj((BlockType)item.sceneBlockType, item.sceneBlockExtendId, isFirst);
                else
                    ChangeCubeObj((BlockType)item.sceneBlockType, item.sceneBlockExtendId, isFirst);
            }
            else
            {
                ChangeModelObj(id, isFirst);
            }
        }

        public void UpdateLightIntensity(float lightIntensity)
        {
            if (_curHandObj != null)
            {
                _curHandObj.GetComponent<MeshRenderer>().material.SetFloat("_lightIntensity", lightIntensity);
            }
        }

        private void ChangeModelObj(int id, bool isFirst)
        {
            HideCurObj();
            GameObject modelObj;
            _modelMap.TryGetValue(id, out modelObj);
            if (modelObj == null)
            {
                GameObject modelPrefab = ResourceManager.Instance.LoadAsset<GameObject>(MODELPATH + id) as GameObject;
                if (modelPrefab == null)
                {
                    Debug.Log("找不到资源为:" + MODELPATH + id + "的模型!");
                    return;
                }
                modelObj = GameObject.Instantiate<GameObject>(modelPrefab);
                modelObj.transform.parent = mountPoint;
                if (isFirst)
                {
                    modelObj.transform.localPosition = modelPrefab.transform.localPosition;
                    modelObj.transform.localRotation = modelPrefab.transform.localRotation;
                    modelObj.transform.localScale = modelPrefab.transform.localScale;
                }
                else
                {
                    modelObj.transform.localPosition = Vector3.zero;
                    modelObj.transform.localRotation = Quaternion.identity;
                    modelObj.transform.localScale = new Vector3(1, 1, 1);
                }
                modelObj.SetActive(false);
                _modelMap.Add(id, modelObj);
            }
            _curHandObj = modelObj;
            _curHandObj.SetActive(true);
        }

        private void ChangeSurfaceObj(BlockType type, byte extendId, bool isFirst)
        {
            HideCurObj();
            RefreshSurfaceCubeObj(type, extendId, isFirst);
            RefreshSurfaceCubeMesh(type, extendId);
            _curHandObj = _surfaceCube;
            _curHandObj.layer = mountPoint.gameObject.layer;
            _curHandObj.SetActive(true);
        }

        private void ChangeCubeObj(BlockType type, byte extendId, bool isFirst)
        {
            HideCurObj();
            RefreshCubeObj(type, extendId, isFirst);
            RefreshCubeMesh(type, extendId);
            _curHandObj = _cube;
            _curHandObj.layer = mountPoint.gameObject.layer;
            _curHandObj.SetActive(true);
        }

        private void RefreshCubeObj(BlockType type, byte extendId, bool isFirst)
        {
            if (_cube == null)
            {
                GameObject cubePrefab = ResourceManager.Instance.LoadAsset<GameObject>(CUBEPATH) as GameObject;
                _cube = GameObject.Instantiate<GameObject>(cubePrefab);
                _cube.transform.parent = mountPoint;
                if (isFirst)
                {
                    _cube.transform.localPosition = cubePrefab.transform.localPosition;
                    _cube.transform.localRotation = cubePrefab.transform.localRotation;
                    _cube.transform.localScale = cubePrefab.transform.localScale;
                }
                else
                {
                    _cube.transform.localPosition = Vector3.zero;
                    _cube.transform.localRotation = Quaternion.identity;
                    _cube.transform.localScale = cubePrefab.transform.localScale;
                }
            }
            _cube.SetActive(false);

            if (!WorldTextureProvider.IsCanUseProvider)
                WorldTextureProvider.Instance.Pack();
            WorldTextureType worldTextureType = WorldTextureProvider.Instance.GetTextureType(type, extendId);
            Texture2D tex = WorldTextureProvider.Instance.GetAtlasTexture(worldTextureType);
            _cube.GetComponent<MeshRenderer>().material.mainTexture = tex;
        }

        private FilterMeshData filterMeshData = new FilterMeshData();
        private void RefreshCubeMesh(BlockType type, byte extendId)
        {
            filterMeshData.Clear();
            int x = 0, y = 0, z = 0;

            filterMeshData.vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
            filterMeshData.vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
            filterMeshData.vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
            filterMeshData.vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

            AddTriangles(filterMeshData);
            AddUV(filterMeshData, type, extendId, Direction.up);

            filterMeshData.vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
            filterMeshData.vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
            filterMeshData.vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
            filterMeshData.vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
            AddTriangles(filterMeshData);
            AddUV(filterMeshData, type, extendId, Direction.down);

            filterMeshData.vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
            filterMeshData.vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
            filterMeshData.vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
            filterMeshData.vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
            AddTriangles(filterMeshData);
            AddUV(filterMeshData, type, extendId, Direction.left);

            filterMeshData.vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
            filterMeshData.vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
            filterMeshData.vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
            filterMeshData.vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
            AddTriangles(filterMeshData);
            AddUV(filterMeshData, type, extendId, Direction.right);

            filterMeshData.vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
            filterMeshData.vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
            filterMeshData.vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
            filterMeshData.vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
            AddTriangles(filterMeshData);
            AddUV(filterMeshData, type, extendId, Direction.front);

            filterMeshData.vertices.Add(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
            filterMeshData.vertices.Add(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
            filterMeshData.vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
            filterMeshData.vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
            AddTriangles(filterMeshData);
            AddUV(filterMeshData, type, extendId, Direction.back);

            MeshFilter filter = _cube.GetComponent<MeshFilter>();
            filter.mesh.vertices = filterMeshData.vertices.ToArray();
            filter.mesh.triangles = filterMeshData.triangles.ToArray();
            filter.mesh.uv = filterMeshData.uv.ToArray();
        }

        private void AddTriangles(FilterMeshData filterMeshData)
        {
            filterMeshData.triangles.Add(filterMeshData.vertices.Count - 4);
            filterMeshData.triangles.Add(filterMeshData.vertices.Count - 3);
            filterMeshData.triangles.Add(filterMeshData.vertices.Count - 2);

            filterMeshData.triangles.Add(filterMeshData.vertices.Count - 4);
            filterMeshData.triangles.Add(filterMeshData.vertices.Count - 2);
            filterMeshData.triangles.Add(filterMeshData.vertices.Count - 1);
        }

        protected void AddUV(FilterMeshData filterMeshData, BlockType type, byte extendId, Direction direction)
        {
            BlockAttributeCalculator calculator = BlockAttributeCalculatorFactory.GetCalculator(type);
            Rect uvRect = calculator.GetUVRect(extendId, direction);
            float scale = uvRect.width * 0.03f;
            filterMeshData.uv.Add(new Vector2(uvRect.x + uvRect.width - scale, uvRect.y + uvRect.height - scale));
            filterMeshData.uv.Add(new Vector2(uvRect.x + scale, uvRect.y + uvRect.height - scale));
            filterMeshData.uv.Add(new Vector2(uvRect.x + scale, uvRect.y + scale));
            filterMeshData.uv.Add(new Vector2(uvRect.x + uvRect.width - scale, uvRect.y + scale));
        }

        private void RefreshSurfaceCubeObj(BlockType type, byte extendId, bool isFirst)
        {
            if (_surfaceCube == null)
            {
                GameObject cubePrefab = ResourceManager.Instance.LoadAsset<GameObject>(SURFACECUBEPATH) as GameObject;
                _surfaceCube = GameObject.Instantiate<GameObject>(cubePrefab);
                _surfaceCube.transform.parent = mountPoint;
                if (isFirst)
                {
                    _surfaceCube.transform.localPosition = cubePrefab.transform.localPosition;
                    _surfaceCube.transform.localRotation = cubePrefab.transform.localRotation;
                    _surfaceCube.transform.localScale = cubePrefab.transform.localScale;
                }
                else
                {
                    _surfaceCube.transform.localPosition = Vector3.zero;
                    _surfaceCube.transform.localRotation = Quaternion.identity;
                    _surfaceCube.transform.localScale = cubePrefab.transform.localScale;
                }
            }
            _surfaceCube.SetActive(false);

            if (!WorldTextureProvider.IsCanUseProvider)
                WorldTextureProvider.Instance.Pack();
            WorldTextureType worldTextureType = WorldTextureProvider.Instance.GetTextureType(type, extendId);
            Texture2D tex = WorldTextureProvider.Instance.GetAtlasTexture(worldTextureType);
            _surfaceCube.GetComponent<MeshRenderer>().material.mainTexture = tex;
        }

        private void RefreshSurfaceCubeMesh(BlockType type, byte extendId)
        {
            filterMeshData.Clear();
            int x = 0, y = 0, z = 0;

            filterMeshData.vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
            filterMeshData.vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
            filterMeshData.vertices.Add(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
            filterMeshData.vertices.Add(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));

            AddTriangles(filterMeshData);
            AddUV(filterMeshData, type, extendId, Direction.up);

            MeshFilter filter = _surfaceCube.GetComponent<MeshFilter>();
            filter.mesh.vertices = filterMeshData.vertices.ToArray();
            filter.mesh.triangles = filterMeshData.triangles.ToArray();
            filter.mesh.uv = filterMeshData.uv.ToArray();
        }

        private void HideCurObj()
        {
            if (_curHandObj != null)
            {
                _curHandObj.SetActive(false);
            }
        }
    }
}

