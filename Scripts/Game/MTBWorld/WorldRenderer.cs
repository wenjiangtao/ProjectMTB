using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
	public class WorldRenderer : MonoBehaviour
	{
//		public int MaxUnloadChunkNum = 10;
		private World _world;
		private bool _render = true;
		private bool _destroy = true;
		private bool _isRendering = false;
		public void Awake()
		{
			_world = GetComponent<World>();
			StartCoroutine(Render());
			StartCoroutine(Destroy());
		}

		private IEnumerator Destroy()
		{
			while(_destroy)
			{
				if(!_isRendering)
				{
					Chunk chunk = _world.WorldGenerator.DataProcessorManager.DequeueWaitRemove();
					if(chunk != null)
					{
						_world.DisposeChunk(chunk);
					}
				}
				yield return null;
			}
		}

		private IEnumerator Render()
		{
			while(_render)
			{
				WaitRenderChunkJob job = _world.WorldGenerator.DataProcessorManager.DequeueWaitRender();
				if(job != null)
				{
					_isRendering = true;
					EventManager.SendEvent(EventMacro.CHUNK_RENDER_START,job.chunk);
					RenderChunkMesh(job.chunk,job.meshData);
					if(!job.chunk.isGenerated)
					{
						job.chunk.isGenerated = true;
						EventManager.SendEvent(EventMacro.CHUNK_GENERATE_FINISH,job.chunk);
					}
				}
				else
				{
					_isRendering = false;
				}
				yield return null;
			}

		}

		void OnDestroy()
		{
			_render = false;
			_destroy = false;
		}

		public void RenderChunkMesh(Chunk chunk,MeshData meshData)
		{
			if(chunk.chunkObj == null)
			{
				_world.InstantiateChunkGameObject(chunk);
			}
			chunk.chunkObj.terrainMesh.SetFilterMeshData(meshData.terrainFilter);
			chunk.chunkObj.terrainMesh.SetColliderMeshData(meshData.terrainCollider);
			chunk.chunkObj.supportColliderMesh.SetFilterMeshData(meshData.supportFilter);
			chunk.chunkObj.supportColliderMesh.SetColliderMeshData(meshData.supportCollider);
			_world.WorldGenerator.DataProcessorManager.SetUnusedMeshData(meshData);
		}
	}


}

