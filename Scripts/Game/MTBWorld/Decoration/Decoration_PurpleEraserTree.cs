using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
    public class Decoration_PurpleEraserTree : DecorationRemoveBase, IDecoration
    {
        #region IDecoration implementation

        public bool Decorade(Chunk chunk, int x, int y, int z, IMTBRandom random)
        {
            corners.Clear();
            MTBPlantData data = MTBPlantDataManager.Instance.getData((int)_decorationType);
			int[] trunktemp = data.leafWidth;
            int treeCrownWidth = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            trunktemp = data.chunkHeight;
            int treeHeight = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            trunktemp = data.leafWidth;
            int treeCrownDepth = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            trunktemp = data.leafHeight;
            int treeCrownHeight = trunktemp.Length > 1 ? random.Range(trunktemp[0], trunktemp[1]) : trunktemp[0];
            int startLeaveHeight = data.leafOffset;

            if (!CheckCanGenerator(x, y, z, chunk, treeHeight, treeCrownWidth, treeCrownDepth, startLeaveHeight))
            {
                return false;
            }
            CreateTree(x, y, z, chunk, treeHeight, treeCrownWidth, treeCrownHeight, treeCrownDepth, startLeaveHeight, random);
            return true;
        }

        #endregion

        private List<int> corners;
        public Decoration_PurpleEraserTree()
        {
            corners = new List<int>();
            _decorationType = DecorationType.PurpleEraserTree;
        }

        private bool CheckCanGenerator(int x, int y, int z, Chunk chunk, int treeHeight, int treeCrownWidth, int treeCrownDepth, int startLeaveHeight)
        {
            for (int cy = y; cy < y + treeHeight; cy++)
            {
                if (cy < startLeaveHeight)
                {
                    if (chunk.GetBlock(x, cy, z).BlockType != BlockType.Air)
                    {
                        return false;
                    }
                    continue;
                }
                for (int cx = x - treeCrownWidth; cx <= x + treeCrownWidth; cx++)
                {
                    for (int cz = z - treeCrownDepth; cz <= z + treeCrownDepth; cz++)
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

        private void CreateTree(int x, int y, int z, Chunk chunk, int treeHeight, int treeCrownWidth, int treeCrownHeight, int treeCrownDepth, int startLeaveHeight, IMTBRandom random)
        {
            int crownHeight = y + treeHeight - treeCrownHeight;
            int endLeaveHeight = y + crownHeight - startLeaveHeight;
            int realHeight = y + treeHeight;

            for (int cy = y; cy < realHeight; cy++)
            {
                if (cy < crownHeight)
                {
                    setBlock(chunk, x, cy, z, new Block(BlockType.Block_40));
                    if (cy <= endLeaveHeight && cy >= startLeaveHeight)
                    {
                        setBlock(chunk, x, cy, z, new Block(BlockType.Block_40));
                        if (random.Range(0, 3) < 1)
                        {
                            int pos = random.Range(0, 4);
                            switch (pos)
                            {
                                case 0:
                                    setBlock(chunk, x + 1, cy, z, new Block(BlockType.Block_41));
                                    break;
                                case 1:
                                    setBlock(chunk, x - 1, cy, z, new Block(BlockType.Block_41));
                                    break;
                                case 2:
                                    setBlock(chunk, x, cy, z + 1, new Block(BlockType.Block_41));
                                    break;
                                default:
                                    setBlock(chunk, x, cy, z - 1, new Block(BlockType.Block_41));
                                    break;
                            }
                        }
                    }
                    continue;
                }
                for (int cx = x - treeCrownWidth; cx <= x + treeCrownWidth; cx++)
                {
                    for (int cz = z - treeCrownDepth; cz <= z + treeCrownDepth; cz++)
                    {
                        if (cx == x && cz == z && cy < realHeight - 1)
                        {
                            setBlock(chunk, cx, cy, cz, new Block(BlockType.Block_40));
                            continue;
                        }
                        setBlock(chunk, cx, cy, cz, new Block(BlockType.Block_41));
                    }
                }
            }

            int unfilledCornerNum = random.Range(2, 8);

            int halfCrownHeight = Mathf.FloorToInt(treeCrownHeight / 2);
            for (int i = 0; i < unfilledCornerNum; i++)
            {
                int corner = random.Range(0, 8);
                if (!corners.Contains(corner))
                {
                    corners.Add(corner);
                    int cornerWidth = random.Range(1, treeCrownWidth + 1) - 1;
                    int cornerHeight = random.Range(1, halfCrownHeight);
                    int cornerDepth = random.Range(1, treeCrownDepth + 1) - 1;
                    switch (corner)
                    {
                        case 0:
                            for (int cx = x - treeCrownWidth; cx <= x - treeCrownWidth + cornerWidth; cx++)
                            {
                                for (int cz = z + treeCrownDepth - cornerDepth; cz <= z + treeCrownDepth; cz++)
                                {
                                    for (int cy = crownHeight + treeCrownHeight - cornerHeight; cy <= crownHeight + treeCrownHeight; cy++)
                                    {
                                        setBlock(chunk, cx, cy, cz, new Block(BlockType.Air));
                                    }
                                }
                            }
                            break;
                        case 1:
                            for (int cx = x + treeCrownWidth - cornerWidth; cx <= x + treeCrownWidth; cx++)
                            {
                                for (int cz = z + treeCrownDepth - cornerDepth; cz <= z + treeCrownDepth; cz++)
                                {
                                    for (int cy = crownHeight + treeCrownHeight - cornerHeight; cy <= crownHeight + treeCrownHeight; cy++)
                                    {
                                        setBlock(chunk, cx, cy, cz, new Block(BlockType.Air));
                                    }
                                }
                            }
                            break;
                        case 2:
                            for (int cx = x + treeCrownWidth - cornerWidth; cx <= x + treeCrownWidth; cx++)
                            {
                                for (int cz = z - treeCrownDepth; cz <= z - treeCrownDepth + cornerDepth; cz++)
                                {
                                    for (int cy = crownHeight + treeCrownHeight - cornerHeight; cy <= crownHeight + treeCrownHeight; cy++)
                                    {
                                        setBlock(chunk, cx, cy, cz, new Block(BlockType.Air));
                                    }
                                }
                            }
                            break;
                        case 3:
                            for (int cx = x - treeCrownWidth; cx <= x - treeCrownWidth + cornerWidth; cx++)
                            {
                                for (int cz = z - treeCrownDepth; cz <= z - treeCrownDepth + cornerDepth; cz++)
                                {
                                    for (int cy = crownHeight + treeCrownHeight - cornerHeight; cy <= crownHeight + treeCrownHeight; cy++)
                                    {
                                        setBlock(chunk, cx, cy, cz, new Block(BlockType.Air));
                                    }
                                }
                            }
                            break;
                        case 4:
                            for (int cx = x - treeCrownWidth; cx <= x - treeCrownWidth + cornerWidth; cx++)
                            {
                                for (int cz = z + treeCrownDepth - cornerDepth; cz <= z + treeCrownDepth; cz++)
                                {
                                    for (int cy = crownHeight; cy <= crownHeight + cornerHeight; cy++)
                                    {
                                        setBlock(chunk, cx, cy, cz, new Block(BlockType.Air));
                                    }
                                }
                            }
                            break;
                        case 5:
                            for (int cx = x + treeCrownWidth - cornerWidth; cx <= x + treeCrownWidth; cx++)
                            {
                                for (int cz = z + treeCrownDepth - cornerDepth; cz <= z + treeCrownDepth; cz++)
                                {
                                    for (int cy = crownHeight; cy <= crownHeight + cornerHeight; cy++)
                                    {
                                        setBlock(chunk, cx, cy, cz, new Block(BlockType.Air));
                                    }
                                }
                            }
                            break;
                        case 6:
                            for (int cx = x + treeCrownWidth - cornerWidth; cx <= x + treeCrownWidth; cx++)
                            {
                                for (int cz = z - treeCrownDepth; cz <= z - treeCrownDepth + cornerDepth; cz++)
                                {
                                    for (int cy = crownHeight; cy <= crownHeight + cornerHeight; cy++)
                                    {
                                        setBlock(chunk, cx, cy, cz, new Block(BlockType.Air));
                                    }
                                }
                            }
                            break;
                        default:
                            for (int cx = x - treeCrownWidth; cx <= x - treeCrownWidth + cornerWidth; cx++)
                            {
                                for (int cz = z - treeCrownDepth; cz <= z - treeCrownDepth + cornerDepth; cz++)
                                {
                                    for (int cy = crownHeight; cy <= crownHeight + cornerHeight; cy++)
                                    {
                                        setBlock(chunk, cx, cy, cz, new Block(BlockType.Air));
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}

