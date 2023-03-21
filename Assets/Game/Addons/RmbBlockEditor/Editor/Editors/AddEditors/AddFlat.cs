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
using DaggerfallWorkshop.Game.Utility.WorldDataEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class AddFlat
    {
        private VisualElement visualElement = new VisualElement();
        private string objectId = "";
        private ObjectPainter painterObject;
        private ObjectPicker pickerObject;
        private List<CatalogItem> catalog;

        public AddFlat()
        {
            catalog = PersistedFlatsCatalog.List();
            RenderTemplate();
            RenderObjectPicker();
            RenderPainter();
        }

        public VisualElement Render()
        {
            catalog = PersistedFlatsCatalog.List();
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
            var pickerElement = visualElement.Query<VisualElement>("object-picker").First();
            pickerElement.Clear();

            pickerObject =
                new ObjectPicker(catalog, OnItemSelected, GetPreview, objectId);
            pickerElement.Add(pickerObject.visualElement);
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
                    new FlatPainter(() => RmbBlockHelper.AddFlatObject(objectId), AddBillboard);
                painterContainer.Add(painterObject.visualElement);
            }
        }

        private void OnItemSelected(string objectId)
        {
            this.objectId = objectId;
            RenderPainter();
        }

        private void AddBillboard(Vector3 position)
        {
            try
            {
                AddBillboard(objectId, position);
            }
            catch (Exception e)
            {
                Console.WriteLine("No such Object ID is available.");
            }
        }

        private void AddBillboard(string flatId, Vector3 position)
        {
            var dot = Char.Parse(".");
            var splitId = flatId.Split(dot);
            var data = new DFBlock.RmbBlockFlatObjectRecord();
            data.TextureArchive = int.Parse(splitId[0]);
            data.TextureRecord = int.Parse(splitId[1]);
            data.XPos = (int)position.x;
            data.YPos = (int)position.y - 2;
            data.ZPos = (int)position.z;

            var gameObject = RmbBlockHelper.AddFlatObject(flatId);
            var miscFlatObjectComponent = gameObject.AddComponent<MiscFlat>();
            miscFlatObjectComponent.CreateObject(data);

            var miscFlatRoot = GameObject.Find("Misc Flat Objects");
            gameObject.transform.parent = miscFlatRoot.transform;
        }

        private VisualElement GetPreview(string id)
        {
            var go = RmbBlockHelper.AddFlatObject(id);
            return new GoPreview(go);
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