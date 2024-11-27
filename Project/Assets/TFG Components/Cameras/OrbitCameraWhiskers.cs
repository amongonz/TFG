using System;

using UnityEditor;

using UnityEngine;

namespace Rein.Cameras
{
    [RequireComponent(typeof(OrbitCameraConstraint))]
    [ExecuteAlways]
    public class OrbitCameraWhiskers : MonoBehaviour
    {
        [Range(0, 180)]
        public float SpreadAngle;
        public int CountPerSide;
        public float SwingImpulse;

        private OrbitCameraConstraint _orbitCamera;
        private Ray[] _leftWhiskers;

        [MenuItem("GameObject/Rein/Orbit Camera (Whiskers)")]
        public static void OnCreateCommand(MenuCommand command)
        {
            var obj = ReinEditorUtils.AddGameObjectByCommand("Orbit Camera", command);
            obj.AddComponent<OrbitCameraWhiskers>();
        }

        public void Start()
        {
            _orbitCamera = GetComponent<OrbitCameraConstraint>();
        }

        private void UpdateWhiskers()
        {
            if (_orbitCamera.OrbitTarget)
            {
                if (_leftWhiskers == null || CountPerSide != _leftWhiskers.Length)
                {
                    _leftWhiskers = new Ray[CountPerSide];
                }

                var orbitCenter = _orbitCamera.GetPivot();
                float startAngle = (-SpreadAngle) / 2;
                float stepAngle = SpreadAngle / CountPerSide / 2;

                for (int i = 0; i < CountPerSide; i++)
                {
                    float angle = startAngle + i * stepAngle;
                    Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.rotation * Vector3.back;
                    _leftWhiskers[i] = new(orbitCenter, dir);
                }
            }
            else
            {
                _leftWhiskers = Array.Empty<Ray>();
            }
        }

        public void LateUpdate()
        {
            UpdateWhiskers();

            float distance = Vector3.Distance(_orbitCamera.transform.position, _orbitCamera.GetPivot());
            float impulsePerRay = SwingImpulse / CountPerSide;
            float impulse = 0;
            foreach (var leftRay in _leftWhiskers)
            {
                if (Physics.Raycast(leftRay, out var leftHitInfo, distance, _orbitCamera.CollisionLayers))
                {
                    impulse += impulsePerRay * (distance - leftHitInfo.distance);
                }

                Ray rightRay = new(leftRay.origin, Vector3.Reflect(leftRay.direction, transform.right));
                if (Physics.Raycast(rightRay, out var rightHitInfo, distance, _orbitCamera.CollisionLayers))
                {
                    impulse -= impulsePerRay * (distance - rightHitInfo.distance);
                }
            }

            _orbitCamera.AddImpulse(new() { HorizontalAngleImpulse = impulse });
        }

        public void OnDrawGizmosSelected()
        {
            UpdateWhiskers();

            void DrawRay(Ray ray)
            {
                Gizmos.color = Color.white * 0.9f;
                float distance = Vector3.Distance(_orbitCamera.transform.position, _orbitCamera.GetPivot()); ;
                if (Physics.Raycast(ray, out var hitInfo, distance, _orbitCamera.CollisionLayers))
                {
                    Gizmos.color = Color.yellow;
                    distance = hitInfo.distance;
                }

                Gizmos.DrawRay(ray.origin, distance * ray.direction);
            }

            Gizmos.matrix = Matrix4x4.identity;
            foreach (var leftRay in _leftWhiskers)
            {
                DrawRay(leftRay);

                Ray rightRay = new(leftRay.origin, Vector3.Reflect(leftRay.direction, transform.right));
                DrawRay(rightRay);
            }
        }
    }
}
