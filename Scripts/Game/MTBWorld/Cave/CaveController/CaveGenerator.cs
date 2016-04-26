using System;
using UnityEngine;

namespace MTB
{
    public class CaveGenerator
    {
        private CavesGen _caveGen;
        private CaveHorizontalGen _caveHorizontalGen;

        public CaveGenerator(int seed)
        {
            _caveGen = new CavesGen(seed);
            _caveHorizontalGen = new CaveHorizontalGen(seed);
        }

        public void generate(Chunk chunk)
        {
            _caveGen.generate(chunk);
            _caveHorizontalGen.generate(chunk);
        }
    }
}
