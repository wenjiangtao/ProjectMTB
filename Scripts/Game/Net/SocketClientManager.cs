using UnityEngine;
using System.Collections;

namespace MTB
{
    public class SocketClientManager : Singleton<SocketClientManager>
    {
        private Client _socketClient;
        private NetPackage _netpackage;
        private JumpCommandPackage _jumpPackage;
        private MoveCommandPackage _movePackage;
        public bool isConnect = false;

        public void InitSockets(GameObject gameobject)
        {
            SocketClientHandler.Instance.InitHandler(gameobject);
            EventManager.RegisterEvent("OnClientConnect", OnConnectSocket);
            _socketClient = gameobject.AddComponent<Client>();
            _socketClient.Init();
        }

        private void OnConnectSocket(params object[] paras)
        {
            isConnect = true;
            _socketClient = (Client)paras[0];
            _movePackage = new MoveCommandPackage();
            _jumpPackage = new JumpCommandPackage();
        }

        public void Move(Vector2 dir)
        {
            _movePackage.uid = 1;
            _movePackage.command = 1;
            _movePackage.dir = dir;
            _socketClient.SendPackage(_movePackage);
        }

        public void Jump(bool b)
        {
            _jumpPackage.uid = 1;
            if (b)
                _jumpPackage.command = 1;
            else
                _jumpPackage.command = 2;
            _socketClient.SendPackage(_jumpPackage);
        }

        public void SendMessages(int uid, byte command, params object[] paras)
        {
        }

        public void SendXtMessages(int uid, byte command, params object[] paras) { }


        //public void 
    }
}
