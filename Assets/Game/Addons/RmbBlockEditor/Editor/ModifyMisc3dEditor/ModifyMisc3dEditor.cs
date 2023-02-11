// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Utility.WorldDataEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class ModifyMisc3dEditor
    {
        private VisualElement visualElement;
        private string objectId;
        private ObjectPicker pickerObject;
        private ClimateBases climate;
        private ClimateSeason season;
        private WindowStyle windowStyle;
        private Action<uint> apply;

        public ModifyMisc3dEditor(uint objectId, Action<uint> apply)
        {
            visualElement = new VisualElement();
            this.objectId = objectId.ToString();
            this.apply = apply;
            RenderTemplate();
            RenderObjectPicker();
            BindApplyButton();
        }

        public VisualElement Render(ClimateBases climate, ClimateSeason season, WindowStyle windowStyle)
        {
            this.climate = climate;
            this.season = season;
            this.windowStyle = windowStyle;
            return visualElement;
        }

        private void RenderTemplate()
        {
            var tree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/ModifyMisc3dEditor/Template.uxml");
            visualElement.Add(tree.CloneTree());
        }

        private void RenderObjectPicker()
        {
            var modelPicker = visualElement.Query<VisualElement>("object-picker").First();
            var categories = new Dictionary<string, Dictionary<string, string>>
            {
                { "Furniture", WorldDataEditorObjectData.models_furniture },
                { "Clutter", WorldDataEditorObjectData.models_clutter },
                { "Structure", WorldDataEditorObjectData.models_structure },
                { "Graveyard", WorldDataEditorObjectData.models_graveyard },
                { "HouseParts", WorldDataEditorObjectData.houseParts },
                { "Dungeon", WorldDataEditorObjectData.models_dungeon },
                { "Dungeon Rooms", WorldDataEditorObjectData.dungeonParts_rooms },
                { "Dungeon Corridors", WorldDataEditorObjectData.dungeonParts_corridors },
                { "Dungeon Misc", WorldDataEditorObjectData.dungeonParts_misc },
                { "Dungeon Caves", WorldDataEditorObjectData.dungeonParts_caves },
                { "Dungeon Doors", WorldDataEditorObjectData.dungeonParts_doors },
            };

            if (pickerObject != null)
            {
                pickerObject.Destroy();
            }

            pickerObject =
                new ObjectPicker(categories, WorldDataEditorObjectData.modelGroups,
                    true, OnItemSelected, AddPreview, objectId);
            modelPicker.Add(pickerObject.visualElement);
        }

        private void BindApplyButton()
        {
            var button = visualElement.Query<Button>("apply-modification").First();
            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                apply((uint)int.Parse(objectId));
                if (pickerObject != null)
                {
                    pickerObject.Destroy();
                }
            });
        }

        private void OnItemSelected(string objectId)
        {
            this.objectId = objectId;
        }

        private GameObject AddPreview(string modelId)
        {
            var previewObject = RmbBlockHelper.Add3dObject(modelId, climate, season, windowStyle);
            return previewObject;
        }
    }
}