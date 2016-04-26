using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ChunkMesh : MonoBehaviour
    {
        public Material chunkMaterial;
        private MeshFilter filter;
        private MeshCollider collider;
        private MeshRenderer render;


        void Awake()
        {
            filter = GetComponent<MeshFilter>();
            collider = GetComponent<MeshCollider>();
            render = GetComponent<MeshRenderer>();
            render.sharedMaterial = chunkMaterial;
        }

        public void SetChunkTexture(string name, Texture2D tex)
        {
            render.sharedMaterial.SetTexture(name, tex);
        }

        public void SetTexWidth(string name, float width)
        {
            render.sharedMaterial.SetFloat(name, width);
        }

        public void SetAdjustLight(string name, float value)
        {
            render.sharedMaterial.SetFloat(name, value);
        }

        public float GetAdjustLight(string name)
        {
            return render.sharedMaterial.GetFloat(name);
        }

        private Mesh mesh;
        private int layerMask;
        public void SetFilterMeshData(FilterMeshData filterMeshData)
        {
            Mesh mesh = filter.sharedMesh;
            if (mesh == null)
            {
				mesh = MeshMemoryCache.Instance.GetMesh();
			}
			else
            {
                mesh.Clear();
            }
            mesh.vertices = filterMeshData.vertices.ToArray();
            mesh.triangles = filterMeshData.triangles.ToArray();
            mesh.uv = filterMeshData.uv.ToArray();
            mesh.colors = filterMeshData.colors.ToArray();
//			Debug.Log("vertices:" + filterMeshData.vertices.Count +
//			          " triangles:" + filterMeshData.triangles.Count + 
//			          " uv:" + filterMeshData.uv.Count + 
//			          " colors:" + filterMeshData.colors.Count);
            mesh.RecalculateNormals();
			mesh.Optimize();
			mesh.MarkDynamic();
            filter.sharedMesh = null;
			filter.sharedMesh = mesh;
			
//            if(mesh == null)
//            {
//            	mesh = new Mesh();
//            }
//            else
//            {
//            	mesh.Clear();
//            }
//            mesh.vertices = filterMeshData.vertices.ToArray();
//            mesh.triangles = filterMeshData.triangles.ToArray();
//            mesh.uv = filterMeshData.uv.ToArray();
//            mesh.colors = filterMeshData.colors.ToArray();
//            mesh.RecalculateNormals();
//			mesh.Optimize();
//            layerMask = LayerMask.NameToLayer("TerrainMesh");

        }

//        void Update()
//        {
//        	if(mesh != null)
//        	{
//        		Graphics.DrawMesh(mesh,transform.position,Quaternion.identity,chunkMaterial,layerMask);
//        	}
//        }

        public void SetColliderMeshData(ColliderMeshData colliderMeshData)
        {
//            Mesh colMesh = collider.sharedMesh;
//            if (colMesh == null)
//            {
//				colMesh = MeshMemoryCache.Instance.GetMesh();
//            }
//            else
//            {
//                colMesh.Clear();
//            }
//            colMesh.vertices = colliderMeshData.colVertices.ToArray();
//            colMesh.triangles = colliderMeshData.colTriangles.ToArray();
//            colMesh.RecalculateNormals();
//			colMesh.Optimize();
//			colMesh.MarkDynamic();
//            collider.sharedMesh = null;
//            collider.sharedMesh = colMesh;
			BoxColliderAdd(colliderMeshData);
        }

		private void BoxColliderAdd(ColliderMeshData colliderMeshData)
		{
			BoxCollider[] colliders = this.GetComponents<BoxCollider>();
			int i;
			for (i = 0; i < colliderMeshData.boxCenters.Count; i++) {
				BoxCollider col;
				if(i < colliders.Length)
				{
					col = colliders[i];
				}
				else
				{
					col = this.gameObject.AddComponent<BoxCollider>();
				}
				col.center = colliderMeshData.boxCenters[i];
				col.size = colliderMeshData.boxSizes[i];
			}
			for (int j = i; j < colliders.Length; j++) {
				GameObject.Destroy(colliders[j]);
			}
		}

        void OnDestroy()
        {
            filter = null;
//            collider = null;
            render = null;
        }

        public void Dispose()
        {
			Mesh mesh = filter.sharedMesh;
			if(mesh != null)
			{
				if(!MeshMemoryCache.Instance.SaveMesh(mesh))
				{
					Destroy(mesh);
				}
			}
			filter.sharedMesh = null;

//			mesh = collider.sharedMesh;
//			if(mesh != null)
//			{
//				if(!MeshMemoryCache.Instance.SaveMesh(mesh))
//				{
//					Destroy(mesh);
//				}
//			}
//			collider.sharedMesh = null;
        }
    }

	public class MeshMemoryCache
	{
		private int maxNum = 10;
		private Queue<Mesh> _cache;
		private static MeshMemoryCache _instance;
		public static MeshMemoryCache Instance{get{
				if(_instance == null)
				{
					_instance = new MeshMemoryCache();
				}
				return _instance;
			}}
		public MeshMemoryCache()
		{
			_cache = new Queue<Mesh>();
		}
		
		public bool SaveMesh(Mesh mesh)
		{
			if(_cache.Count >= maxNum)return false;
			mesh.Clear();
			_cache.Enqueue(mesh);
			return true;
		}
		
		public Mesh GetMesh()
		{
			if(_cache.Count > 0)
			{
				return _cache.Dequeue();
			}
			return new Mesh();
		}
	}
}

