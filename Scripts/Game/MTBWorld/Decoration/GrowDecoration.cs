using UnityEngine;
using System;
using System.Collections.Generic;
namespace MTB
{
    public class GrowDecoration
    {
        private GrowDecorationParam _params;
        private IDecoration _curDecoration;
        private float _growNeedTime;
        private Chunk _attachChunk;
        private bool _growMark;

        public GrowDecoration(GrowDecorationParam paras)
        {
            _params = paras;
            _params.growedTime = 0;
            _growMark = true;
            _curDecoration = DecorationFactory.GetDecorationInstance((DecorationType)_params.plantData.decorationType);
            (_curDecoration as DecorationRemoveBase).isGrow = true;
            _attachChunk = World.world.GetChunk((int)_params.pos.x, (int)_params.pos.y, (int)_params.pos.z);
            _curDecoration.Decorade(_attachChunk, (int)_params.loacalPos.x, (int)_params.loacalPos.y, (int)_params.loacalPos.z, _params.random);
            _growNeedTime = _params.plantData.growTime;
        }

        public void updateState()
        {
            if (!_growMark)
                return;
            _params.growedTime++;
            if (_params.growedTime >= _growNeedTime)
            {
                _params.growedTime = 0;
                changeToNextState();
            }
        }

        private void changeToNextState()
        {
            _params.growedTime = 0;
            (_curDecoration as DecorationRemoveBase).removeDecoration(_attachChunk);
            if (_params.plantData.nextId == 0)
            {
                HasActionObjectManager.Instance.plantManager.endGrow(_params.aoId);
                dispose();
                return;
            }
            _params.plantData = MTBPlantDataManager.Instance.getData(_params.plantData.nextId);
            _curDecoration = DecorationFactory.GetDecorationInstance((DecorationType)_params.plantData.decorationType);
            _curDecoration.Decorade(_attachChunk, (int)_params.loacalPos.x, (int)_params.loacalPos.y, (int)_params.loacalPos.z, _params.random);
            _growNeedTime = DayNightTime.Instance.normalTimeConfig.daySeconds * _params.plantData.growTime;
            World.world.CheckAndRecalculateMesh((int)_params.pos.x, (int)_params.pos.y, (int)_params.pos.z, World.world.GetBlock((int)_params.pos.x, (int)_params.pos.y, (int)_params.pos.z));
        }

        public void dispose()
        {
            (_curDecoration as DecorationRemoveBase).isGrow = false;
            _params = null;
            _curDecoration = null;
            _attachChunk = null;
        }

        public void remove()
        {
            (_curDecoration as DecorationRemoveBase).removeDecoration(_attachChunk);
            dispose();
        }

        public void pauseGrow()
        {
            _growMark = false;
        }

        public void resumeGrow()
        {
            _growMark = true;
        }

        public EntityData GetEntityData()
        {
            EntityData entityData = new EntityData();
            entityData.type = EntityType.PLANT;
            entityData.id = _params.plantData.decorationType;
            entityData.pos = _params.pos;
            entityData.exData.Clear();
            entityData.exData.Add((int)_params.growedTime);
            return entityData;
        }

        public WorldPos ChunkPos { get { return _attachChunk.worldPos; } }

        public int plantAoId { get { return _params.aoId; } }
    }

    public class GrowDecorationParam
    {
        private MTBPlantData _plantData;

        public int aoId { get; set; }

        public float growedTime { get; set; }

        public Vector3 pos { get; set; }

        public Vector3 loacalPos { get; set; }

        public IMTBRandom random { get; set; }

        public MTBPlantData plantData { get { return _plantData; } set { _plantData = value; } }

        public GrowDecorationParam(MTBPlantData data)
        {
            _plantData = data;
        }
    }
}
