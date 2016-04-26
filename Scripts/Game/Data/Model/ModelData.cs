using System;
using UnityEngine;
using LitJson;
namespace MTB
{
	public class ModelData
	{
		public readonly Vector3[] vertices;
		public readonly Vector2[] uvs;
		public readonly int[] triangles;
		public ModelData (string text)
		{
			JsonData data = JsonMapper.ToObject(text);
			JsonData verticeArr = data["vertices"];
			vertices = new Vector3[verticeArr.Count / 3];
			for (int i = 0; i < vertices.Length; i++) {
				int index = 3 * i;
				vertices[i] = new Vector3((float)((double)verticeArr[index]),(float)((double)verticeArr[index + 1]),(float)((double)verticeArr[index + 2]));
			}

			JsonData triangleArr = data["triangles"];
			triangles = new int[triangleArr.Count];
			for (int i = 0; i < triangles.Length; i++) {
				triangles[i] = (int)triangleArr[i];
			}

			JsonData uvArr = data["uvs"];
			uvs = new Vector2[uvArr.Count / 2];
			for (int i = 0; i < uvs.Length; i++) {
				int index = 2 * i;
				uvs[i] = new Vector2((float)((double)uvArr[index]),(float)((double)uvArr[index + 1]));
			}
		}
	}
}

