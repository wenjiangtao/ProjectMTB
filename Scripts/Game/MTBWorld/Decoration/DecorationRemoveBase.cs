using UnityEngine;
using System.Collections.Generic;
namespace MTB
{
    public class DecorationRemoveBase
    {
        private bool _isGrow = false;
        private List<Vector3> decorationBlockList;
        protected DecorationType _decorationType;

        public bool isGrow
        {
            get
            {
                return _isGrow;
            }
            set
            {
                _isGrow = value;
                if (_isGrow && decorationBlockList == null)
                {
                    decorationBlockList = new List<Vector3>();
                }
            }
        }

        protected void setBlock(Chunk chunk, int x, int y, int z, Block block, bool isInrange = false)
        {
            chunk.SetBlock(x, y, z, block, isInrange);
            if (isGrow)
            {
                decorationBlockList.Add(new Vector3(x, y, z));
            }
        }

        public void removeDecoration(Chunk chunk)
        {
            if (isGrow)
            {
                int length = decorationBlockList.ToArray().Length;
                for (int i = 0; i < length; i++)
                {
                    chunk.SetBlock((int)decorationBlockList[i].x, (int)decorationBlockList[i].y, (int)decorationBlockList[i].z, new Block(BlockType.Air));
                }
            }
        }

        public void discard()
        {
            decorationBlockList = null;
        }
    }
}
