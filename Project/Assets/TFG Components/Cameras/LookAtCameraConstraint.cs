using UnityEngine;

namespace Rein.Cameras
{
    [ExecuteAlways]
    public class LookAtCameraConstraint : MonoBehaviour
    {
        public Transform LookAtTarget;
        public Transform RollTarget;

        public void Update()
        {
            if (LookAtTarget)
            {
                var worldUp = RollTarget ? RollTarget.up : Vector3.up;
                transform.LookAt(LookAtTarget, worldUp);
            }
        }
    }
}
