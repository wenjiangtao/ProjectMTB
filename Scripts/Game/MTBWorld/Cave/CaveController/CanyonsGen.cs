﻿using System;
using UnityEngine;

namespace MTB
{
    public class CanyonsGen : CaveGenBase
    {

        private const int worldHeightCap = 150;
        private const int CaveLowCap = 0;

        private const int CaveRarity = 10;
        private const int IndividualCaveRarity = 2;

        private const int CaveMaxAltitude = 150;
        private const int CaveMinAltitude = 0;

        private const int CaveFrequency = 100;
        //横向？
        private const int CaveSystemFrequency = 8;
        //直径相关
        private const int CaveSystemPocketChance = 10;
        private const int CaveSystemPocketMaxSize = 5;
        private const int CaveSystemPocketMinSize = 3;

        private Worms_Horizontal _worms;

        private bool _evenCaveDistribution = false;

        public CanyonsGen(int seed)
            : base(seed)
        {
            _worms = new Worms_Horizontal(seed);
        }

        #region CavesGen Generates
        protected void generateLargeCaveNode(int seed, Chunk chunk, double x, double y, double z)
        {
            generateCaveNode(seed, chunk, x, y, z, 1.0F + (float)this._random.NextDouble() * 6.0F, 0.0F, 0.0F, -1, -1, 0.5D);
        }

        protected void generateCaveNode(int seed, Chunk chunk, double x, double y, double z,
            float paramFloat1, float paramFloat2, float paramFloat3, int angle, int maxAngle, double paramDouble4)
        {

            int real_x = chunk.worldPos.x + Chunk.chunkWidth / 2;
            int real_z = chunk.worldPos.z + Chunk.chunkDepth / 2;

            float f1 = 0.0F;
            float f2 = 0.0F;

            System.Random localRandom = new System.Random(seed);

            if (maxAngle <= 0)
            {
                int checkAreaSize = CaveHorizontallyExtending * 16 - 16;
                maxAngle = checkAreaSize - localRandom.Next(checkAreaSize / 4);
            }
            bool isLargeCave = false;

            if (angle == -1)
            {
                angle = maxAngle / 2;
                isLargeCave = true;
            }
            int j = localRandom.Next(maxAngle / 2) + maxAngle / 4;
            int k = localRandom.Next(6) == 0 ? 1 : 0;

            for (; angle < maxAngle; angle++)
            {
                double d3 = 1.5D + Math.Sin(angle * 3.141593F / maxAngle) * paramFloat1 * 1.0F;
                double d4 = d3 - paramDouble4;

                float f3 = (float)Math.Cos(paramFloat3);
                float f4 = (float)Math.Cos(paramFloat3);
                x += Math.Cos(paramFloat2) * f3;
                y += 0.3F;
                z += Math.Sin(paramFloat2) * f3;

                if (k != 0)
                    paramFloat3 *= 0.92F;
                else
                {
                    paramFloat3 *= 0.7F;
                }
                paramFloat3 += f2 * 0.1F;
                paramFloat2 += f1 * 0.1F;

                f2 *= 0.9F;
                f1 *= 0.75F;
                f2 += (float)(localRandom.NextDouble() - localRandom.NextDouble()) * (float)localRandom.NextDouble() * 2.0F;
                f1 += (float)(localRandom.NextDouble() - localRandom.NextDouble()) * (float)localRandom.NextDouble() * 4.0F;

                if ((!isLargeCave) && (angle == j) && (paramFloat1 > 1.0F) && (maxAngle > 0))
                {

                    generateCaveNode(localRandom.Next(), chunk, x, y, z, (float)localRandom.NextDouble() * 0.5F + 0.5F,
                        paramFloat2 - 1.570796F, paramFloat3 / 3.0F, angle, maxAngle, 1.0D);
                    generateCaveNode(localRandom.Next(), chunk, x, y, z, (float)localRandom.NextDouble() * 0.5F + 0.5F,
                        paramFloat2 + 1.570796F, paramFloat3 / 3.0F, angle, maxAngle, 1.0D);
                    return;
                }

                if ((!isLargeCave) && (localRandom.Next(4) == 0))
                {
                    continue;
                }

                double d5 = x - real_x;
                double d6 = z - real_z;
                double d7 = maxAngle - angle;
                double d8 = paramFloat1 + 2.0F + 16.0F;

                if (d5 * d5 + d6 * d6 - d7 * d7 > d8 * d8)
                {
                    return;
                }

                if ((x < real_x - 16.0D - d3 * 2.0D) || (z < real_z - 16.0D - d3 * 2.0D) || (x > real_x + 16.0D + d3 * 2.0D) || (z > real_z + 16.0D + d3 * 2.0D))
                    continue;

                int m = Int32.Parse(Math.Floor(x - d3).ToString()) - chunk.worldPos.x - 1;
                int n = Int32.Parse(Math.Floor(x + d3).ToString()) - chunk.worldPos.x + 1;

                int i1 = Int32.Parse(Math.Floor(y - d4).ToString()) - 1;
                int i2 = Int32.Parse(Math.Floor(y + d4).ToString()) + 1;

                int i3 = Int32.Parse(Math.Floor(z - d3).ToString()) - chunk.worldPos.z - 1;
                int i4 = Int32.Parse(Math.Floor(z + d3).ToString()) - chunk.worldPos.z + 1;

                m = m < 0 ? 0 : m;
                n = n > 16 ? 16 : n;
                i1 = i1 < 1 ? 1 : i1;
                i2 = i2 > worldHeightCap - 8 ? worldHeightCap - 8 : i2;
                i2 = i2 - i1 < 2 ? (i2 + 2) : i2;
                i3 = i3 < 0 ? 0 : i3;
                i4 = i4 > 16 ? 16 : i4;

                // Generate cave
                for (int local_x = m; local_x < n; local_x++)
                {
                    double d9 = (local_x + chunk.worldPos.x + 0.5D - x) / d3;
                    for (int local_z = i3; local_z < i4; local_z++)
                    {
                        double d10 = (local_z + chunk.worldPos.z + 0.5D - z) / d3;
                        if (d9 * d9 + d10 * d10 < 1.0D)
                        {
                            for (int local_y = i2; local_y > CaveLowCap && local_y > i1; local_y--)
                            {
                                //double d11 = ((local_y - 1) + 0.5D - y) / d4;
                                //if ((d11 > -2.0D) && (d9 * d9 + d11 * d11 + d10 * d10 < 2.5D))
                                //{
                                //可能需要判断下是否可以填充
                                chunk.SetBlock(local_x, local_y, local_z, new Block(BlockType.Air));
                                //}
                            }
                        }
                    }
                }
                if (isLargeCave)
                    break;
            }
        }

        #endregion

        protected override void generateChunk(Vector3 chunkCoord, Chunk chunk)
        {
            //_worms.Generator(chunk);
            int i = this._random.Next(this._random.Next(this._random.Next(CaveRarity) + 1) + 1);
            if (_evenCaveDistribution)
            {
                i = CaveRarity;
            }
            if (this._random.Next(100) >= CaveRarity)
                i = 0;

            for (int j = 0; j < i; j++)
            {
                double x = chunkCoord.x * Chunk.chunkWidth + Chunk.chunkWidth;
                double z = chunkCoord.z * Chunk.chunkDepth + Chunk.chunkDepth;
                double y;
                int count = CaveSystemFrequency;
                bool largeCaveSpawned = false;
                if (_evenCaveDistribution)
                    y = this._random.Next(CaveMaxAltitude - CaveMinAltitude) + CaveMinAltitude;
                else
                    y = this._random.Next(this._random.Next(CaveMaxAltitude - CaveMinAltitude) + 1) + CaveMinAltitude;
                if (this._random.Next(100) <= IndividualCaveRarity)
                {
                    generateLargeCaveNode(this._random.Next(), chunk, x, y, z);
                    largeCaveSpawned = true;
                }
                if ((largeCaveSpawned) || (this._random.Next(100) <= CaveSystemPocketChance - 1))
                {
                    count += this._random.Next(CaveSystemPocketMaxSize - CaveSystemPocketMinSize) + CaveSystemPocketMinSize;
                }
                //count = count > 3 ? 3 : count;
                //count决定每个大节点中有多少个小节点
                while (count > 0)
                {
                    count--;
                    float f1 = (float)this._random.NextDouble() * 3.141593F * 2.0F;
                    float f2 = ((float)this._random.NextDouble() - 0.5F) * 2.0F / 8.0F;
                    float f3 = (float)(this._random.NextDouble() * 2.0F + this._random.NextDouble());

                    //相当于产生洞口
                    generateCaveNode(this._random.Next(), chunk, x, y, z, f3, f1, f2, 0, 0, 1.0D);
                }
            }
        }
    }
}
