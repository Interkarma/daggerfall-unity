// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using DaggerfallWorkshop.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class SceneryEditor
    {
        public VisualElement visualElement;
        private GameObject gameObject;
        private GameObject placeholderGo;
        public static int BlankTextureRecord = -1;
        private int selectedScenery = BlankTextureRecord;

        public SceneryEditor(GameObject go)
        {
            gameObject = go;
            visualElement = new VisualElement();
        }

        public VisualElement Render()
        {
            visualElement.Clear();
            RenderTemplate();
            RenderCells();
            BindPaintButton();
            return visualElement;
        }

        private void RenderTemplate()
        {
            var tree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/SceneryEditor/Template.uxml");
            visualElement.Add(tree.CloneTree());
        }

        private void RenderCells()
        {
            var images = visualElement.Query<Image>(null, "scenery-cell");
            images.ForEach(cell =>
            {
                var record = int.Parse(cell.name.Replace("scenery-", ""));
                var texture = GetGroundTexture(record);
                cell.image = texture;
                cell.userData = record;
                cell.scaleMode = ScaleMode.ScaleToFit;
                cell.RegisterCallback<MouseUpEvent>(evt =>
                {
                    StopPainting();
                    var component = (Image)evt.currentTarget;
                    var textureRecord = (int)component.userData;
                    SelectNewCell(selectedScenery, textureRecord);
                    selectedScenery = textureRecord;
                }, TrickleDown.TrickleDown);
            });
        }

        private void BindPaintButton()
        {
            var paint = visualElement.Query<Button>("paint").First();
            paint.RegisterCallback<MouseUpEvent>(evt =>
            {
                DestroyPlaceholder();
                CreatePlaceholder();
            }, TrickleDown.TrickleDown);
        }

        private void CreatePlaceholder()
        {
            if (selectedScenery == -1) return;

            placeholderGo = RmbBlockHelper.AddFlatObject(RmbBlockHelper.GetSceneryTextureArchive() + "." + selectedScenery);
            placeholderGo.name = "Placeholder";
            var placeholder = placeholderGo.AddComponent<SceneryPlaceholder>();
            placeholder.CreateFlatPlaceholder(PlaceScenery, StopPainting);
        }

        private void DestroyPlaceholder()
        {
            // Remove the placeholder GameObject
            if (placeholderGo != null)
            {
                GameObject.DestroyImmediate(placeholderGo);
            }
        }

        private void PlaceScenery(int x, int z)
        {
            var scenery = gameObject.GetComponent<Scenery>();
            scenery.AddItem(selectedScenery, x, z);
        }

        private void StopPainting()
        {
            SelectNewCell(selectedScenery, BlankTextureRecord);
            selectedScenery = BlankTextureRecord;
            DestroyPlaceholder();
        }

        private void SelectNewCell(int oldCell, int newCell)
        {
            var oldSelectedCell = visualElement.Query<Image>("scenery-" + oldCell).First();
            if (oldSelectedCell != null)
            {
                oldSelectedCell.RemoveFromClassList("selected");
            }

            var newSelectedCell = visualElement.Query<Image>("scenery-" + newCell).First();
            if (newSelectedCell != null)
            {
                newSelectedCell.AddToClassList("selected");
            }
        }

        private static Texture2D GetGroundTexture(int record)
        {
            var dfUnity = DaggerfallUnity.Instance;
            var textureReader = new TextureReader(dfUnity.Arena2Path);
            var texture = record == BlankTextureRecord
                ? GetBlankTexture()
                : textureReader.GetTexture2D(RmbBlockHelper.GetSceneryTextureArchive(), record, 0, 0);
            return texture;
        }

        private static Texture2D GetBlankTexture()
        {
            var blankTexture = new Texture2D(32, 32, TextureFormat.RGB24, false);
            blankTexture.SetPixels(0, 0, 32, 32, new Color[1024]);
            return blankTexture;
        }
    }
}