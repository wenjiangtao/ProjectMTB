using System;
using UnityEngine;
namespace MTB
{
	public interface IWorldLoader : IDisposable
	{
		void LoadFirst(Vector3 pos,int size);
		void Update();
		void Stop();
		void Start();
		void SaveAll();
        void updateArea(RefreshChunkArea area);
	}
}

