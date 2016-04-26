using UnityEngine;
using System.Collections;

public class TestArraySpeed : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Test3DArray ();
	}

	void Test2DArray ()
	{
		int leng = 1024;
		int leng2d = leng * leng;
		int [,] array2D = new int[leng, leng];
		int [] array1D = new int[leng2d];
		for (int i =0; i < leng; i++) {
			for (int j = 0; j < leng; j++){
				array2D[i,j] = 1;
			}
	//		array2D[
		}
		for (int k = 0; k < leng2d; k++) {
			array1D[k] = 1;
		}
		float start = Time.realtimeSinceStartup;
		int b = 0;
		for (int i = 0; i < leng; i++) {
			for (int j = 0; j < leng; j++)
			{
				b = array1D[(i<<10) + j];
			}
		}


		float step1 = Time.realtimeSinceStartup;
		for (int i= 0; i < leng; i++)
		for (int j = 0; j < leng; j++) {
			b = array2D[i,j];
		}
		float end = Time.realtimeSinceStartup;
		Debug.Log (string.Format ("1D:{0},2D:{1}", step1 - start, end - step1));
	}

	void Test3DArray ()
	{
		int xLeng = 256;
		int yLeng = 256;
		int zLeng = 256;
		int[,,] array3D = new int[xLeng,yLeng,zLeng];
		int [] array1D  = new int[xLeng * yLeng * zLeng];
		float start = Time.realtimeSinceStartup;
		int b;
		int c;
		for (int i = 0; i < xLeng; i++)
			for (int j = 0; j < yLeng; j++)
				for (int k = 0; k < zLeng; k++)
					b = array3D [i, j, k];
		float step1 = Time.realtimeSinceStartup;
		for (int i = 0; i < xLeng; i++)
			for (int j = 0; j < yLeng; j++)
				for (int k = 0; k < zLeng; k++)
					c = array1D [(k << 16) + (j << 8) + i];
		float end = Time.realtimeSinceStartup;
		Debug.Log (string.Format("3D:{0},1D:{1}", step1 - start, end - step1));

	}
	// Update is called once per frame
	void Update () {
	
	}
}
