using UnityEditor;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public static class CameraHelper
    {
        public static void CenterCamera()
        {
            var abovePos = new Vector3(51.2f, 51.2f, -51.2f);
            var camPos = SceneView.lastActiveSceneView.pivot;
            camPos.x = abovePos.x;
            camPos.y = abovePos.y;
            camPos.z = abovePos.z;
            SceneView.lastActiveSceneView.pivot = camPos;
            SceneView.lastActiveSceneView.LookAt(camPos, Quaternion.Euler(90, 0, 0));
            SceneView.lastActiveSceneView.Repaint();
        }
    }
}