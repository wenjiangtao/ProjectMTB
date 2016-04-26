using System;
using System.Collections;
using System.Collections.Generic;
namespace MTB
{
    public class Decoration_FaceTree : DecorationRemoveBase, IDecoration
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
            int treeLeaveWidth = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            trunktemp = data.leafHeight;
            int treeLeaveHeight = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            int treeLeaveOffset = data.leafOffset;

            if (!CheckCanGenerator(x, y, z, chunk, treeTrunkWidth, treeTrunkHeight, treeLeaveWidth, treeLeaveHeight, treeLeaveOffset))
            {
                return false;
            }
            return CreateTree(x, y, z, chunk, treeTrunkWidth, treeTrunkHeight, treeLeaveWidth, treeLeaveHeight, treeLeaveOffset, random);
        }

        #endregion

        private Queue<WorldPos> _spreadQueue;
        public Decoration_FaceTree()
        {
            _spreadQueue = new Queue<WorldPos>();
            _decorationType = DecorationType.FaceTree;
        }

        private bool CheckCanGenerator(int x, int y, int z, Chunk chunk, int treeTrunkWidth, int treeTrunkHeight, int treeLeaveWidth, int treeLeaveHeight, int treeLeaveOffset)
        {
            int treeHeight = treeTrunkHeight + treeLeaveHeight + treeLeaveOffset;
            int treeCrownStartHeight = treeTrunkHeight + treeLeaveOffset;
            int halfTreeLeaveWidth = treeLeaveWidth / 2;
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
                for (int cx = x - halfTreeLeaveWidth; cx <= x + halfTreeLeaveWidth; cx++)
                {
                    for (int cz = z - halfTreeLeaveWidth; cz <= z + halfTreeLeaveWidth; cz++)
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

        private bool CreateTree(int x, int y, int z, Chunk chunk, int treeTrunkWidth, int treeTrunkHeight, int treeLeaveWidth, int treeLeaveHeight, int treeLeaveOffset, IMTBRandom random)
        {
            int halfTreeLeaveWidth = treeLeaveWidth / 2;
            int treeLeaveTopY = treeLeaveHeight + treeLeaveOffset + treeTrunkHeight - 1 + y;
            int treeLeaveStartY = treeLeaveTopY - halfTreeLeaveWidth;
            _spreadQueue.Clear();
            Block curBlock = chunk.GetBlock(x, treeLeaveStartY, z);
            if (curBlock.BlockType == BlockType.Air)
            {
                setBlock(chunk, x, treeLeaveStartY, z, new Block(BlockType.Block_41, 2));
            }
            WorldPos startPos = new WorldPos(x, treeLeaveStartY, z);
            _spreadQueue.Enqueue(startPos);
            while (_spreadQueue.Count > 0)
            {
                WorldPos pos = _spreadQueue.Dequeue();
                TreeLeaveSpread(chunk, pos.x + 1, pos.y, pos.z, _spreadQueue, halfTreeLeaveWidth, treeLeaveTopY, treeLeaveHeight, startPos);
                TreeLeaveSpread(chunk, pos.x - 1, pos.y, pos.z, _spreadQueue, halfTreeLeaveWidth, treeLeaveTopY, treeLeaveHeight, startPos);
                TreeLeaveSpread(chunk, pos.x, pos.y + 1, pos.z, _spreadQueue, halfTreeLeaveWidth, treeLeaveTopY, treeLeaveHeight, startPos);
                TreeLeaveSpread(chunk, pos.x, pos.y - 1, pos.z, _spreadQueue, halfTreeLeaveWidth, treeLeaveTopY, treeLeaveHeight, startPos);
                TreeLeaveSpread(chunk, pos.x, pos.y, pos.z + 1, _spreadQueue, halfTreeLeaveWidth, treeLeaveTopY, treeLeaveHeight, startPos);
                TreeLeaveSpread(chunk, pos.x, pos.y, pos.z - 1, _spreadQueue, halfTreeLeaveWidth, treeLeaveTopY, treeLeaveHeight, startPos);
            }


            bool hasFaceBlock = random.Range(0, 100) < 50;
            int faceBlock = random.Range(0, treeTrunkHeight);
            for (int i = 0; i < treeTrunkHeight; i++)
            {
                if (hasFaceBlock && faceBlock == i)
                {
                    setBlock(chunk, x, y + i, z, new Block(BlockType.Block_75, (byte)random.Range(0, 4)), true);
                }
                else
                {
                    setBlock(chunk, x, y + i, z, new Block(BlockType.Block_19, 2), true);
                }
            }
            return true;
        }

        private void TreeLeaveSpread(Chunk chunk, int x, int y, int z, Queue<WorldPos> queue, int radius, int treeTopY, int treeLeaveHeight, WorldPos startPos)
        {
            if (treeTopY - y < treeLeaveHeight)
            {
                int dis = Math.Abs(x - startPos.x) + Math.Abs(y - startPos.y) + Math.Abs(z - startPos.z);
                Block curBlock = chunk.GetBlock(x, y, z);
                if (curBlock.BlockType == BlockType.Air)
                {
                    setBlock(chunk, x, y, z, new Block(BlockType.Block_41, 2));
                }
                else
                {
                    return;
                }
                if (dis >= radius) return;
                queue.Enqueue(new WorldPos(x, y, z));
            }
        }
    }
}

