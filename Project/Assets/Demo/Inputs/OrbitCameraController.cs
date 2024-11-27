using System.Linq;

using Rein.Cameras;
using Rein.Inputs;

using UnityEngine;

namespace Rein.Demo
{
    [RequireComponent(typeof(OrbitCameraConstraint))]
    public class OrbitCameraController : MonoBehaviour
    {
        public DragCameraInput ReticleInput;
        public Vector2 ReticlePos;
        public float CameraSpeed;

        public RectTransform ReticleObject;

        private OrbitCameraConstraint _orbitCamera;
        private MainCameraVirtualizer _mainCamera;

        public void Start()
        {
            _orbitCamera = GetComponent<OrbitCameraConstraint>();
            _mainCamera = FindAnyObjectByType<MainCameraVirtualizer>();
        }

        public void Update()
        {
            var cameraDelta = new Vector2(Input.GetAxisRaw("Camera X"), -Input.GetAxisRaw("Camera Y"));
            ReticleInput.HandleInput(cameraDelta, ref ReticlePos, out var cameraDrag);
            Debug.Log(Input.gyro.attitude);

            _orbitCamera.AddImpulse(new()
            {
                HorizontalAngleImpulse = cameraDrag.x * CameraSpeed,
                VerticalAngleImpulse = cameraDrag.y * CameraSpeed,
            }, isHighPriority: true);
        }

        public void LateUpdate()
        {
            if (ReticleObject && _mainCamera.CameraStack.Last() == _orbitCamera.GetComponent<VirtualCamera>())
            {
                ReticleObject.anchoredPosition = ReticlePos * new Vector3(1, -1, 1);
                ReticleObject.gameObject.SetActive(true);
            }
            else
            {
                ReticleObject.gameObject.SetActive(false);
            }
        }
    }
}
