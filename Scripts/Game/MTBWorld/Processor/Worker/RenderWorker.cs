using System;
using System.Collections.Generic;
namespace MTB
{
	public class RenderWorker : ProcessWorker
	{
		#region implemented abstract members of ProcessWorker

		protected override void Process ()
		{
			try{
				PrevRenderChunk prevChunk = DequeuePrevRender();
				if(prevChunk != null)
				{
					WorldPos pos = prevChunk.chunk.worldPos;
					if(World.world.GetChunk(pos.x,pos.y,pos.z) == null)return;
					List<Chunk> renderChunks = _prevRenderHandler.HandlePrevRenderChunk(prevChunk);
					for (int i = 0; i < renderChunks.Count; i++) {
						EnqueueRender(renderChunks[i],true);
					}
				}
				
				Chunk chunk = DequeueHighRender();
				if(chunk == null)
				{
					chunk = DequeueRender();
				}
				if(chunk != null)
				{
					WorldPos pos = chunk.worldPos;
					if(World.world.GetChunk(pos.x,pos.y,pos.z) == null)return;
					MeshData meshData = _manager.GetUnusedMeshData();
					for (int i = 0; i < chunk.Sections.Length; i++) {
						Section s = chunk.Sections[i];
						_meshCalculator.CalculateSectionMesh(s,meshData);
					}
					_manager.EnqueueWaitRender(chunk,meshData);
				}
			}catch(Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}
		}

		#endregion
		private MeshCalculator _meshCalculator;
		private PrevRenderHandler _prevRenderHandler;

		private Queue<Chunk> _highRenderQueue;
		private Queue<Chunk> _renderQueue;
		private Queue<PrevRenderChunk> _prevRenderQueue;
		public RenderWorker (DataProcessorManager manager)
			:base(manager)
		{
			_meshCalculator = new MeshCalculator();
			_prevRenderHandler = new PrevRenderHandler(manager);

			_renderQueue = new Queue<Chunk>();
			_highRenderQueue = new Queue<Chunk>();
			_prevRenderQueue = new Queue<PrevRenderChunk>();
		}

		public void EnqueuePrevRender(Chunk chunk,int x,int y,int z,Block block)
		{
			lock(_prevRenderQueue)
			{
				PrevRenderChunk newPrevRenderChunk = new PrevRenderChunk(chunk,x,y,z,block);
				IEnumerator<PrevRenderChunk> enumerator = _prevRenderQueue.GetEnumerator();
				bool isContains = false;
				while(enumerator.MoveNext())
				{
					if(enumerator.Current.EqualOther(newPrevRenderChunk))
					{
						isContains = true;
						break;
					}
				}
				if(!isContains)
				{
					_prevRenderQueue.Enqueue(newPrevRenderChunk);
				}
			}
		}

		private PrevRenderChunk DequeuePrevRender()
		{
			lock(_prevRenderQueue)
			{
				if(_prevRenderQueue.Count > 0)
				{
					return _prevRenderQueue.Dequeue();
				}
				return null;
			}
		}

		public void EnqueueRender(Chunk chunk,bool isHighPriority = false)
		{
			if(isHighPriority)
			{
				lock(_highRenderQueue)
				{
					if(!_highRenderQueue.Contains(chunk))
					{
						_highRenderQueue.Enqueue(chunk);
					}
				}
			}
			else
			{
				lock(_renderQueue)
				{
					if(!_renderQueue.Contains(chunk))
					{
						_renderQueue.Enqueue(chunk);
					}
				}
			}
		}
		
		private Chunk DequeueRender()
		{
			lock(_renderQueue)
			{
				if(_renderQueue.Count > 0)
				{
					return _renderQueue.Dequeue();
				}
				return null;
			}
		}
		
		private Chunk DequeueHighRender()
		{
			lock(_highRenderQueue)
			{
				if(_highRenderQueue.Count > 0)
				{
					return _highRenderQueue.Dequeue();
				}
				return null;
			}
		}
	}
}

