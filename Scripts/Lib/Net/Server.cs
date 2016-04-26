using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using MTB;

public class ServerAcceptWorker
{
    public ServerAcceptWorker(Connection conn)
    {
        this.conn = conn;
    }
    Thread serverAcceptThread;
    Connection conn;
    public Queue<Connection> newConnQueue = new Queue<Connection>();
    public void DoWork()
    {
        while (true)
        {
            if (conn.CanRead())
            {
                Connection newConn = conn.Accept();
                lock (newConnQueue)
                {
                    newConnQueue.Enqueue(newConn);
                }
            }
            Thread.Sleep(1);
        }
    }

    public void Start()
    {
        serverAcceptThread = new Thread(this.DoWork);
        serverAcceptThread.Start();
    }

    public void Stop()
    {
		serverAcceptThread.Abort();
    }

	public void Dispose()
	{
		conn.Close();
	}
}

public class ServerDetailWorker
{

	public List<ClientConnectionWorker> clientConnWorkerList = new List<ClientConnectionWorker>();
    private Thread serverDetailThread;
    public ServerDetailWorker()
    {
    }

    public void Start()
    {
        serverDetailThread = new Thread(this.DoWork);
        serverDetailThread.Start();
    }

    public void DoWork()
    {
        while (true)
        {
            lock (clientConnWorkerList)
            {
				for (int i = clientConnWorkerList.Count - 1; i >= 0; i--) {
					ClientConnectionWorker worker = clientConnWorkerList[i];
					if(worker.HasDisConnection())
					{
						clientConnWorkerList.Remove(worker);
						worker.Stop();
						worker.Dispose();
						continue;
					}
					lock (worker.readPackageQueue)
					{
						while (worker.readPackageQueue.Count > 0)
						{
							NetPackage package = worker.readPackageQueue.Dequeue();
							try{
								((TcpPackage)package).ServerDo(worker);
							}catch(System.Exception e)
							{
								Debug.LogError(e.StackTrace);
							}

						}
					}
				}
            }
            Thread.Sleep(1);
        }
    }

    public void Stop()
    {
		lock (clientConnWorkerList)
		{
			for (int i = clientConnWorkerList.Count - 1; i >= 0; i--) {
				clientConnWorkerList[i].Stop();
				clientConnWorkerList[i].Dispose();
			}
			clientConnWorkerList.Clear();
		}
        serverDetailThread.Abort();
	}

	public void AddConnectionWorker(ClientConnectionWorker worker)
	{
		lock (clientConnWorkerList)
		{
			worker.Start();
			clientConnWorkerList.Add(worker);
		}
	}
}
		
public class Server : MonoBehaviour
{
	public bool stop{get;private set;}
    public int port = 9999;
	private bool _isFirstStartServer = true;
    ServerAcceptWorker serverAcceptWorker;
    ServerDetailWorker serverDetailWorker;
	public float gameTime{get;set;}

	public ClientPlayerManager playerManager{get;private set;}
	public ClientSceneManager sceneManager{get;private set;}
	public ClientEntityManager entityManager{get;private set;}

	void Awake()
	{
		stop = true;
	}

    public void StartAcceptClient()
    {
		if(_isFirstStartServer)
		{
			_isFirstStartServer = false;
			TcpConnection connection = new TcpConnection("0.0.0.0", this.port, true);
			serverAcceptWorker = new ServerAcceptWorker(connection);
			serverDetailWorker = new ServerDetailWorker();
		}
		stop = false;
		playerManager = new ClientPlayerManager();
		sceneManager = new ClientSceneManager();
		entityManager = new ClientEntityManager();
        serverAcceptWorker.Start();
        serverDetailWorker.Start();
        StartCoroutine(HandleNewConn());
        EventManager.SendEvent(NetEventMacro.ON_SERVER_START, "127.0.0.1", this.port);
    }

	public void StopServer()
	{
		stop = true;
		EventManager.SendEvent(NetEventMacro.ON_SERVER_STOP);
		serverAcceptWorker.Stop();
		serverDetailWorker.Stop();
		playerManager.Dispose();
		sceneManager.Dispose();
		entityManager.Dispose();
	}

    void StartClientThread(Connection newConn)
    {
		ClientConnectionWorker worker = new ClientConnectionWorker(newConn);
		serverDetailWorker.AddConnectionWorker(worker);
    }

    IEnumerator HandleNewConn()
    {
		while (!stop)
        {
            lock (serverAcceptWorker.newConnQueue)
            {
                while (serverAcceptWorker.newConnQueue.Count > 0)
                {
                    Connection newConn = serverAcceptWorker.newConnQueue.Dequeue();
                    StartClientThread(newConn);
                }
            }
            yield return null;
        }
    }

    public void BroadcastPackage(NetPackage package)
    {
		playerManager.BroadcastPackage(package);
    }

	public void BroadcastPackage(NetPackage package,ClientPlayer broadcastSource,bool includeSelf = true)
	{
		playerManager.BroadcastPackage(package,broadcastSource,includeSelf);
	}

	private List<WorldPos> chunkPosList = new List<WorldPos>();
	public void BroadcastPackage(NetPackage package,ClientPlayer broadcastSource,int broadcastChunkWidth,bool includeSelf = true)
	{
		WorldPos chunkPos = broadcastSource.inChunkPos;
		for (int x = -broadcastChunkWidth; x <= broadcastChunkWidth; x++) {
			for (int z = -broadcastChunkWidth; z <= broadcastChunkWidth; z++) {
				chunkPosList.Add(new WorldPos(chunkPos.x + x * Chunk.chunkWidth,chunkPos.y,chunkPos.z + z * Chunk.chunkDepth));
			}
		}
		playerManager.BroadcastChunkPackage(package,broadcastSource,chunkPosList,includeSelf);
		chunkPosList.Clear();
	}

//    void OnDestroy()
//    {
//		if(!stop)
//		{
//			StopServer();
//		}
//		if(!_isFirstStartServer)
//		{
//			serverAcceptWorker.Dispose();
//		}
//    }

	public void Dispose()
	{
		if(!stop)
		{
			StopServer();
		}
		if(!_isFirstStartServer)
		{
			serverAcceptWorker.Dispose();
		}
	}
}
