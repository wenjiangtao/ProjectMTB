using System;
using System.Collections;
using UnityEngine;
using System.Net;
using System.Net.NetworkInformation;
using MTB;
public class BroadcastService : MonoBehaviour
{
	ConnectionWorker broadcastWorker;
	public int broadcastPort = 9998;
	bool stop = true;
	public bool isBroadcast{get{return !stop;}}
	void Awake()
	{

	}
	
	IEnumerator ListenReceive()
	{
		while(!stop)
		{
			lock(broadcastWorker.readPackageQueue)
			{
				while (broadcastWorker.readPackageQueue.Count > 0)
				{
					NetPackage package = broadcastWorker.readPackageQueue.Dequeue();
					((UdpPackage)package).Do();
				}
			}
			yield return new WaitForSeconds(0.5f);
		}
		yield return null;
	}
	
	public void SendPackage(NetPackage package)
	{
		if(!stop)
		{
			broadcastWorker.SendPackage(package);
		}
	}
	
	public void StartBroadcast()
	{
		if(stop)
		{
			try
			{
			UdpConnection udpConnection = new UdpConnection(broadcastPort);
			broadcastWorker = new ConnectionWorker(udpConnection);
			broadcastWorker.Start();
			stop = false;
			StartCoroutine(ListenReceive());
			}
			catch(Exception e)
			{
				GUITextDebug.debug(e.Message + "\n" +e.StackTrace);
			}
		}
	}

//	private bool IsPortUse(int port)
//	{
//		IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
//		IPEndPoint[] arr = properties.GetActiveUdpListeners();
//		foreach (var item in arr) {
//			Debug.Log("udpPort:" + item.Port);
//			if(item.Port == port)return true;
//		}
//		return false;
//	}
	
	public void StopBroadcast()
	{
		if(!stop)
		{
			try
			{
			broadcastWorker.Stop();
			broadcastWorker.Dispose();
			broadcastWorker = null;
			stop = true;
			}catch(Exception e)
			{
				GUITextDebug.debug(e.Message + "\n" +e.StackTrace);
			}
		}
	}
	
	void OnDestroy()
	{
		StopBroadcast();
	}
}

