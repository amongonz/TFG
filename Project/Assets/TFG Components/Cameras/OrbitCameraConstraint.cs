using System;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Rein.Cameras
{
    [RequireComponent(typeof(VirtualCamera))]
    [RequireComponent(typeof(SphereCollider))]
    [ExecuteAlways]
    public class OrbitCameraConstraint : MonoBehaviour
    {
        [Serializable]
        public struct Impulse
        {
            public float HorizontalAngleImpulse;
            public float VerticalAngleImpulse;

            public static Impulse operator +(Impulse a, Impulse b) =>
                new()
                {

                    HorizontalAngleImpulse = a.HorizontalAngleImpulse + b.HorizontalAngleImpulse,
                    VerticalAngleImpulse = a.VerticalAngleImpulse + b.VerticalAngleImpulse,
                };

            public Impulse OrElse(Impulse other) =>
                new()
                {
                    HorizontalAngleImpulse = HorizontalAngleImpulse != 0 ? HorizontalAngleImpulse : other.HorizontalAngleImpulse,
                    VerticalAngleImpulse = VerticalAngleImpulse != 0 ? VerticalAngleImpulse : other.VerticalAngleImpulse,
                };
        }

        public Transform LookAtTarget;
        public Transform OrbitTarget;
        public Vector3 OrbitOffset;
        public AnimationCurve Distance;
        public AnimationCurve VerticalFieldOfView;

        public float HorizontalAngle;
        [Range(-89, 89)]
        public float VerticalAngle;

        public LayerMask CollisionLayers;

        private VirtualCamera _camera;
        private SphereCollider _collider;

        private Impulse _nextLowPriorityImpulse;
        private Impulse _nextHighPriorityImpulse;

        [MenuItem("GameObject/Rein/Orbit Camera")]
        public static void OnCreateCommand(MenuCommand command)
        {
            var obj = ReinEditorUtils.AddGameObjectByCommand("Orbit Camera", command);
            obj.AddComponent<OrbitCameraConstraint>();
        }

        [ContextMenu("Reset curves")]
        public void ResetCurves()
        {
            Distance = AnimationCurve.Constant(-89, 89, 4);
            VerticalFieldOfView = AnimationCurve.Constant(-89, 89, _camera.Parameters.VerticalFieldOfView);
        }

        public void Reset()
        {
            ResetCurves();
        }

        public void Start()
        {
            _camera = GetComponent<VirtualCamera>();
            _collider = GetComponent<SphereCollider>();
        }

        public Vector3 GetPivot() => OrbitTarget.position + Quaternion.AngleAxis(HorizontalAngle, Vector3.up) * OrbitOffset;

        public void AddImpulse(Impulse impulse, bool isHighPriority = false)
        {
            if (isHighPriority)
            {
                _nextHighPriorityImpulse += impulse;
            }
            else
            {
                _nextLowPriorityImpulse += impulse;
            }
        }

        private (float Min, float Max) GetVerticalAngleBounds() => (Distance.keys.First().time, Distance.keys.Last().time);

        public void LateUpdate()
        {
            var impulse = _nextHighPriorityImpulse.OrElse(_nextLowPriorityImpulse);
            _nextHighPriorityImpulse = default;
            _nextLowPriorityImpulse = default;

            HorizontalAngle += impulse.HorizontalAngleImpulse * Time.smoothDeltaTime;
            VerticalAngle += impulse.VerticalAngleImpulse * Time.smoothDeltaTime;

            if (OrbitTarget)
            {
                var vAngleBounds = GetVerticalAngleBounds();
                VerticalAngle = Mathf.Clamp(VerticalAngle, vAngleBounds.Min, vAngleBounds.Max);
                var targetDir = Quaternion.Euler(VerticalAngle, HorizontalAngle, 0) * Vector3.back;

                var orbitPivot = GetPivot();

                float targetDistance = Distance.Evaluate(VerticalAngle);
                if (Physics.SphereCast(new(orbitPivot, targetDir), _collider.radius, out var hitInfo, targetDistance, CollisionLayers))
                {
                    var posAtHit = hitInfo.point + _collider.radius * hitInfo.normal;
                    targetDistance = (posAtHit - orbitPivot).magnitude;
                }

                transform.position = orbitPivot + targetDistance * targetDir;
                _camera.Parameters.VerticalFieldOfView = VerticalFieldOfView.Evaluate(VerticalAngle);

                var lookAtPosition = LookAtTarget ? LookAtTarget.position : orbitPivot;
                transform.LookAt(lookAtPosition, Vector3.up);
            }
        }

        public void OnDrawGizmosSelected()
        {
            if (OrbitTarget)
            {
                int verticalSteps = 30;
                var verticalPath = Enumerable.Range(0, verticalSteps).Select(i =>
                {
                    var vAngleBounds = GetVerticalAngleBounds();
                    var angle = Mathf.Lerp(vAngleBounds.Min, vAngleBounds.Max, (float)i / verticalSteps);

                    return Quaternion.Euler(angle, HorizontalAngle, 0)
                        * Vector3.back
                        * Distance.Evaluate(angle);
                }).ToList();

                var targetDistance = Distance.Evaluate(VerticalAngle);

                int horizontalSteps = 60;
                var horizontalPath = Enumerable.Range(0, horizontalSteps + 1).Select(i =>
                {
                    var angle = Mathf.Lerp(0, 360, (float)i / horizontalSteps);

                    return Quaternion.Euler(VerticalAngle, angle, 0)
                        * Vector3.back
                        * targetDistance;
                }).ToList();

                Gizmos.color = Color.green * 0.9f;
                Gizmos.matrix = Matrix4x4.Translate(GetPivot());
                foreach (var (startPos, endPos) in verticalPath.Zip(verticalPath.Skip(1), ValueTuple.Create))
                {
                    Gizmos.DrawLine(startPos, endPos);
                }
                foreach (var (startPos, endPos) in horizontalPath.Zip(horizontalPath.Skip(1), ValueTuple.Create))
                {
                    Gizmos.DrawLine(startPos, endPos);
                }
            }
        }
    }
}
