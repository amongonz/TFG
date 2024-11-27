using System;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Rein.Cameras.OnRails
{
    public class FixedCameraPath : MonoBehaviour
    {
        [MenuItem("GameObject/Rein/Camera Path")]
        public static void OnCreateCommand(MenuCommand command)
        {
            var obj = ReinEditorUtils.AddGameObjectByCommand("Camera Path", command);
            obj.AddComponent<FixedCameraPath>();
        }

        public void OnDrawGizmosSelected()
        {
            var path = GetComponentsInChildren<VirtualCamera>();

            var g = Color.green;
            Gizmos.color = new(g.r, g.g, g.b, 0.5f);
            foreach (var (prev, next) in path.Zip(path.Skip(1), ValueTuple.Create))
            {
                Gizmos.DrawLine(next.transform.position, prev.transform.position);
            }
        }
    }
}
