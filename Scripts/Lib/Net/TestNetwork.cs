using UnityEngine;
using System.Collections;
using MTB;

public class TestNetwork : MonoBehaviour
{

    Server server;
    Client client;
    // Use this for initialization
	ConnectionWorker broadcastWorker;
	ConnectionWorker clientWorker;
    void Start()
    {
        SocketClientManager.Instance.InitSockets(gameObject);
        server = gameObject.AddComponent<Server>();
        server.StartAcceptClient();
    }


    void OnDestroy()
    {
        Debug.Log("OnDestroy");
    }
}
