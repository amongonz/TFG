using UnityEditor;

using UnityEngine;

namespace Rein.Cameras
{
    public class VirtualCamera : MonoBehaviour
    {
        public VirtualCameraParameters Parameters;

        [MenuItem("GameObject/Rein/Virtual Camera")]
        public static void OnCreateCommand(MenuCommand command)
        {
            var obj = ReinEditorUtils.AddGameObjectByCommand("Virtual Camera", command);
            obj.AddComponent<VirtualCamera>();
        }

        public void Reset()
        {
            Parameters = new()
            {
                VerticalFieldOfView = 70,
                NearClipPlane = 0.3f,
                FarClipPlane = 1000,
            };
        }

        public Matrix4x4 GetViewMatrix()
        {
            // Unity cameras use the OpenGL convention of negative-Z forward,
            // the opposite to the rest of the engine.
            Vector3 scale = new(1, 1, -1);

            var tr = transform;
            return Matrix4x4.Inverse(Matrix4x4.TRS(tr.position, tr.rotation, scale));
        }

        public Matrix4x4 GetViewProjectionMatrix(float aspectRatio) =>
            Parameters.GetProjectionMatrix(aspectRatio) * GetViewMatrix();

        public bool TestFrustumAABB(Bounds bounds, float aspectRatio)
        {
            var viewProjection = GetViewProjectionMatrix(aspectRatio);
            var planes = GeometryUtility.CalculateFrustumPlanes(viewProjection);
            return GeometryUtility.TestPlanesAABB(planes, bounds);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white * 0.9f;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawFrustum(
                Vector3.zero,
                Parameters.VerticalFieldOfView,
                Parameters.FarClipPlane,
                Parameters.NearClipPlane,
                Camera.main.aspect
            );
        }
    }
}
