using System;
using System.Collections.Generic;
namespace MTB
{
    class ClientAreaCollection
    {
        public static ClientAreaCollection Instance
        {
            get
            {
                if (_instance == null) _instance = new ClientAreaCollection();
                return _instance;
            }
        }
        private static ClientAreaCollection _instance;

        public RefreshChunkArea _area;
        public bool canCollection { get; private set; }
        public ClientAreaCollection()
        {
            canCollection = false;
        }

        public void BeginCollection()
        {
            canCollection = true;
        }

        public void EndCollection()
        {
            _area = null;
            canCollection = false;
        }

        public void Collection(RefreshChunkArea area)
        {
            if (canCollection)
            {
                _area = area;
            }
        }

        public void SendPackage()
        {
            ChunkAreaChangedPackage package = PackageFactory.GetPackage(PackageType.ChunkAreaChanged) as ChunkAreaChangedPackage;
            package.area = _area;
            NetManager.Instance.client.SendPackage(package);
        }

        public void Clear()
        {
            canCollection = false;
        }
    }
}
