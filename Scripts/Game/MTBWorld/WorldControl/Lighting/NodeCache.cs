using System;
using System.Collections.Generic;
namespace MTB
{
	public class NodeCache
	{
		private static NodeCache _instance;
		public static NodeCache Instance{get{
				if(_instance == null)_instance = new NodeCache();
				return _instance;
			}}

		private Queue<LightShrinkNode> shrinkNodes;
		private Queue<LightSpreadNode> spreadNodes;
		private static object _lockObj = new object();
		public NodeCache ()
		{
			shrinkNodes = new Queue<LightShrinkNode>(50);
			spreadNodes = new Queue<LightSpreadNode>(1000);
		}

		public LightShrinkNode GetShrinkNode(int index,int prevLightLevel,int lightLevel,Chunk chunk)
		{
			LightShrinkNode node;
			lock(_lockObj)
			{
				if(shrinkNodes.Count > 0)
				{
					node = shrinkNodes.Dequeue();
					node.index = index;
					node.prevLightLevel = prevLightLevel;
					node.lightLevel = lightLevel;
					node.chunk = chunk;
				}
				else
				{
					node = new LightShrinkNode(index,prevLightLevel,lightLevel,chunk);
				}
			}
			return node;
		}

		public LightSpreadNode GetSpreadNode(int index,int level,Chunk chunk)
		{
			LightSpreadNode node;
			lock(_lockObj)
			{
				if(spreadNodes.Count > 0)
				{
					node = spreadNodes.Dequeue();
					node.index = index;
					node.lightLevel = level;
					node.chunk = chunk;
				}
				else
				{
					node = new LightSpreadNode(index,level,chunk);
				}
			}
			return node;
		}

		public int maxShrink = 0;
		public void SaveShrinkNode(LightShrinkNode node)
		{
			lock(_lockObj)
			{
				node.index = 0;
				node.prevLightLevel = 0;
				node.lightLevel = 0;
				node.chunk = null;
				shrinkNodes.Enqueue(node);
				if(maxShrink < shrinkNodes.Count)maxShrink = shrinkNodes.Count;
			}
		}

		public int maxSpread = 0;
		public void SaveSpreadNode(LightSpreadNode node)
		{
			lock(_lockObj)
			{
				node.index = 0;
				node.lightLevel = 0;
				node.chunk = null;
				spreadNodes.Enqueue(node);
				if(maxSpread < spreadNodes.Count)maxSpread = spreadNodes.Count;
			}
		}

	}
}

