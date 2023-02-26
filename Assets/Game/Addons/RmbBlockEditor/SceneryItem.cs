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
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class SceneryItem : MonoBehaviour
    {
        public int textureRecord;
        public int i;
        public int j;
        private int currentArchive;
        private DaggerfallBillboard billboard;

        public void CreateObject(int textureRecord, int i, int j)
        {
            var xPos = i * 6.4f;
            var zPos = j * 6.4f;
            this.textureRecord = textureRecord;
            this.i = i;
            this.j = j;
            currentArchive = RmbBlockHelper.GetSceneryTextureArchive();
            billboard = gameObject.AddComponent<DaggerfallBillboard>();
            billboard.SetMaterial(currentArchive, textureRecord);
            transform.position = new Vector3(xPos, billboard.Summary.Size.y / 2, -zPos);
        }

        public void Start()
        {
            SceneView.beforeSceneGui += OnSceneGUI;
        }

        public void Update()
        {
            var newArchive = RmbBlockHelper.GetSceneryTextureArchive();
            if (currentArchive == newArchive)
            {
                return;
            }

            var position = transform.position;
            currentArchive = newArchive;
            billboard.SetMaterial(currentArchive, textureRecord);

            // The same texture record can vary in height, depending on the archive so it needs to be positioned
            transform.position = new Vector3(position.x, billboard.Summary.Size.y / 2, position.z);
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            var cameraPos = SceneView.lastActiveSceneView.camera.transform.position;
            var targetPos = new Vector3(cameraPos.x, transform.position.y, cameraPos.z);
            transform.LookAt(targetPos);
        }

        private void OnDestroy()
        {
            SceneView.beforeSceneGui -= OnSceneGUI;
        }
    }
#endif
}