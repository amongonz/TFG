using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

namespace Rein.Cameras
{
    [RequireComponent(typeof(Camera))]
    [ExecuteAlways]
    public class MainCameraVirtualizer : MonoBehaviour
    {
        [Serializable]
        public sealed class UpdateEvent : UnityEvent<VirtualCamera> { }

        public List<VirtualCamera> CameraStack;

        public UpdateEvent UpdatedActiveCamera;
        public UnityEvent ActiveCameraCut;

        private Camera _camera;

        public void Start()
        {
            _camera = GetComponent<Camera>();
        }

        public void PushCamera(VirtualCamera camera)
        {
            var prevCamera = CameraStack.LastOrDefault();
            CameraStack.Add(camera);

            if (camera != prevCamera)
            {
                ActiveCameraCut.Invoke();
            }
        }

        public void RemoveCamera(VirtualCamera camera)
        {
            var prevCamera = CameraStack.Last();
            CameraStack.Remove(camera);

            if (prevCamera != CameraStack.LastOrDefault())
            {
                ActiveCameraCut.Invoke();
            }
        }

        public void LateUpdate()
        {
            if (CameraStack.Count > 0)
            {
                var currentCamera = CameraStack.Last();

                currentCamera.transform.GetPositionAndRotation(out var position, out var orientation);
                transform.SetPositionAndRotation(position, orientation);

                _camera.fieldOfView = currentCamera.Parameters.VerticalFieldOfView;
                _camera.nearClipPlane = currentCamera.Parameters.NearClipPlane;
                _camera.farClipPlane = currentCamera.Parameters.FarClipPlane;

                UpdatedActiveCamera.Invoke(currentCamera);
            }
        }
    }
}
