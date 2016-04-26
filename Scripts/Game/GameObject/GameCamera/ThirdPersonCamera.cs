using System;
using UnityEngine;
namespace MTB
{
    public class ThirdPersonCamera : MTBCamera
    {
        #region implemented abstract members of MTBCamera

        public override MTBCameraType CameraType
        {
            get
            {
                return MTBCameraType.Third;
            }
        }

        #endregion

        //主角对象

        private Transform watchPoint;
        
        private Transform curWatchPoint;
        private bool isNealy;

        public Vector3 WatchPointPosition { get { return watchPoint.position; } }
        public Vector3 CameraPosition { get { return followCamera.transform.position; } }
        public float DistanceFromCameraToWatchPoint { get { return Vector3.Distance(CameraPosition, WatchPointPosition); } }

        public float maxCameraDistance = 5f;
        public float minCameraDistance = 1f;
        public float viewCorrect = 0.001f;

        void Start()
        {
            isNealy = false;
			watchPoint = this.transform;
            watchPoint.position = new Vector3(watchPoint.position.x, watchPoint.position.y + 1.5F, watchPoint.position.z);
        }

        protected override void Update()
        {
            Debug.DrawLine(WatchPointPosition, followCamera.transform.position, Color.red);
            float halfFOV = (followCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
            float aspect = followCamera.aspect;
            float height = followCamera.nearClipPlane * Mathf.Tan(halfFOV) + viewCorrect;
            float width = height * aspect;

            Vector3[] startRaycastPoint = new Vector3[4];
            Vector3[] endRaycastPoint = new Vector3[4];

            float raycastDistance = maxCameraDistance - followCamera.nearClipPlane;
            startRaycastPoint[0] = WatchPointPosition - followCamera.transform.right * width;
            startRaycastPoint[0] += followCamera.transform.up * height;

            startRaycastPoint[1] = WatchPointPosition + followCamera.transform.right * width;
            startRaycastPoint[1] += followCamera.transform.up * height;

            startRaycastPoint[2] = WatchPointPosition - followCamera.transform.right * width;
            startRaycastPoint[2] -= followCamera.transform.up * height;


            startRaycastPoint[3] = WatchPointPosition + followCamera.transform.right * width;
            startRaycastPoint[3] -= followCamera.transform.up * height;

            float minDistance = raycastDistance;
            for (int i = 0; i < startRaycastPoint.Length; i++)
            {
                endRaycastPoint[i] = RayCastPoint(startRaycastPoint[i], raycastDistance);
                float hitDistance = Vector3.Distance(startRaycastPoint[i], endRaycastPoint[i]);
                if (hitDistance < minDistance)
                {
                    minDistance = hitDistance;
                }
            }
            float cameraDistance = minDistance + followCamera.nearClipPlane;
            if (cameraDistance < minCameraDistance) cameraDistance = 0;

            //条件再改
            curWatchPoint = watchPoint;

            if (HeadPointDistance(mainPlayer.position) && !isNealy)//
            {
                curWatchPoint.position = new Vector3(watchPoint.position.x, watchPoint.position.y - 1.5F, watchPoint.position.z);
                isNealy = true;
            }
            else if (!HeadPointDistance(mainPlayer.position) && isNealy)
            {
                curWatchPoint.position = new Vector3(watchPoint.position.x, watchPoint.position.y + 1.5F, watchPoint.position.z);
                isNealy = false;
            }

            watchPoint.eulerAngles = new Vector3(curWatchPoint.eulerAngles.x, mainPlayer.eulerAngles.y, curWatchPoint.eulerAngles.z);
            followCamera.transform.forward = curWatchPoint.forward;

            followCamera.transform.position = curWatchPoint.position - followCamera.transform.forward * cameraDistance;
            // 让射线机永远看着主角！！

            followCamera.transform.LookAt(curWatchPoint);
			base.Update();
        }

        private Vector3 RayCastPoint(Vector3 point, float distance)
        {
            Vector3 result;
            RaycastHit hitInfo;
//            //屏蔽人物
//            int playerLayer = LayerMask.NameToLayer("Player");
//            int maskLayer = ~(1 << playerLayer);
//			int monsterLayer = LayerMask.NameToLayer("Monster");
//			maskLayer &= ~(1 << monsterLayer);
			int maskLayer = (1 << LayerMask.NameToLayer("TerrainMesh"));
            if (Physics.Raycast(point, -followCamera.transform.forward, out hitInfo, distance, maskLayer))
            {
                result = hitInfo.point;
            }
            else
            {
                result = point - followCamera.transform.forward * distance;
            }
            return result;
        }

        private bool HeadPointDistance(Vector3 point)
        {
            RaycastHit hitInfo;
            int playerLayer = LayerMask.NameToLayer("Player");
            int maskLayer = ~(1 << playerLayer);
			int monsterLayer = LayerMask.NameToLayer("Monster");
			maskLayer &= ~(1 << monsterLayer);
            if (Physics.Raycast(point, new Vector3(0, 1f, 0), out hitInfo, 2F, maskLayer))
            {
                return true;
            }
            return false;
        }

    }
}

