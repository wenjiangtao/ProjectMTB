using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
namespace MTB
{
    public class MTBPathFinder : Singleton<MTBPathFinder>
    {
        private Thread _workerThread;
        private World _world;
        private Queue<MTBPath> pathQueue;
        private object pathQueueLock = new object();
        private bool _isDestroy;
		private Queue<PathNode> cache;

        public void Init(World world)
        {
            this._world = world;
            this.pathQueue = new Queue<MTBPath>();
			this.cache = new Queue<PathNode>(500);

            this._workerThread = new Thread(this.WorkerThread);
            this._workerThread.Start();
//			StartCoroutine(WorkerThreadCoroutine());
            _isDestroy = false;
        }

		private PathNode GetNewPathNode(Vector3 position, PathNode owner, int movementCost, Vector3 targetPosition)
		{
			PathNode node;
			if(cache.Count > 0)
			{
				node = cache.Dequeue();
				node.position = position;
				
				node.heuristicCost = (int)(Vector3.Distance(position, targetPosition) * 10);
				node.movementCost = movementCost;
				node.completeCost = node.heuristicCost + node.movementCost;
				node.owner = owner;
				node.nextNode = null;
			}
			else
			{
				node = new PathNode(position,owner,movementCost,targetPosition);
			}
			return node;
		}
		private void SavePathNode(PathNode node)
		{
			if(cache.Count > 500)return;
			cache.Enqueue(node);
		}

        public void Dispose()
        {
            _isDestroy = true;
        }

        public MTBPath GetPath(Vector3 startingPos, Vector3 goalPos, bool needsGround = false)
        {
            MTBPath pathObject = new MTBPath(startingPos, goalPos);
            pathObject.needsGround = needsGround;
            lock (this.pathQueue)
            {
                this.pathQueue.Enqueue(pathObject);
            }
            return pathObject;
        }

		private IEnumerator WorkerThreadCoroutine()
		{
			while (!_isDestroy)
			{
				try
				{
					paths.Clear();
					lock (this.pathQueue)
					{
						while (this.pathQueue.Count > 0)
						{
							paths.Add(this.pathQueue.Dequeue());
						}
					}
					// Do path calculations
					for (int i = 0; i < paths.Count; i++) {
						this.FindPath(paths[i]);
					}
					Thread.Sleep(10);
				}
				catch (Exception e)
				{
					Debug.LogError(e);
				}
				yield return null;
			}
			_world = null;
		}

		private List<MTBPath> paths = new List<MTBPath>();
        private void WorkerThread()
        {
            while (!_isDestroy)
            {
                try
                {
					paths.Clear();
                    lock (this.pathQueue)
                    {
                        while (this.pathQueue.Count > 0)
                        {
                            paths.Add(this.pathQueue.Dequeue());
                        }
                    }
                    // Do path calculations
					for (int i = 0; i < paths.Count; i++) {
						this.FindPath(paths[i]);
					}
                    Thread.Sleep(10);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

            }
            _world = null;
        }

		private Dictionary<Vector3, PathNode> openList = new Dictionary<Vector3, PathNode>(new Vector3Comparer());
		private List<Vector3> closedList = new List<Vector3>();
		private System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

		private Vector3[] positionsOffset = new Vector3[16]
		{
			// Front
			Vector3.forward,
			// Back
			Vector3.back,
			// Left
			Vector3.left,
			// Right
			Vector3.right,
			// Front right
			Vector3.forward + Vector3.right,
			// Front left
			Vector3.forward + Vector3.left,
			// Back right
			Vector3.back + Vector3.right,
			// Back left
			Vector3.back + Vector3.left,
			// Up Front
			Vector3.forward + Vector3.up,
			// Up Back
			Vector3.back + Vector3.up,
			// Up Left
			Vector3.left + Vector3.up,
			// Up Right
			Vector3.right + Vector3.up,
			// Down Front
			Vector3.forward + Vector3.down,
			// Down Back
			Vector3.back + Vector3.down,
			// Down Left
			Vector3.left + Vector3.down,
			// Down Right
			Vector3.right + Vector3.down,
		};

		private Vector3[] positions = new Vector3[16];

		private PathNode[] nodes = new PathNode[16];

		private Queue<PathNode> queuePathNode = new Queue<PathNode>(500);

        private void FindPath(MTBPath path)
        {
			openList.Clear();
			closedList.Clear();
			while(queuePathNode.Count > 0)
			{
				SavePathNode(queuePathNode.Dequeue());
			}
			Vector3 a;
//            PathNode startNode = new PathNode(path.startPos, null, 0, path.goalPos);
			PathNode startNode = GetNewPathNode(path.startPos, null, 0, path.goalPos);
			queuePathNode.Enqueue(startNode);
            startNode.owner = startNode;
            PathNode currentNode = startNode;

            bool pathFound = false;
            bool noPath = false;
            int movementCost = 0;

			stopWatch.Reset();
            stopWatch.Start();
            try
            {
                while (!pathFound && !noPath)
                {
					for (int i = 0; i < positions.Length; i++) {
						positions[i] = currentNode.position + positionsOffset[i];
					}
                    // Analyze surrounding path nodes
//                    PathNode[] nodes = new PathNode[positions.Length];
					for (int i = 0; i < nodes.Length; i++) {
						nodes[i] = null;
					}
                    PathNode lowestCostNode = null;

                    // Check which ones are walkable and add them to the nodes-array
                    for (int i = 0; i < positions.Length; i++)
                    {
                        // Movement cost from this to the surrounding block
                        int currentMovementCost = (int)(Vector3.Distance(positions[i], currentNode.position) * 10);

                        // Check if this node is walkable
                        if (isGenerated((int)positions[i].x, (int)positions[i].y, (int)positions[i].z) &&
						    !ListContains(closedList,positions[i]) && !HasBlockInPos((int)positions[i].x, (int)positions[i].y, (int)positions[i].z) &&
                            // Walkable check
                            (!path.needsGround || HasBlockInPos((int)positions[i].x, (int)positions[i].y - 1, (int)positions[i].z)))
                        {
                            // Add node to the nodes-array
                            if (openList.ContainsKey(positions[i]))
                            {
                                nodes[i] = openList[positions[i]];
                            }
                            else
                            {
//                                nodes[i] = new PathNode(positions[i], currentNode, movementCost + currentMovementCost, path.goalPos);
								nodes[i] = GetNewPathNode(positions[i], currentNode, movementCost + currentMovementCost, path.goalPos);
                                openList.Add(positions[i], nodes[i]);
								queuePathNode.Enqueue(nodes[i]);
                            }

                        }

                        // Check for lowest cost
                        if (nodes[i] != null && (lowestCostNode == null || nodes[i].completeCost < lowestCostNode.completeCost))
                        {
                            lowestCostNode = nodes[i];
                        }
                    }

                    if (lowestCostNode == null)
                    {
                        noPath = true;
                        break;
                    }

                    if (currentNode.position.x == path.goalPos.x && currentNode.position.z == path.goalPos.z)
                        pathFound = true;

                    // Put the lowest cost node on the closed list
                    if (currentNode.owner.position == lowestCostNode.owner.position)
                    {
                        currentNode.owner.nextNode = lowestCostNode;
                    }
                    else
                        currentNode.nextNode = lowestCostNode;

                    closedList.Add(currentNode.position);
                    currentNode = lowestCostNode;
                }
            }
            catch (System.Exception exception)
            {
                noPath = true;
            }
            stopWatch.Stop();

            if (noPath)
            {
                path.SetPathData(null);
            }
            else
            {
                // This is needed because in the closedlist there can be movements which are like
                // front, right
                // this should be done in one step frontright and this gets achieved by generating an array from the path node's linked list.
				pathData.Clear();
                PathNode cNode = startNode;
                while (cNode != null)
                {
                    pathData.Add(cNode.position);
                    cNode = cNode.nextNode;
                }

                path.SetPathData(pathData.ToArray());
            }
            path.runtime = (float)stopWatch.Elapsed.TotalMilliseconds;
        }
		private List<Vector3> pathData = new List<Vector3>();

		private bool ListContains(List<Vector3> list,Vector3 v)
		{
			for (int i = 0; i < list.Count; i++) {
				Vector3 lv = list[i];
				if(lv.x == v.x && lv.y == v.y && lv.z == v.z)return true;
			}
			return false;
		}

        private bool HasBlockInPos(int x, int y, int z)
        {
            return _world.GetBlock(x, y, z).BlockType != BlockType.Air;
        }

        private bool isGenerated(int x, int y, int z)
        {
            return _world.GetChunk(x, y, z).isGenerated;
        }

        public Vector3 getCanWalkPointByXZ(int x, int y, int z, int radius)
        {
            Vector3 result = new Vector3(0, -1000, 0);
            for (int curY = y - radius; curY < y + radius; curY++)
            {
                if (_world.GetBlock(x, curY, z).BlockType != BlockType.Air && _world.GetBlock(x, curY + 1, z).BlockType == BlockType.Air)
                {
                    result.Set(x, curY, z);
                    return result;
                }
            }
            return result;
        }
    }

	public class Vector3Comparer : IEqualityComparer<Vector3>
	{
		#region IEqualityComparer implementation
		bool IEqualityComparer<Vector3>.Equals (Vector3 a, Vector3 b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}
		int IEqualityComparer<Vector3>.GetHashCode (Vector3 obj)
		{
			return (int)(obj.x + obj.y + obj.z);
		}
		#endregion
	}

    public class PathNode
    {
        public Vector3 position;

        public int movementCost;

        public int heuristicCost;

        public int completeCost;

        public PathNode owner;

        public PathNode nextNode;

        public PathNode(Vector3 position, PathNode owner, int movementCost, Vector3 targetPosition)
        {
            this.position = position;

            this.heuristicCost = (int)(Vector3.Distance(position, targetPosition) * 10);
            this.movementCost = movementCost;
            this.completeCost = this.heuristicCost + this.movementCost;

            this.owner = owner;
        }
    }
}

