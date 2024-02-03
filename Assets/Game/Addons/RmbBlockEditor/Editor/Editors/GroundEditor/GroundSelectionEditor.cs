// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class GroundSelectionEditor
    {
        private readonly VisualElement visualElement;
        private readonly Ground ground;
        private GroundGridElement gridElement;
        private Vector2 selectedTileCoords;

        public GroundSelectionEditor(GameObject go)
        {
            ground = go.GetComponent<Ground>();
            visualElement = new VisualElement();
            selectedTileCoords = new Vector2(0, 0);
        }

        public VisualElement Render()
        {
            visualElement.Clear();
            RenderTemplate();
            RenderTileGrid();
            RenderPreview();
            RenderControls();
            BindCenterCameraButton();
            return visualElement;
        }

        private void RenderTemplate()
        {
            var rmbPropsTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/GroundEditor/GroundSelectionTemplate.uxml");
            visualElement.Add(rmbPropsTree.CloneTree());
        }

        private void RenderTileGrid()
        {
            var gridContainer = visualElement.Query<VisualElement>("terrain-grid").First();
            gridElement = new GroundGridElement(ground.groundTiles);
            gridElement.OnSelection = (coords) =>
            {
                selectedTileCoords = coords;
                RenderPreview();
                return coords;
            };
            gridContainer.Add(gridElement);
        }

        private void RenderPreview()
        {
            var preview = visualElement.Query<VisualElement>("ground-selected-tile-preview").First();
            preview.Clear();
            var tile = ground.groundTiles[(int)selectedTileCoords.y, (int)selectedTileCoords.x];
            var texture = GroundGridElement.GetGroundTexture(tile.TextureRecord);
            var rotation = GroundHelper.GetRotationDeg(tile.IsRotated, tile.IsFlipped);

            // I found no way to rotate the background of a VisualElement so I have to use IMGUI here
            var previewGUI = new IMGUIContainer(() =>
            {
                var tileRect = new Rect(0, 0, 256, 256);
                GUIUtility.RotateAroundPivot(rotation, new UnityEngine.Vector2(128.0f, 128.0f));
                GUI.DrawTexture(tileRect, texture, ScaleMode.ScaleToFit);
            });
            preview.Add(previewGUI);
        }

        private void RenderControls()
        {
            RenderPreview();

            // Handle rotation
            var rotateCCW = visualElement.Query<Button>("ground-selected-tile-rotate-ccw").First();
            var rotateCW = visualElement.Query<Button>("ground-selected-tile-rotate-cw").First();
            rotateCCW.RegisterCallback<MouseUpEvent>(evt =>
            {
                gridElement.RotateSelected(-90);
                RenderPreview();
            }, TrickleDown.TrickleDown);
            rotateCW.RegisterCallback<MouseUpEvent>(evt =>
            {
                gridElement.RotateSelected(90);
                RenderPreview();
            }, TrickleDown.TrickleDown);

            // Handle TextureRecord change
            var textureRecord = visualElement.Query<IntegerField>("ground-selected-tile-record").First();
            var previousRecord = visualElement.Query<Button>("ground-selected-tile-record-decrement").First();
            var nextRecord = visualElement.Query<Button>("ground-selected-tile-record-increment").First();
            textureRecord.value =
                ground.groundTiles[(int)selectedTileCoords.y, (int)selectedTileCoords.x].TextureRecord;

            textureRecord.RegisterCallback<BlurEvent>(evt =>
            {
                if (textureRecord.value != ground.groundTiles[(int)selectedTileCoords.y, (int)selectedTileCoords.x]
                        .TextureRecord)
                {
                    if (textureRecord.value != GroundGridElement.BlankTextureRecord && textureRecord.value < 0)
                    {
                        textureRecord.value = 0;
                    }
                    else if (textureRecord.value != GroundGridElement.BlankTextureRecord &&
                             textureRecord.value > 55)
                    {
                        textureRecord.value = 55;
                    }

                    gridElement.ChangeSelectedRecord(textureRecord.value);
                    RenderPreview();
                }
            }, TrickleDown.TrickleDown);
            previousRecord.RegisterCallback<MouseUpEvent>(evt =>
            {
                var previousRecordVal =
                    ground.groundTiles[(int)selectedTileCoords.y, (int)selectedTileCoords.x].TextureRecord - 1;
                if (previousRecordVal < 0)
                {
                    previousRecordVal = GroundGridElement.BlankTextureRecord;
                }
                else if (previousRecordVal > 55 && previousRecordVal < GroundGridElement.BlankTextureRecord)
                {
                    previousRecordVal = 55;
                }

                gridElement.ChangeSelectedRecord(previousRecordVal);
                textureRecord.value = previousRecordVal;
                RenderPreview();
            }, TrickleDown.TrickleDown);

            nextRecord.RegisterCallback<MouseUpEvent>(evt =>
            {
                var nextRecordVal =
                    ground.groundTiles[(int)selectedTileCoords.y, (int)selectedTileCoords.x].TextureRecord + 1;

                if (nextRecordVal > GroundGridElement.BlankTextureRecord)
                {
                    nextRecordVal = 0;
                } else if (nextRecordVal > 55)
                {
                    nextRecordVal = GroundGridElement.BlankTextureRecord;
                }

                gridElement.ChangeSelectedRecord(nextRecordVal);
                textureRecord.value = nextRecordVal;
                RenderPreview();
            }, TrickleDown.TrickleDown);
        }

        private void BindCenterCameraButton()
        {
            var centerCamera = visualElement.Query<Button>("center-camera").First();
            centerCamera.RegisterCallback<MouseUpEvent>(evt => CameraHelper.CenterCamera());
        }
    }
}