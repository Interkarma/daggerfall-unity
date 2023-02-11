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
    [ExecuteInEditMode]
    public class SceneryItem : MonoBehaviour
    {
        public int textureRecord;
        public int i;
        public int j;
        public void CreateObject(int textureRecord, int i, int j)
        {
            this.textureRecord = textureRecord;
            this.i = i;
            this.j = j;
            SceneView.duringSceneGui += OnSceneGUI;
        }
        protected void OnSceneGUI(SceneView sceneView)
        {
            var cameraPos = SceneView.lastActiveSceneView.camera.transform.position;
            var targetPos = new Vector3(cameraPos.x, transform.position.y, cameraPos.z);
            transform.LookAt(targetPos);
        }

        protected void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }
}