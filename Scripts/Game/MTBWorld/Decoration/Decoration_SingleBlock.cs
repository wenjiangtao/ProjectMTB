using System;
namespace MTB
{
    public abstract class Decoration_SingleBlock : DecorationRemoveBase, IDecoration
    {
        #region IDecoration implementation

        public bool Decorade(Chunk chunk, int x, int y, int z, IMTBRandom random)
        {
            if (chunk.GetBlock(x, y, z, true).BlockType == BlockType.Air)
            {
                setBlock(chunk, x, y, z, GetDecoration());
                return true;
            }
            return false;
        }

        #endregion

        public Decoration_SingleBlock()
        {
        }

        public abstract Block GetDecoration();
    }
}

