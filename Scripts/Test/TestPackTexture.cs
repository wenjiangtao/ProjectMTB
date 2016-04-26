using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MTB;
public class TestPackTexture : MonoBehaviour {

//    Mesh m;
//    GameObject go;
//    Material mt;
//    public PackTexture packTexture;
//    public Texture2D atlas;
//    GameObject CreateCube(string name)
//    {
//        packTexture = new PackTexture("test");
//        string zName = "grass_side";
//        string xName = "dirt";
//        string yName = "grass_top";
//        string []nameArray = {yName, xName, zName};
//        string basePath = "MTBWorld/Textures/";
//        packTexture.AddTexture(ResourceManager.Instance.LoadAsset<Texture2D>(basePath + zName) as Texture2D, zName);
//        packTexture.AddTexture(ResourceManager.Instance.LoadAsset<Texture2D>(basePath + xName) as Texture2D, xName);
//        packTexture.AddTexture(ResourceManager.Instance.LoadAsset<Texture2D>(basePath + yName) as Texture2D, yName);
//        packTexture.Pack();
//        atlas = packTexture.Texture;
//        Vector2[] uvTemplate = {new Vector2(0f,1f), new Vector2(1f,1f), new Vector2(1f, 0f), new Vector2(0f,0f) };
//        GameObject go = new GameObject(name);
//        MeshFilter mf = go.AddComponent<MeshFilter>();
//        MeshRenderer mr = go.AddComponent<MeshRenderer>();
//        m = new Mesh();
//        Vector3[] vertexTemplate = {new Vector3(0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, -0.5f),new Vector3(0.5f, -0.5f, -0.5f),
//                                    new Vector3(0.5f, 0.5f, 0.5f),new Vector3(-0.5f, 0.5f, 0.5f),new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f)};
//        int[] vertexIdxArray = new int[24];
//        List<Vector3> vertexList = new List<Vector3>();
//        List<int> triangleList = new List<int>();
//        List<Vector2> uvList = new List<Vector2>();
//
//        //填写3角形顶点的原则:从面的外面往里看是顺时针
//        //先加垂直于Y轴的4个三角面，再加垂直于X轴的4个三角面，再加垂直于Z轴的4个三角面
//        //Y轴反方向的2个3角面
//        //0
//        vertexList.Add(vertexTemplate[0]);
//        vertexList.Add(vertexTemplate[1]);
//        vertexList.Add(vertexTemplate[2]);
//        vertexList.Add(vertexTemplate[3]);
//    
//        triangleList.Add(2);
//        triangleList.Add(3);
//        triangleList.Add(0);
//
//        triangleList.Add(2);
//        triangleList.Add(0);
//        triangleList.Add(1);
//
//        
//        //Y轴正方向的2个3脚面
//        //4
//        vertexList.Add(vertexTemplate[4]);
//        vertexList.Add(vertexTemplate[7]);
//        //vertexList.Add(vertexTemplate[6]);
//
//        //vertexList.Add(vertexTemplate[4]);
//        vertexList.Add(vertexTemplate[6]);
//        vertexList.Add(vertexTemplate[5]);
//
//        triangleList.Add(4);
//        triangleList.Add(5);
//        triangleList.Add(6);
//
//        triangleList.Add(4);
//        triangleList.Add(6);
//        triangleList.Add(7);
//        
//      //X轴反方向的2个3角面
//      //8
//      vertexList.Add(vertexTemplate[1]);
//      vertexList.Add(vertexTemplate[5]);
//      vertexList.Add(vertexTemplate[6]);
//    
//      //vertexList.Add(vertexTemplate[1]);
//      //vertexList.Add(vertexTemplate[6]);
//      vertexList.Add(vertexTemplate[2]);
//      triangleList.Add(8);
//      triangleList.Add(9);
//      triangleList.Add(10);
//
//      triangleList.Add(8);
//      triangleList.Add(10);
//      triangleList.Add(11);
//    
//      //X轴正方向的2个3角面
//      //12
//      vertexList.Add(vertexTemplate[3]);
//      vertexList.Add(vertexTemplate[7]);
//      vertexList.Add(vertexTemplate[4]);
//      
//      //vertexList.Add(vertexTemplate[3]);
//      vertexList.Add(vertexTemplate[0]);
//      //vertexList.Add(vertexTemplate[4]);
//      triangleList.Add(12);
//      triangleList.Add(13);
//      triangleList.Add(14);
//
//      triangleList.Add(12);
//      triangleList.Add(14);
//      triangleList.Add(15);
//     
//     //Z轴反方向的2个3脚面
//     //16
//     vertexList.Add(vertexTemplate[6]);
//     //vertexList.Add(vertexTemplate[2]);
//     //vertexList.Add(vertexTemplate[7]);
//     
//      vertexList.Add(vertexTemplate[7]);
//      vertexList.Add(vertexTemplate[3]);
//      vertexList.Add(vertexTemplate[2]);
//      triangleList.Add(16);
//      triangleList.Add(17);
//      triangleList.Add(19);
//
//      triangleList.Add(17);
//      triangleList.Add(18);
//      triangleList.Add(19);
//     
//      //Z轴正方向的2个3脚面
//      //20
//      vertexList.Add(vertexTemplate[4]);
//      vertexList.Add(vertexTemplate[5]);
//      vertexList.Add(vertexTemplate[1]);
//
//      vertexList.Add(vertexTemplate[0]);
//      triangleList.Add(20);
//      triangleList.Add(21);
//      triangleList.Add(23);
//
//      triangleList.Add(23);
//      triangleList.Add(21);
//      triangleList.Add(22);
//      
//      
//      for (int i = 0; i < vertexList.Count; i++ )
//      {
//          Vector2 insideUV = uvTemplate[i % 4];
//          Rect baseUV;
//          string textureName = nameArray[i / 8];
//          baseUV = packTexture.GetSubTextureUV(textureName);
//          insideUV.x = baseUV.x + insideUV.x * baseUV.width;
//          insideUV.y = baseUV.y + insideUV.y * baseUV.height;
//          uvList.Add(insideUV);
//      }
//        
//        m.vertices = vertexList.ToArray();
//        m.triangles = triangleList.ToArray();
//        m.uv = uvList.ToArray();
//        mf.mesh = m;
//        return go;
//    }
//
//    void ShowCubeUv()
//    {
//        return;
//        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        Vector2 [] uv = go.GetComponent<MeshFilter>().mesh.uv;
//        foreach (Vector2 e in uv)
//        {
//            Debug.Log(e);
//        }
//    }
//	// Use this for initialization
//	void Start () {
//        go = CreateCube("TestAtlas");
//        mt = ResourceManager.Instance.LoadAsset<Material>("Material/test") as Material;
//        MeshRenderer mr = go.GetComponent<MeshRenderer>();
//        mr.material = mt;
//        mt.mainTexture = atlas;
//        ShowCubeUv();
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
}
