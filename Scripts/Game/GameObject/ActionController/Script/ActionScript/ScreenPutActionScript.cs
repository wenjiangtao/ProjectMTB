using System;
using UnityEngine;
namespace MTB
{
    public class ScreenPutActionScript : BaseActionScript
    {
        private GOPlayerController _playerController;

        private int oppoMaskLayer;
        private float distance;

        public ScreenPutActionScript(GameObjectController gameObjectController)
            : base(gameObjectController)
        {
            _playerController = gameObjectController as GOPlayerController;
        }

        public override void SetParam(System.Collections.Generic.Dictionary<string, string> param)
        {
            string oppoMaskLayerStr = "";
            param.TryGetValue("oppoMaskLayer", out oppoMaskLayerStr);
            oppoMaskLayer = ~0;
            if (oppoMaskLayerStr != "")
            {
                oppoMaskLayerStr = oppoMaskLayerStr + "|UI";
                string[] layerNames = oppoMaskLayerStr.Split('|');
                oppoMaskLayer = ~LayerMask.GetMask(layerNames);
            }

            distance = Convert.ToSingle(param["distance"]);
        }

        public override void ActionIn()
        {
            float screenX = _playerController.playerInputState.X;
            float screenY = _playerController.playerInputState.Y;
            BlockMaskController.Instance.Do(screenX, screenY, _playerController.transform.position, distance);
            RaycastHit hit;
            int handMaterialId = _playerController.playerAttribute.handMaterialId;
            if (handMaterialId <= 0) return;
            Item item = ItemManager.Instance.GetItem(handMaterialId);
            if (item.itemType != (int)ItemType.Block) return;
            if (Terrain.RayToWorld(screenX, screenY, _playerController.transform.position,
                                  distance, out hit, oppoMaskLayer))
            {
                WorldPos pos = Terrain.GetWorldPos(hit, true);
                if (hit.collider.GetComponentInParent<ChunkObj>() == null) return;
                if (!Terrain.CheckPosCanPlaced(_playerController.transform, pos)) return;
                Block handBlock = new Block((BlockType)item.sceneBlockType, item.sceneBlockExtendId);

                BlockAttributeCalculator calculator = BlockAttributeCalculatorFactory.GetCalculator(handBlock.BlockType);
                if (calculator is BAC_ModelBlock)
                {
                    Vector3 forward = _gameObjectController.transform.forward;
                    float degree = Vector2.Angle(Vector2.right, new Vector2(forward.x, forward.z));
                    byte extendId = 8;
                    if (degree < 45) extendId |= 3;
                    else if (degree > 135) extendId |= 1;
                    else if (forward.z > 0) extendId |= 2;
                    handBlock = new Block(BlockType.Block_100, extendId);
                }

                int decType = HasActionObjectManager.Instance.plantManager.checkIsPlantSeedling(handMaterialId);
                if (decType != -1)
                {
                    HasActionObjectManager.Instance.plantManager.buildPlant(new Vector3(pos.x, pos.y, pos.z), (DecorationType)decType);
                    return;
                }

                Terrain.SetBlock(pos, handBlock);
            }
        }

        public override void ActionOut()
        {
            BlockMaskController.Instance.StopDo();
        }
    }
}

