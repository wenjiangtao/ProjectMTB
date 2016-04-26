using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
    public class EntityData
    {
        public int type;
        public int id;
        public Vector3 pos;
        public List<int> exData = new List<int>();

        public virtual void Serialize(Stream stream)
        {
            Serialization.WriteIntToStream(stream, type);
            Serialization.WriteIntToStream(stream, id);
            Serialization.WriteIntToStream(stream, (int)pos.x);
            Serialization.WriteIntToStream(stream, (int)pos.y);
            Serialization.WriteIntToStream(stream, (int)pos.z);
            Serialization.WriteIntToStream(stream, exData.Count);
            for (int i = 0; i < exData.Count; i++)
            {
                Serialization.WriteIntToStream(stream, exData[i]);
            }
        }

        public virtual void Deserialize(Stream stream)
        {
            type = Serialization.ReadIntFromStream(stream);
            id = Serialization.ReadIntFromStream(stream);
            pos = new Vector3(Serialization.ReadIntFromStream(stream), Serialization.ReadIntFromStream(stream), Serialization.ReadIntFromStream(stream));
            int count = Serialization.ReadIntFromStream(stream);
            exData.Clear();
            for (int i = 0; i < count; i++)
            {
                exData.Add(Serialization.ReadIntFromStream(stream));
            }
        }
    }

    public class EntityType
    {
        public static int MONSTER = 1;
        public static int PLANT = 2;
        public static int NPC = 3;
    }
}

