using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace MTB
{
    public enum ClientHandlerType
    {
        Move = 1,
        Jump = 2,
        Action = 3,
        Ext = 4
    }

    public class SocketClientHandler : Singleton<SocketClientHandler>
    {
        private static Dictionary<ClientHandlerType, BaseHandler> handlers = new Dictionary<ClientHandlerType, BaseHandler>() { 
            {ClientHandlerType.Move,new MoveHandler()},
            {ClientHandlerType.Jump,new JumpHandler()},
            {ClientHandlerType.Action,new ActionHandler()},
            {ClientHandlerType.Ext,new ExtHandler()}
        };

        public void InitHandler(GameObject obj) { 
        }

        public void Handler(NetPackage package)
        {
            handlers[(ClientHandlerType)Enum.Parse(typeof(ClientHandlerType), package.GetId().ToString())].Handler(package);
        }
    }
}
