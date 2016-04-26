using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
	public class MeshData {
		public const bool DefaultUseRenderDataForCol = true;
		public const bool DefaultUseTransparentTexture = false;
		public const bool DefaultUseSupportDataForCol = true;
		public const bool DefaultUseDoubleFace = false;

		public bool useRenderDataForCol = DefaultUseRenderDataForCol;
		public bool useTransparentTexture = DefaultUseTransparentTexture;
		public bool useSupportDataForCol = DefaultUseSupportDataForCol;
		public bool useDoubleFace = DefaultUseDoubleFace;

		public FilterMeshData terrainFilter;
		public ColliderMeshData terrainCollider;

		public FilterMeshData supportFilter;
		public ColliderMeshData supportCollider;
	    

	    public MeshData()
	    {
			terrainFilter = new FilterMeshData();
			terrainCollider = new ColliderMeshData();

			supportFilter = new FilterMeshData();
			supportCollider = new ColliderMeshData();
	    }

		public void AddUV(Vector2 uv)
		{
			if(useTransparentTexture)
			{
				supportFilter.uv.Add(uv);
			}
			else
			{
				terrainFilter.uv.Add(uv);
			}
		}

		public void AddColor(Color color)
		{
			if(useTransparentTexture)
			{
				supportFilter.colors.Add(color);
			}
			else
			{
				terrainFilter.colors.Add(color);
			}
		}

		private int[] triangleForward = new int[]{4,3,2,4,2,1};
		private int[] triangleBackward = new int[]{1,2,4,2,3,4};
		public void AddQuadTriangles(bool isForward = true)
	    {
			if(isForward)
			{
				AddQuadTrianglesOrder(triangleForward);
			}
			else
			{
				AddQuadTrianglesOrder(triangleBackward);
			}

	    }
		//不支持双面
		public void AddTriangle(int triangle)
		{
			if(useTransparentTexture)
			{
				supportFilter.triangles.Add(triangle);
			}
			else
			{
				terrainFilter.triangles.Add(triangle);
			}
		}

		public int GetCurVerticesIndex()
		{
			if(useTransparentTexture)
			{
				return supportFilter.vertices.Count;
			}
			else
			{
				return terrainFilter.vertices.Count;
			}
		}

		public int GetCurColVerticesIndex()
		{
			if(useRenderDataForCol)
			{
				return terrainCollider.colVertices.Count;
			}
			else
			{
				return supportCollider.colVertices.Count;
			}
		}

		private void AddQuadTrianglesOrder(int[] order)
		{
			if(useTransparentTexture)
			{
				for (int i = 0; i < order.Length; i++) {
					supportFilter.triangles.Add(supportFilter.vertices.Count - order[i]);
				}
				
				if(useDoubleFace)
				{
					for (int i = order.Length - 1; i >= 0; i--) {
						supportFilter.triangles.Add(supportFilter.vertices.Count - order[i]);
					}
				}
			}
			else
			{
				for (int i = 0; i < order.Length; i++) {
					terrainFilter.triangles.Add(terrainFilter.vertices.Count - order[i]);
				}
				
				if(useDoubleFace)
				{
					for (int i = order.Length - 1; i >= 0; i--) {
						terrainFilter.triangles.Add(terrainFilter.vertices.Count - order[i]);
					}
				}
			}
		}

		public void AddVertice(Vector3 vertice)
		{
			if(useTransparentTexture)
			{
				supportFilter.vertices.Add(vertice);
			}
			else
			{
				terrainFilter.vertices.Add(vertice);
			}

		}

		public void AddColVertice(Vector3 vertice)
		{
			if(useRenderDataForCol)
			{
				terrainCollider.colVertices.Add(vertice);
			}
			else if(useSupportDataForCol)
			{
				supportCollider.colVertices.Add(vertice);
			}
		}

		public void AddColQuadTriangles(bool face)
		{
			if(face)
			{
				AddColQuadTrianglesOrder(triangleForward);
			}
			else
			{
				AddColQuadTrianglesOrder(triangleBackward);
			}

		}

		public void AddColTriangle(int triangle)
		{
			if(useRenderDataForCol)
			{
				terrainCollider.colTriangles.Add(triangle);
			}
			else if(useSupportDataForCol)
			{
				supportCollider.colTriangles.Add(triangle);
			}
		}

		private void AddColQuadTrianglesOrder(int[] order)
		{
			if(useRenderDataForCol)
			{
				for (int i = 0; i < order.Length; i++) {
					terrainCollider.colTriangles.Add(terrainCollider.colVertices.Count - order[i]);
				}
			}
			else if(useSupportDataForCol)
			{
				for (int i = 0; i < order.Length; i++) {
					supportCollider.colTriangles.Add(supportCollider.colVertices.Count - order[i]);
				}
			}
		}

		public void Clear()
		{
			terrainFilter.Clear();
			terrainCollider.Clear();
			supportFilter.Clear();
			supportCollider.Clear();
		}
	}

	public class FilterMeshData
	{
		public List<Vector3> vertices;
		public List<int> triangles;

		public List<Vector2> uv;

		public List<Color> colors;

		public FilterMeshData()
		{
			vertices = new List<Vector3>(10000);
			triangles = new List<int>(10000);

			uv = new List<Vector2>(10000);

			colors = new List<Color>(10000);
		}

		public void Clear()
		{
			int i;
			for (i = 0; i < vertices.Count; i++) {
				MeshBaseDataCache.Instance.SaveCacheVector3(vertices[i]);
			}
			for (i = 0; i < uv.Count; i++) {
				MeshBaseDataCache.Instance.SaveCacheVector2(uv[i]);
			}
			for (i = 0; i < colors.Count; i++) {
				MeshBaseDataCache.Instance.SaveCacheColor(colors[i]);
			}

			vertices.Clear();
			triangles.Clear();

			uv.Clear();
			colors.Clear();
		}
	}

	public class ColliderMeshData
	{

		public List<Vector3> colVertices;
		public List<int> colTriangles;
		public List<Vector3> boxCenters;
		public List<Vector3> boxSizes;

		public ColliderMeshData()
		{
			colVertices = new List<Vector3>(10000);
			colTriangles = new List<int>(10000);

			boxCenters = new List<Vector3>();
			boxSizes = new List<Vector3>();
		}

		public void Clear()
		{
			for (int i = 0; i < colVertices.Count; i++) {
				MeshBaseDataCache.Instance.SaveCacheVector3(colVertices[i]);
			}
			colVertices.Clear();
			colTriangles.Clear();

			boxCenters.Clear();
			boxSizes.Clear();
		}

		public void AddBox(Vector3 center,Vector3 size)
		{
			boxCenters.Add(center);
			boxSizes.Add(size);
		}
	}

	public enum MeshColliderType
	{
		none,
		terrainCollider,
		supportCollider
	}
	//只在渲染线程中使用
	public class MeshBaseDataCache
	{
		private static MeshBaseDataCache _instance;
		public static MeshBaseDataCache Instance{get{
				if(_instance == null)_instance = new MeshBaseDataCache();
				return _instance;
			}}

		private Queue<Vector2> _queueVector2;
		private Queue<Vector3> _queueVector3;
		private Queue<Color> _queueColor;
		public MeshBaseDataCache()
		{
			_queueVector2 = new Queue<Vector2>(10000);
			_queueVector3 = new Queue<Vector3>(10000);
			_queueColor = new Queue<Color>(10000);
		}

		public void Init()
		{
			for (int i = 0; i < 10000; i++) {
				_queueVector2.Enqueue(new Vector2());
				_queueVector3.Enqueue(new Vector3());
				_queueColor.Enqueue(new Color());
			}
		}

		public void SaveCacheVector2(Vector2 v)
		{
			_queueVector2.Enqueue(v);
		}
		public void SaveCacheVector3(Vector3 v)
		{
			_queueVector3.Enqueue(v);
		}
		public void SaveCacheColor(Color c)
		{
			_queueColor.Enqueue(c);
		}

		public Vector2 GetVector2(float x,float y)
		{
			Vector2 result;
			if(_queueVector2.Count > 0)
			{
				result = _queueVector2.Dequeue();
				result.x = x;
				result.y = y;
			}
			else
			{
				result = new Vector2(x,y);
			}
			return result;
		}

		public Vector3 GetVector3(float x,float y,float z)
		{
			Vector3 result;
			if(_queueVector3.Count > 0)
			{
				result = _queueVector3.Dequeue();
				result.x = x;
				result.y = y;
				result.z = z;
			}
			else
			{
				result = new Vector3(x,y,z);
			}
			return result;
		}

		public Color GetColor(float r,float g,float b,float a)
		{
			Color result;
			if(_queueColor.Count > 0)
			{
				result = _queueColor.Dequeue();
				result.r = r;
				result.g = g;
				result.b = b;
				result.a = a;
			}else
			{
				result = new Color(r,g,b,a);
			}
			return result;
		}
	}
}
