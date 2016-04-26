using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MTB;

public class ConnectionWorker
{
    Thread worker;
	public Connection conn{get;private set;}
    byte[] buffer = new byte[4096];
    public Queue<NetPackage> readPackageQueue = new Queue<NetPackage>();
    public Queue<NetPackage> writePackageQueue = new Queue<NetPackage>();
    PackageBuffer readBuffer;
    PackageBuffer writeBuffer;
	bool stop;
    public ConnectionWorker(Connection conn)
    {
        this.conn = conn;
        readBuffer = new PackageBuffer();
        writeBuffer = new PackageBuffer();
		stop = false;
    }

    public void SendPackage(NetPackage package)
    {
		lock(writePackageQueue)
		{
        	writePackageQueue.Enqueue(package);
		}
    }
    public void DoWork()
    {
        while (true)
        {
//			Debug.Log(this is ClientConnectionWorker ? "ServerDetailWorker:work" : "ClientDetailWorker");
            int length = 0;
            //这里要检测是否有一段可读数据的时候只读一部分仍然检测为可读
            if (conn.CanRead())
            {
                length = conn.Read(buffer);
                if (length > 0) 
                    readBuffer.Write(buffer, 0, length);
            }

            if (conn.CanWrite())
            {
                byte[] tempBuffer = writeBuffer.GetDataBuffer();
                int readPos = writeBuffer.GetReadPos();
                length = writeBuffer.GetDataLength();
                if (length > 0)
                {
                    int writeLen = conn.Write(tempBuffer, readPos, length);
                    if (length > 0)
                        writeBuffer.SetReadPos(length, SeekOrigin.Current);
                }
            }

            if (conn.HasError())
            {
                //Debug.Log ("HasError~~~~~~~~~~~");
				stop = true;
                break;
            }

            //从buffer中读package
            while (!stop)
            {
                int dataLength = readBuffer.GetDataLength();
                if (dataLength < 8)
                    break;
                int packageLeng = readBuffer.ReadInt();
                System.Diagnostics.Debug.Assert(packageLeng > 0);
                if (dataLength < packageLeng)
                {
                    readBuffer.SetReadPos(-4, SeekOrigin.Current);
                    break;
                }
                int packageId = readBuffer.ReadInt();
                byte[] packageData = readBuffer.ReadBytes(packageLeng - 8);
				NetPackage package = PackageFactory.GetPackage(packageId);
                System.Diagnostics.Debug.Assert(package != null);
                package.Deserialize(packageData);
				package.DeserializeRemoteEndPoint(conn.RemoteEndPoint);
                lock (readPackageQueue)
                {
                    readPackageQueue.Enqueue(package);
                }
            }
            if (stop)
                break;
            lock (writePackageQueue)
            {
                while (writePackageQueue.Count > 0)
                {
                    NetPackage package = writePackageQueue.Dequeue();
					lock(package)
					{
	                    package.Serialize();
	                    MemoryStream ms = package.GetMemoryStream();
	                    writeBuffer.Write(ms.GetBuffer(), 0, (int)ms.Length);
					}
                }
            }
            Thread.Sleep(1);
        }
    }

    public void Start()
    {
		stop = false;
        worker = new Thread(this.DoWork);
        worker.Start();
		Debug.Log("ConnectionWorker DoWork~~~~~~~~~~~");
    }

    public virtual void Stop()
    {
        stop = true;
		worker.Abort();
		Debug.Log("ConnectionWorker StopWork~~~~~~~~~~~");
    }

	public virtual void Dispose()
	{
		conn.Close();
	}

	public bool HasDisConnection()
	{
		if(stop)return true;
		return false;
	}
}
