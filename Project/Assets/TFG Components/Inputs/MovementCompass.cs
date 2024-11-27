using System;

using Rein.Cameras;

using UnityEditor;

using UnityEngine;

namespace Rein.Inputs
{
    public class MovementCompass : MonoBehaviour
    {
        [Range(0, 180)]
        public float UnlockAngle;

        public Vector3 PlaneForward;
        public bool IsLocked;
        public Vector2 LockedInputVector;

        public Vector3 GizmoOffset;
        public float GizmoScale;

        [MenuItem("GameObject/Rein/Movement Plane")]
        public static void OnCreateCommand(MenuCommand command)
        {
            var obj = ReinEditorUtils.AddGameObjectByCommand("Movement Plane", command);
            obj.AddComponent<MovementCompass>();
        }

        public void Reset()
        {
            UnlockAngle = 30;
            GizmoScale = 1;
        }

        private Quaternion GetPlaneOrientation() => Quaternion.LookRotation(PlaneForward, transform.up);

        public void OnCameraCut() => IsLocked = true;

        public void OnCameraUpdate(VirtualCamera camera)
        {
            if (!IsLocked)
            {
                PlaneForward = Vector3.ProjectOnPlane(camera.transform.forward, transform.up);
            }
        }

        public Vector3 OnInputAxis(Vector2 vector)
        {
            if (IsLocked)
            {
                if (vector == Vector2.zero || Vector2.Angle(vector, LockedInputVector) >= UnlockAngle)
                {
                    IsLocked = false;
                }
            }

            if (!IsLocked)
            {
                LockedInputVector = vector;
            }

            return GetPlaneOrientation() * new Vector3(LockedInputVector.x, 0, LockedInputVector.y);
        }

        public void DrawGizmosAt(Vector3 position)
        {
            var c = Color.cyan;
            c.a = 0.5f;
            Gizmos.color = c;

            Gizmos.matrix = Matrix4x4.TRS(position, GetPlaneOrientation(), Vector3.one);
            Gizmos.DrawCube(Vector3.zero, new(GizmoScale, 0, GizmoScale));

            Gizmos.color = Color.white;
            Gizmos.DrawLine(Vector3.zero, GizmoScale / 2 * Vector3.forward);
        }

        public void OnDrawGizmosSelected() => DrawGizmosAt(transform.position);
    }
}
