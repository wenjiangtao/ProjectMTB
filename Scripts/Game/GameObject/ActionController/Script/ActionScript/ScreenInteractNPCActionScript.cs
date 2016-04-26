using System;
using UnityEngine;
namespace MTB
{
    public class ScreenInteractNPCActionScript : BaseActionScript
    {
        private int maskLayer;
        private float distance;
        private float rayDistance;
        private GOPlayerController _playerController;
        public ScreenInteractNPCActionScript(GameObjectController gameObjectController)
            : base(gameObjectController)
        {
            _playerController = gameObjectController as GOPlayerController;
        }

        public override void SetParam(System.Collections.Generic.Dictionary<string, string> param)
        {
            string maskLayerStr = "";
            param.TryGetValue("maskLayer", out maskLayerStr);
            maskLayer = 0;
            if (maskLayerStr != "")
            {
                string[] layerNames = maskLayerStr.Split('|');
                maskLayer = LayerMask.GetMask(layerNames);
            }

            distance = Convert.ToSingle(param["distance"]);

            rayDistance = param.ContainsKey("rayDistance") ? Convert.ToSingle(param["rayDistance"]) : 2000;
        }

        public override void ActionIn()
        {
            float screenX = _playerController.playerInputState.X;
            float screenY = _playerController.playerInputState.Y;
            RaycastHit hit;
            Ray ray = CameraManager.Instance.CurCamera.followCamera.ScreenPointToRay(new Vector3(screenX, screenY, 0));

            bool IsHit = Physics.Raycast(ray.origin, ray.direction, out hit, rayDistance, maskLayer);
            if (IsHit && Vector3.Distance(_playerController.transform.position, hit.point)
                < distance)
            {
                //GONPCController controller = hit.collider.GetComponent<GONPCController>();
                //if(controller == null)return;
                //controller.onTouch();
                EventManager.SendEvent(PlotEvent.ACTIONFINISH, hit.collider.GetComponent<NPCAttributes>().aoId,hit.collider.GetComponent<NPCAttributes>().taskId,hit.collider.GetComponent<NPCAttributes>().stepId, "ScreenInteractNPCActionScript");
            }
        }
    }
}
