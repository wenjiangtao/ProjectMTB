using System;
using UnityEngine;
namespace MTB
{
    public class MonsterAttributes : BaseAttributes
    {
        public int monsterId
        {
            get
            {
                return objectId;
            }
            set
            {
                objectId = value;
            }
        }
    }
}

