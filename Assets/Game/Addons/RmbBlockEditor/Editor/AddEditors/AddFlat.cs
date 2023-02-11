// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Utility.WorldDataEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class AddFlat
    {
        private VisualElement visualElement;
        private string objectId;
        private ObjectPainter painterObject;
        private ObjectPicker pickerObject;

        public AddFlat()
        {
            visualElement = new VisualElement();
            objectId = "";
            RenderTemplate();
            RenderObjectPicker();
            RenderPainter();
        }

        public VisualElement Render()
        {
            return visualElement;
        }

        private void RenderTemplate()
        {
            var tree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/AddEditors/Template.uxml");
            visualElement.Add(tree.CloneTree());
        }

        private void RenderObjectPicker()
        {
            var modelPicker = visualElement.Query<VisualElement>("object-picker").First();
            var categories = new Dictionary<string, Dictionary<string, string>>
            {
                { "Interior", WorldDataEditorObjectData.billboards_interior },
                { "Exterior", WorldDataEditorObjectData.billboards_nature },
                { "Lights", WorldDataEditorObjectData.billboards_lights },
                { "Dungeon", WorldDataEditorObjectData.billboards_treasure },
                { "Npc", WorldDataEditorObjectData.NPCs },
            };

            if (pickerObject != null)
            {
                pickerObject.Destroy();
            }

            pickerObject =
                new ObjectPicker(categories, WorldDataEditorObjectData.flatGroups,
                    true, OnItemSelected, RmbBlockHelper.AddFlatObject);
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
            data.YPos = (int)position.y;
            data.ZPos = (int)position.z;

            var gameObject = RmbBlockHelper.AddFlatObject(flatId);
            var miscFlatObjectComponent = gameObject.AddComponent<MiscFlat>();
            miscFlatObjectComponent.CreateObject(data);

            var miscFlatRoot = GameObject.Find("Misc Flat Objects");
            gameObject.transform.parent = miscFlatRoot.transform;
        }

        public void Destroy()
        {
            if (painterObject != null)
            {
                painterObject.Destroy();
            }
            if (pickerObject != null)
            {
                pickerObject.Destroy();
            }
        }
    }
}