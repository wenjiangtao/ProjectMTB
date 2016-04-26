/*****
 * 镜头锁定模式后续增加
 * ****/

using System;
using UnityEngine;
namespace MTB
{
    public class PlotCameraController : Singleton<PlotCameraController>
    {
        private MTBCamera _curControlCamera;
        private MTBCamera _oraginCamera;
        private Vector3 _cameraNormalPosition;
        private Vector3 _cameraNormalrotation;
        private CameraMoveData _curPathData;
        private Vector3 _curMoveSpeed;
        private Vector3 _curRotationSpeed;
        private int _curTaskId;
        private int _curStepId;

        private float _curStepCostTime;
        private int _curStepIndex;
        private bool _workMark;
        private int _stepSum;
        private bool _editorMode;

        public void runScript(int id, MTBCamera camera, int taskId = 0, int stepId = 0, bool editorMode = false)
        {
            _curTaskId = taskId;
            _curStepId = stepId;

            _oraginCamera = camera;
            if (_oraginCamera.CameraType == MTBCameraType.Third)
                CameraManager.Instance.UseFirstPersonCamera();
            _curControlCamera = CameraManager.Instance.CurCamera;

            _curPathData = CameraMoveDataManager.Instance.getData(id);
            _stepSum = _curPathData.steps.ToArray().Length;
            startPosition();
            _workMark = true;
            _editorMode = editorMode;
            MTBUserInput.Instance.SetJoyStickActive(false);
            UIManager.Instance.closeUI(UITypes.MAIN_UI);
#if UNITY_EDITOR
            MTBKeyboard.setEnable(false);
#endif
        }

        //编辑器回放专用
        public void runScript(CameraMoveData data, bool editorMode = false)
        {
            _editorMode = editorMode;
            _curPathData = data;

            _oraginCamera = CameraManager.Instance.CurCamera;
            if (_oraginCamera.CameraType == MTBCameraType.Third)
                CameraManager.Instance.UseFirstPersonCamera();
            _curControlCamera = CameraManager.Instance.CurCamera;

            _stepSum = _curPathData.steps.ToArray().Length;
            startPosition();
            _workMark = true;
            MTBUserInput.Instance.SetJoyStickActive(false);
            UIManager.Instance.closeUI(UITypes.MAIN_UI);
#if UNITY_EDITOR
            MTBKeyboard.setEnable(false);
#endif
        }

        public void RecoveryState()
        {
            _workMark = false;
            MTBUserInput.Instance.SetJoyStickActive(true);
            UIManager.Instance.showUI<MainUI>(UITypes.MAIN_UI);
            EventManager.SendEvent(PlotEvent.CAMERAMOVEFINISH, _curTaskId, _curStepId);
            endPosition();

            if (_curControlCamera.CameraType != _oraginCamera.CameraType)
                if (_oraginCamera.CameraType == MTBCameraType.First)
                    CameraManager.Instance.UseFirstPersonCamera();
                else
                    CameraManager.Instance.UseThirdPersonCamera();

#if UNITY_EDITOR
            MTBKeyboard.setEnable(true);
#endif
        }

        void Update()
        {
            if (_workMark)
            {
                updateCameraPosition(Time.deltaTime);
            }
        }

        private void startPosition()
        {
            _cameraNormalPosition = _curControlCamera.transform.position;
            _cameraNormalrotation = _curControlCamera.transform.eulerAngles;
            _curControlCamera.transform.position = _curPathData.startpos.position;
            _curControlCamera.transform.eulerAngles = _curPathData.startpos.rotation;
            _curStepIndex = 0;
            updateSpeed();
        }

        private void endPosition()
        {
            _curControlCamera.transform.position = _cameraNormalPosition;
            _curControlCamera.transform.eulerAngles = _cameraNormalrotation;
        }

        private void updateCameraPosition(float timedelate)
        {
            _curStepCostTime += timedelate;
            _curControlCamera.transform.position += timedelate * _curMoveSpeed;

            _curControlCamera.transform.eulerAngles = new Vector3(
               _curControlCamera.transform.eulerAngles.x + timedelate * _curRotationSpeed.x,
               _curControlCamera.transform.eulerAngles.y + timedelate * _curRotationSpeed.y,
               _curControlCamera.transform.eulerAngles.z + timedelate * _curRotationSpeed.z);
            checkNextStep();
        }

        private void checkNextStep()
        {
            if (_curStepCostTime >= _curPathData.steps[_curStepIndex].time)
            {
                _curStepCostTime = 0;
                _curStepIndex++;
                if (_curStepIndex >= _stepSum)
                {
                    _curStepIndex = 0;
                    RecoveryState();
                    return;
                }
                updateSpeed();
                //_curControlCamera.transform.position = newStep.position;
                //_curControlCamera.transform.eulerAngles = newStep.rotation;
            }
        }

        private void updateSpeed()
        {
            CameraMoveStep newStep = _curPathData.steps[_curStepIndex];
            _curMoveSpeed = (newStep.position - _curControlCamera.transform.position) / newStep.time;

            float rsx = newStep.rotation.x - _curControlCamera.transform.eulerAngles.x;
            rsx = Math.Abs(newStep.rotation.x - _curControlCamera.transform.eulerAngles.x) > 180 ?
               (rsx > 0 ? rsx - 360 : (rsx < 0 ? rsx + 360 : rsx)) : rsx;

            float rsy = newStep.rotation.y - _curControlCamera.transform.eulerAngles.y;
            rsy = Math.Abs(newStep.rotation.y - _curControlCamera.transform.eulerAngles.y) > 180 ?
                (rsy > 0 ? rsy - 360 : (rsy < 0 ? rsy + 360 : rsy)) : rsy;
            float rsz = newStep.rotation.z - _curControlCamera.transform.eulerAngles.z;


            _curRotationSpeed = new Vector3(rsx, rsy, rsz) / newStep.time;

        }
    }
}
