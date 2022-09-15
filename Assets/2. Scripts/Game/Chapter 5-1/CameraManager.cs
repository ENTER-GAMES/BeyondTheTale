using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chapter_5_1
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField]
        private CameraSetting[] cameraSettings;

        [ContextMenu("UseFistCameraSetting")]
        public void UseFistCameraSetting() => ChangeCameraSettingByIndex(0);

        public void ChangeCameraSettingByIndex(int index)
        {
            if (index < 0 || index >= cameraSettings.Length) return;

            CameraSetting setting = cameraSettings[index];
            Camera.main.transform.position = setting.cameraPosition;
            Camera.main.orthographicSize = setting.cameraSize;
        }

        [Serializable]
        public struct CameraSetting
        {
            public Vector3 cameraPosition;
            public float cameraSize;
        }
    }
}
