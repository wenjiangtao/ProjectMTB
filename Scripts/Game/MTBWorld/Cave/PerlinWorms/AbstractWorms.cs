using System;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using UnityEngine;
namespace MTB
{
    public abstract class AbstractWorms : IWorms
    {
        #region IWorms implementation
        public virtual void Generator(Chunk chunk)
        {
            _path.path = new List<Vector3>();

            if (_preChunk == null || _preChunk != null && _preChunk.worldPos.x != chunk.worldPos.x && _preChunk.worldPos.z != chunk.worldPos.z)
            {
                _preChunk = chunk;
                if (checkEmptyChunk(chunk.worldPos.x, chunk.worldPos.z))
                    return;

                GeneratorWormPath(chunk);

                for (int i = _path.path.Count - 1; i >= 0; i--)
                {
                    Vector3 point = _path.path[i];
                    generatorWormBlocks(chunk, point);
                    _path.path.Remove(point);
                }
            }
        }

        public virtual void checkValue(Chunk chunk)
        {
        }

        protected virtual void GeneratorWormPath(Chunk chunk)
        {
            Vector3 Points;
            float px = 0;
            float py = 0;
            float pz = Chunk.chunkDepth / 2;
            float levelDir;
            float frontDir;
            float verticalDir;

            for (int i = 0; i < _lenght; i++)
            {
                py = getHeightValue(chunk.worldPos.x + px, chunk.worldPos.z + pz);

                px = px < 0 ? -1 * px : (px >= Chunk.chunkWidth ? Chunk.chunkWidth * 2 - px : px);
                pz = pz < 0 ? -1 * pz : (pz >= Chunk.chunkDepth ? Chunk.chunkDepth * 2 - pz : pz);
                Points = new Vector3(px, py, pz);
                _path.path.Add(Points);

                levelDir = 200 * (float)_directGenerator.GetValue(chunk.worldPos.x, (_directLeveScanningLine + i), chunk.worldPos.z);
                frontDir = 200 * (float)_directGenerator.GetValue(chunk.worldPos.x, (_directFrontScanningLine + i), chunk.worldPos.z);
                verticalDir = 200 * (float)_directGenerator.GetValue(chunk.worldPos.x, (_directVerticalScanningLine + i), chunk.worldPos.z);
                py += verticalDir > _dirThreshold - _thresholdOffsetVerticalDir ? _limitHeight : -1 * _limitHeight;
                px += frontDir > _dirThreshold - _thresholdOffsetFront ? _limitWidth : -1 * _limitWidth;
                pz += levelDir > _dirThreshold - _thresholdOffsetLevel ? _limitWidth : -1 * _limitWidth;
            }
        }

        protected virtual void generatorWormBlocks(Chunk chunk, Vector3 point)
        {
            int x = Int32.Parse(point.x.ToString());
            int y = Int32.Parse(point.y.ToString());
            int z = Int32.Parse(point.z.ToString());
            int tx = x - _radiusWidth;
            int ty = y - _radiusHeight;
            int tz = z - _radiusWidth;
            for (tx = x - _radiusWidth; tx <= x + _radiusWidth; tx++)
            {
                if (tx >= 0 && tx < Chunk.chunkWidth)
                {
                    for (tz = z - _radiusWidth; tz <= z + _radiusWidth; tz++)
                    {
                        if (tz >= 0 && tz < Chunk.chunkDepth)
                        {
                            for (ty = y - _radiusHeight; ty <= y + _radiusHeight; ty++)
                            {
                                if (ty >= 0 && ty < Chunk.chunkHeight)
                                {
                                    chunk.SetBlock(tx, ty, tz, new Block(BlockType.Air), true);
                                }
                            }
                        }
                    }
                }
            }

            //墙面不规则处理差值
            float flu, fld, fru, frd, blu, bld, bru, brd;
            float lerp1, lerp2, lerp3;
            flu = 200 * (float)_sectionGenerator.GetValue(x - _radiusWidth, y + _radiusHeight, z + _radiusWidth);
            fld = 200 * (float)_sectionGenerator.GetValue(x - _radiusWidth, y - _radiusHeight, z + _radiusWidth);
            fru = 200 * (float)_sectionGenerator.GetValue(x + _radiusWidth, y + _radiusHeight, z + _radiusWidth);
            frd = 200 * (float)_sectionGenerator.GetValue(x + _radiusWidth, y - _radiusHeight, z + _radiusWidth);
            blu = 200 * (float)_sectionGenerator.GetValue(x - _radiusWidth, y + _radiusHeight, z - _radiusWidth);
            bld = 200 * (float)_sectionGenerator.GetValue(x - _radiusWidth, y - _radiusHeight, z - _radiusWidth);
            bru = 200 * (float)_sectionGenerator.GetValue(x + _radiusWidth, y + _radiusHeight, z - _radiusWidth);
            brd = 200 * (float)_sectionGenerator.GetValue(x + _radiusWidth, y - _radiusHeight, z - _radiusWidth);

            //左侧
            tx = x - _radiusWidth - 1;
            if (tx >= 0 && tx < Chunk.chunkWidth)
            {
                for (tz = z - _radiusWidth - 1; tz <= z + _radiusWidth + 1; tz++)
                {
                    if (tz >= 0 && tz < Chunk.chunkDepth)
                    {
                        for (ty = y - _radiusHeight - 1; ty <= y + _radiusHeight + 1; ty++)
                        {
                            if (ty >= 0 && ty < Chunk.chunkHeight)
                            {
                                lerp1 = Mathf.Lerp(blu, flu, (float)(tz - z + _radiusWidth + 1) / (float)(_radiusWidth * 2 + 2));
                                lerp2 = Mathf.Lerp(bld, fld, (float)(tz - z + _radiusWidth + 1) / (float)(_radiusWidth * 2 + 2));
                                lerp3 = Mathf.Lerp(lerp2, lerp1, (float)(ty - y + _radiusHeight + 1) / (float)(_radiusHeight * 2 + 2));
                                if (lerp3 > _wallThreshold)
                                {
                                    chunk.SetBlock(tx, ty, tz, new Block(BlockType.Air), true);
                                }
                            }
                        }
                    }
                }
            }

            //右侧
            tx = x + _radiusWidth + 1;
            if (tx >= 0 && tx < Chunk.chunkWidth)
            {
                for (tz = z - _radiusWidth - 1; tz <= z + _radiusWidth + 1; tz++)
                {
                    if (tz >= 0 && tz < Chunk.chunkDepth)
                    {
                        for (ty = y - _radiusHeight - 1; ty <= y + _radiusHeight + 1; ty++)
                        {
                            if (ty >= 0 && ty < Chunk.chunkHeight)
                            {
                                lerp1 = Mathf.Lerp(bru, fru, (float)(tz - z + _radiusWidth + 1) / (float)(_radiusWidth * 2 + 2));
                                lerp2 = Mathf.Lerp(brd, frd, (float)(tz - z + _radiusWidth + 1) / (float)(_radiusWidth * 2 + 2));
                                lerp3 = Mathf.Lerp(lerp2, lerp1, (float)(ty - y + _radiusHeight + 1) / (float)(_radiusHeight * 2 + 2));
                                if (lerp3 > _wallThreshold)
                                {
                                    chunk.SetBlock(tx, ty, tz, new Block(BlockType.Air), true);
                                }
                            }
                        }
                    }
                }
            }

            //前侧
            tz = z + _radiusWidth + 1;
            if (tz >= 0 && tz < Chunk.chunkWidth)
            {
                for (tx = x - _radiusWidth - 1; tx <= x + _radiusWidth + 1; tx++)
                {
                    if (tx >= 0 && tx < Chunk.chunkWidth)
                    {
                        for (ty = y - _radiusHeight - 1; ty <= y + _radiusHeight + 1; ty++)
                        {
                            if (ty >= 0 && ty < Chunk.chunkHeight)
                            {
                                lerp1 = Mathf.Lerp(flu, fru, (float)(tx - x + _radiusWidth + 1) / (float)(_radiusWidth * 2 + 2));
                                lerp2 = Mathf.Lerp(fld, frd, (float)(tx - x + _radiusWidth + 1) / (float)(_radiusWidth * 2 + 2));
                                lerp3 = Mathf.Lerp(lerp2, lerp1, (float)(ty - y + _radiusHeight + 1) / (float)(_radiusHeight * 2 + 2));
                                if (lerp3 > _wallThreshold)
                                {
                                    chunk.SetBlock(tx, ty, tz, new Block(BlockType.Air), true);
                                }
                            }
                        }
                    }
                }
            }

            //后侧
            tz = z - _radiusWidth - 1;
            if (tz >= 0 && tz < Chunk.chunkWidth)
            {
                for (tx = x - _radiusWidth - 1; tx <= x + _radiusWidth + 1; tx++)
                {
                    if (tx >= 0 && tx < Chunk.chunkWidth)
                    {
                        for (ty = y - _radiusHeight - 1; ty <= y + _radiusHeight + 1; ty++)
                        {
                            if (ty >= 0 && ty < Chunk.chunkHeight)
                            {
                                lerp1 = Mathf.Lerp(blu, bru, (float)(tx - x + _radiusWidth + 1) / (float)(_radiusWidth * 2 + 2));
                                lerp2 = Mathf.Lerp(bld, brd, (float)(tx - x + _radiusWidth + 1) / (float)(_radiusWidth * 2 + 2));
                                lerp3 = Mathf.Lerp(lerp2, lerp1, (float)(ty - y + _radiusHeight + 1) / (float)(_radiusHeight * 2 + 2));
                                if (lerp3 > _wallThreshold)
                                {
                                    chunk.SetBlock(tx, ty, tz, new Block(BlockType.Air), true);
                                }
                            }
                        }
                    }
                }
            }
        }

        protected virtual int getHeightValue(float x, float z)
        {
            float heightOffset = (float)_heightGenerator.GetValue(x, 0, z);
			int heightOff = Mathf.RoundToInt(Chunk.chunkHeight * heightOffset / 2);
            return heightOff;
        }

        protected virtual Vector3 getWorldPos(WormsPath path, int step)
        {
            Vector3 worldpos = new Vector3(path.path[step].x + path.startPoint.x, path.path[step].y + path.startPoint.y, path.path[step].z + path.startPoint.z);
            return worldpos;
        }

        protected virtual bool checkEmptyChunk(int x, int z)
        {

            if (200 * (float)_directGenerator.GetValue(x, 0, z) > (1.6f + _emptyRateOffset))
            {
                return false;
            }
            return true;
        }

        #endregion

        protected ModuleBase _sectionGenerator;
        protected ModuleBase _directGenerator;
        protected ModuleBase _heightGenerator;
        protected int _seed;
        protected float _dirThreshold;
        protected float _wallThreshold;
        protected float _thresholdOffsetFront;
        protected float _thresholdOffsetLevel;
        protected float _thresholdOffsetVerticalDir;
        protected WormsPath _path;
        //protected WormsPath _outPath;
        protected Chunk _preChunk;
        protected Chunk _centerChunk;
        protected int _directLeveScanningLine = 10;
        protected int _directVerticalScanningLine = 10;
        protected int _directFrontScanningLine = 30;
        protected int _lenght;
        protected int _limitHeight;
        protected int _limitWidth;
        protected int _radiusHeight;
        protected int _radiusWidth;
        protected int _upMixValue;
        protected int _downMixValue;
        protected float _emptyRateOffset;

        public AbstractWorms(int seed)
        {
            _seed = seed;
            _dirThreshold = 1.6f;
            _wallThreshold = 1.65f;
            _thresholdOffsetFront = 0;
            _thresholdOffsetLevel = 0;
            _thresholdOffsetVerticalDir = 0;
            _lenght = 10;
            _limitWidth = 3;
            _limitHeight = 1;
            _emptyRateOffset = 0;
            _path = new WormsPath();
            initDirectionDenerator();
            InitWormsGenerator();
        }

        protected virtual void initDirectionDenerator()
        {
            var perlin2 = new MTBPerlin(_seed);
            perlin2.OctaveCount = 80;
            perlin2.Frequency = 50;
            perlin2.Persistence = 0.1;
            perlin2.Lacunarity = 0.1;
            var direction = new ScaleBias(perlin2.ModuleBase);
            direction.Bias = BlockHeightToFloat(1);
            _directGenerator = direction;
        }

        public static float BlockHeightToFloat(int height)
        {
            float result = 2.0f * height / Chunk.chunkHeight;
            return result;
        }

        public static int FloatToBlockHeight(float height)
        {
            return Mathf.RoundToInt(Chunk.chunkHeight * height / 2);
        }

        protected abstract void InitWormsGenerator();

        public abstract CaveType CaveType { get; }

        public struct WormsPath
        {
            public List<Vector3> path;
            public Vector3 startPoint;
            public WormsPath(int x, Vector3 start)
            {
                path = new List<Vector3>();
                startPoint = start;
            }
        }
    }
}
