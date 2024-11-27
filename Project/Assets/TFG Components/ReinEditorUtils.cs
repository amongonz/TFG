using UnityEditor;

using UnityEngine;

namespace Rein
{
    public static class ReinEditorUtils
    {
        public static GameObject AddGameObjectByCommand(string name, MenuCommand command)
        {
            var obj = new GameObject(name);
            GameObjectUtility.SetParentAndAlign(child: obj, parent: command.context as GameObject);
            Undo.RegisterCreatedObjectUndo(obj, $"Create {obj.name}.");
            Selection.activeGameObject = obj;
            return obj;
        }
    }
}
