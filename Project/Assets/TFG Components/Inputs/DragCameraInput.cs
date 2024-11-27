using UnityEngine;

namespace Rein.Inputs
{
    [CreateAssetMenu(menuName = "Rein/Drag Camera Input")]
    public class DragCameraInput : ScriptableObject
    {
        public float FreeRadius;
        public float FreeSensitivity;
        public float DragSensitivity;

        public void Reset()
        {
            FreeRadius = 0;
            FreeSensitivity = 1;
            DragSensitivity = 1;
        }

        public void HandleInput(Vector2 input, ref Vector2 freePosition, out Vector2 drag)
        {
            var targetPos = freePosition + FreeSensitivity * input;
            freePosition = Vector2.ClampMagnitude(targetPos, FreeRadius);
            drag = DragSensitivity * (targetPos - freePosition);
        }
    }
}
