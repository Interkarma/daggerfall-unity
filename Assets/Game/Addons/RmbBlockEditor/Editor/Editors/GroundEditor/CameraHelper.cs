// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

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