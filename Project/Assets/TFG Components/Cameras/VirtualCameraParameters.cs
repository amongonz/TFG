using System;

using UnityEngine;

namespace Rein.Cameras
{
    [Serializable]
    public struct VirtualCameraParameters
    {
        [Serializable]
        public struct Delta
        {
            public float VerticalFieldOfViewDelta;
            public float NearClipPlaneDelta;
            public float FarClipPlaneDelta;

            public readonly Delta Scale(float scale) => new()
            {
                VerticalFieldOfViewDelta = VerticalFieldOfViewDelta * scale,
                NearClipPlaneDelta = NearClipPlaneDelta * scale,
                FarClipPlaneDelta = FarClipPlaneDelta * scale,
            };
        }

        [Range(1, 179)]
        public float VerticalFieldOfView;
        public float NearClipPlane;
        public float FarClipPlane;

        public readonly Matrix4x4 GetProjectionMatrix(float aspectRatio) =>
            Matrix4x4.Perspective(VerticalFieldOfView, aspectRatio, NearClipPlane, FarClipPlane);

        public readonly VirtualCameraParameters MoveTowards(VirtualCameraParameters to, Delta maxDelta)
        {
            return new()
            {
                VerticalFieldOfView = Mathf.MoveTowards(VerticalFieldOfView, to.VerticalFieldOfView, maxDelta.VerticalFieldOfViewDelta),
                NearClipPlane = Mathf.MoveTowards(NearClipPlane, to.NearClipPlane, maxDelta.NearClipPlaneDelta),
                FarClipPlane = Mathf.MoveTowards(FarClipPlane, to.FarClipPlane, maxDelta.FarClipPlaneDelta),
            };
        }
    }
}
