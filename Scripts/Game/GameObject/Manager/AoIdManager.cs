using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class AoIdManager
    {
        private int _aoId;
        private static AoIdManager _instance;
        public static AoIdManager instance
        {
            get
            {
                if (_instance == null) _instance = new AoIdManager();
                return _instance;
            }
        }

        public AoIdManager()
        {
            _aoId = 0;
        }

        public int getAoId()
        {
            _aoId++;
            return _aoId;
        }

        public void synchronizeAoId(int aoId)
        {
            _aoId = aoId;
        }
    }
}
