using System.Linq;

using UnityEngine;

namespace Rein.Cameras.OnRails
{
    [RequireComponent(typeof(VirtualCamera))]
    [ExecuteAlways]
    public class FixedCameraSet : MonoBehaviour
    {
        public FixedCameraPath Path;
        public Transform Target;

        public float TransitionSpeed;
        public float AngleTransitionSpeed;
        public VirtualCameraParameters.Delta ParameterTransitionSpeed;

        public LayerMask ClearShotMask;

        private VirtualCamera _camera;

        public void Start()
        {
            _camera = GetComponent<VirtualCamera>();
        }

        private Bounds GetTargetBounds()
        {
            var targetRenderer = Target.GetComponent<Renderer>();
            return targetRenderer ? targetRenderer.bounds : new(Target.position, Vector3.zero);
        }

        public void Update()
        {
            var allCameras = Path.GetComponentsInChildren<VirtualCamera>();
            var bounds = GetTargetBounds();
            float aspectRatio = Camera.main.aspect;

            var clearShotCameras =
                allCameras
                .Where(camera =>
                    {
                        var origin = camera.transform.position;
                        var dir = Target.position - origin;

                        if (camera.TestFrustumAABB(bounds, aspectRatio))
                        {
                            if (Physics.Raycast(new(origin, dir), out var hitInfo, float.PositiveInfinity, ClearShotMask))
                            {
                                return hitInfo.distance * hitInfo.distance > dir.sqrMagnitude;
                            }

                            return true;
                        }

                        return false;
                    })
                .ToArray();

            if (clearShotCameras.Length > 0)
            {
                var maxAnchorIndex = allCameras.Max(camera => camera.transform.GetSiblingIndex());

                var nearestAnchor =
                    allCameras
                    .OrderBy(camera => (camera.transform.position - transform.position).sqrMagnitude)
                    .First();

                var bestAnchor =
                    clearShotCameras
                    .OrderBy(camera => (Target.position - camera.transform.position).sqrMagnitude)
                    .First();

                int nearestAnchorIndex = nearestAnchor.transform.GetSiblingIndex();
                int bestAnchorIndex = bestAnchor.transform.GetSiblingIndex();

                int nextAnchorIndex = nearestAnchorIndex;
                if (bestAnchorIndex < nextAnchorIndex)
                {
                    nextAnchorIndex = Mathf.Max(0, nextAnchorIndex - 1);
                }
                else if (nextAnchorIndex < bestAnchorIndex)
                {
                    nextAnchorIndex = Mathf.Min(nextAnchorIndex + 1, maxAnchorIndex);
                }

                var nextAnchor = Path.transform.GetChild(nextAnchorIndex);

                var position = Vector3.MoveTowards(transform.position, nextAnchor.position, TransitionSpeed * Time.deltaTime);
                var rotation = Quaternion.RotateTowards(transform.rotation, nextAnchor.rotation, AngleTransitionSpeed * Time.deltaTime);
                transform.SetPositionAndRotation(position, rotation);

                var nextAnchorParameters = nextAnchor.GetComponent<VirtualCamera>().Parameters;
                _camera.Parameters = _camera.Parameters.MoveTowards(nextAnchorParameters, ParameterTransitionSpeed.Scale(Time.deltaTime));
            }
        }
    }
}
