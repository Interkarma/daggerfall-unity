// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Addons.RmbBlockEditor.Elements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class Add3d
    {
        private VisualElement visualElement = new VisualElement();
        private string objectId = "";
        private ObjectPainter painterObject;
        private ObjectPicker pickerObject;
        private List<CatalogItem> catalog = PersistedModelsCatalog.List();

        public Add3d()
        {
            RenderTemplate();
            RenderObjectPicker();
            RenderPainter();
        }

        public VisualElement Render()
        {
            catalog = PersistedModelsCatalog.List();
            RenderObjectPicker();
            return visualElement;
        }

        private void RenderTemplate()
        {
            var tree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/AddEditors/Template.uxml");
            visualElement.Add(tree.CloneTree());
        }

        private void RenderObjectPicker()
        {
            var modelPicker = visualElement.Query<VisualElement>("object-picker").First();
            modelPicker.Clear();

            pickerObject =
                new ObjectPicker(catalog, OnItemSelected, GetPreview, objectId);
            modelPicker.Add(pickerObject.visualElement);
        }

        private void RenderPainter()
        {
            var painterContainer = visualElement.Query<VisualElement>("painter-container").First();
            painterContainer.Clear();
            if (painterObject != null)
            {
                painterObject.Destroy();
            }

            if (objectId != "")
            {
                painterObject =
                    new ModelPainter(AddPlaceholder, AddModel);
                painterContainer.Add(painterObject.visualElement);
            }
        }

        private void OnItemSelected(string objectId)
        {
            this.objectId = objectId;
            RenderPainter();
        }

        private GameObject AddPlaceholder()
        {
            return RmbBlockHelper.Add3dObject(objectId);
        }

        private VisualElement GetPreview(string modelId)
        {
            var previewObject = RmbBlockHelper.Add3dObject(modelId);
            return new GoPreview(previewObject);
        }

        private void AddModel(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            try
            {
                AddMisc3d(objectId, position, rotation, scale);
            }
            catch (Exception e)
            {
                Console.WriteLine("No such Object ID is available.");
            }
        }

        private void AddMisc3d(string modelId, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            DFBlock.RmbBlock3dObjectRecord data = new DFBlock.RmbBlock3dObjectRecord();
            data.ModelId = modelId;
            data.ModelIdNum = uint.Parse(modelId);
            data.ObjectType = 0;
            data.XPos = (int)position.x;
            data.YPos = (int)position.y;
            data.ZPos = (int)position.z;
            data.XRotation = (short)rotation.x;
            data.YRotation = (short)rotation.y;
            data.ZRotation = (short)rotation.z;
            data.XScale = scale.x;
            data.YScale = scale.y;
            data.ZScale = scale.z;

            var gameObject = RmbBlockHelper.Add3dObject(modelId);
            var misc3DObjectComponent = gameObject.AddComponent<Misc3d>();
            misc3DObjectComponent.CreateObject(data);

            var misc3dRoot = GameObject.Find("Misc 3D Objects");
            gameObject.transform.parent = misc3dRoot.transform;
        }

        public void Destroy()
        {
            if (painterObject != null)
            {
                painterObject.Destroy();
            }
        }
    }
}