using System;
namespace MTB
{
    public class Decoration_CFLMushroom : DecorationRemoveBase, IDecoration
    {
        #region IDecoration implementation

        public bool Decorade(Chunk chunk, int x, int y, int z, IMTBRandom random)
        {
            MTBPlantData data = MTBPlantDataManager.Instance.getData((int)_decorationType);
            int[] trunktemp = data.chunkWidth;
            int treeTrunkWidth = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            trunktemp = data.chunkHeight;
            int treeTrunkHeight = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            trunktemp = data.leafWidth;
            int treeHalfLeaveWidth = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            trunktemp = data.leafHeight;
            int treeLeaveHeight = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            int treeLeaveOffset = data.leafOffset;
            if (!CheckCanGenerator(x, y, z, chunk, treeTrunkWidth, treeTrunkHeight, treeHalfLeaveWidth, treeLeaveHeight, treeLeaveOffset))
            {
                return false;
            }
            return CreateTree(x, y, z, chunk, treeTrunkWidth, treeTrunkHeight, treeHalfLeaveWidth, treeLeaveHeight, treeLeaveOffset);
        }

        #endregion

        public Decoration_CFLMushroom()
        {
            _decorationType = DecorationType.CFLMushroom;
        }

        private bool CheckCanGenerator(int x, int y, int z, Chunk chunk, int treeTrunkWidth, int treeTrunkHeight, int treeHalfLeaveWidth, int treeLeaveHeight, int treeLeaveOffset)
        {
            int treeHeight = treeTrunkHeight + treeLeaveHeight + treeLeaveOffset;
            int treeCrownStartHeight = treeTrunkHeight + treeLeaveOffset;
            for (int cy = y; cy < y + treeHeight; cy++)
            {
                if (cy - y < treeCrownStartHeight)
                {
                    if (chunk.GetBlock(x, cy, z).BlockType != BlockType.Air)
                    {
                        return false;
                    }
                    continue;
                }
                for (int cx = x - treeHalfLeaveWidth; cx <= x + treeHalfLeaveWidth; cx++)
                {
                    for (int cz = z - treeHalfLeaveWidth; cz <= z + treeHalfLeaveWidth; cz++)
                    {
                        if (chunk.GetBlock(cx, cy, cz).BlockType != BlockType.Air)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool CreateTree(int x, int y, int z, Chunk chunk, int treeTrunkWidth, int treeTrunkHeight, int treeHalfLeaveWidth, int treeLeaveHeight, int treeLeaveOffset)
        {
            int treeHeight = y + treeTrunkHeight + treeLeaveHeight + treeLeaveOffset;
            int treeCrownStartHeight = y + treeTrunkHeight + treeLeaveOffset;
            int removeLevel = treeHalfLeaveWidth / 2;
            for (int cy = treeCrownStartHeight; cy < treeHeight; cy++)
            {
                int startX = x - treeHalfLeaveWidth;
                int endX = x + treeHalfLeaveWidth;
                int startZ = z - treeHalfLeaveWidth;
                int endZ = z + treeHalfLeaveWidth;
                for (int cx = startX; cx <= endX; cx++)
                {
                    for (int cz = startZ; cz <= endZ; cz++)
                    {
                        if (Math.Abs(cx - startX) + Math.Abs(cz - startZ) <= removeLevel) continue;
                        if (Math.Abs(cx - startX) + Math.Abs(cz - endZ) <= removeLevel) continue;
                        if (Math.Abs(cx - endX) + Math.Abs(cz - startZ) <= removeLevel) continue;
                        if (Math.Abs(cx - endX) + Math.Abs(cz - endZ) <= removeLevel) continue;
                        setBlock(chunk, cx, cy, cz, new Block(BlockType.Block_41, 6));
                    }
                }
                treeHalfLeaveWidth--;
                if (treeHalfLeaveWidth < 0) break;
            }

            for (int i = 0; i < treeTrunkHeight; i++)
            {
                setBlock(chunk, x, y + i, z, new Block(BlockType.Block_90), true);
            }
            return true;
        }
    }
}

