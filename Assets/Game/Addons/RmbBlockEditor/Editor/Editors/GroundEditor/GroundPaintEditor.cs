// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class GroundPaintEditor
    {
        private readonly VisualElement visualElement;
        private readonly Ground ground;
        private GroundGridElement gridElement;
        private int paintSelection;
        private int paintRotation;

        public GroundPaintEditor(GameObject go)
        {
            ground = go.GetComponent<Ground>();
            visualElement = new VisualElement();
            paintSelection = 0;
            paintRotation = 0;
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
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/GroundEditor/GroundPaintTemplate.uxml");
            visualElement.Add(rmbPropsTree.CloneTree());
        }

        private void RenderTileGrid()
        {
            var gridContainer = visualElement.Query<VisualElement>("terrain-grid").First();
            gridElement = new GroundGridElement(ground.groundTiles, paintSelection, paintRotation);
            gridContainer.Add(gridElement);
        }

        private void RenderPreview()
        {
            var preview = visualElement.Query<VisualElement>("ground-paint-tile-preview").First();
            preview.Clear();
            var texture = GroundGridElement.GetGroundTexture(paintSelection);

            // I found no way to rotate the background of a VisualElement so I have to use IMGUI here
            var previewGUI = new IMGUIContainer(() =>
            {
                var tileRect = new Rect(0, 0, 256, 256);
                GUIUtility.RotateAroundPivot(paintRotation, new UnityEngine.Vector2(128.0f, 128.0f));
                GUI.DrawTexture(tileRect, texture, ScaleMode.ScaleToFit);
            });
            preview.Add(previewGUI);
        }

        private void RenderControls()
        {
            var paletteContainer = visualElement.Query<VisualElement>("ground-paint-palette").First();
            var preview = visualElement.Query<VisualElement>("ground-paint-tile-preview").First();
            var rotateCCW = visualElement.Query<Button>("ground-paint-rotate-ccw").First();
            var rotateCW = visualElement.Query<Button>("ground-paint-rotate-cw").First();
            paletteContainer.Clear();
            preview.Clear();

            // Render the palette
            for (var i = 0; i < 8; i++)
            {
                var row = new VisualElement();
                row.AddToClassList("rmb-palette-row");
                for (var j = 0; j < 7; j++)
                {
                    var index = i * 7 + j;
                    var cell = RenderCell(index);
                    cell.userData = index;
                    cell.RegisterCallback<MouseDownEvent>(HandlePaletteSelect, TrickleDown.TrickleDown);
                    row.Add(cell);
                }

                paletteContainer.Add(row);
            }

            // Add a cell for a "blank" texture
            var lastRow = new VisualElement();
            lastRow.AddToClassList("rmb-palette-row");
            var blankCell = RenderCell(GroundGridElement.BlankTextureRecord);
            blankCell.userData = GroundGridElement.BlankTextureRecord;
            blankCell.RegisterCallback<MouseDownEvent>(HandlePaletteSelect, TrickleDown.TrickleDown);
            lastRow.Add(blankCell);
            paletteContainer.Add(lastRow);

            // Handle rotation
            rotateCCW.RegisterCallback<MouseUpEvent>(evt =>
            {
                paintRotation -= 90;
                // make sure rotation is positive
                if (paintRotation < 0)
                {
                    paintRotation = 270;
                }

                gridElement.SetPaintRotation(paintRotation);
            }, TrickleDown.TrickleDown);
            rotateCW.RegisterCallback<MouseUpEvent>(evt =>
            {
                paintRotation += 90;
                if (paintRotation > 270)
                {
                    paintRotation = 0;
                }

                gridElement.SetPaintRotation(paintRotation);
            }, TrickleDown.TrickleDown);
        }

        private void BindCenterCameraButton()
        {
            var centerCamera = visualElement.Query<Button>("center-camera").First();
            centerCamera.RegisterCallback<MouseUpEvent>(evt => CameraHelper.CenterCamera());
        }

        private IMGUIContainer RenderCell(int index)
        {
            var texture = GroundGridElement.GetGroundTexture(index);
            var cell = new IMGUIContainer(() =>
            {
                var cellIsSelected = index == paintSelection;
                var style = new GUIStyle();
                style.padding = new RectOffset(2, 2, 2, 2);

                var tileRect = new Rect(0, 0, 36, 36);

                GUIUtility.RotateAroundPivot(paintRotation, new Vector2(17.0f, 17.0f));
                if (cellIsSelected)
                {
                    GUI.color = Color.red;
                }

                GUI.Button(tileRect, texture, style);
            });
            return cell;
        }

        private void HandlePaletteSelect(MouseDownEvent evt)
        {
            var element = evt.target as VisualElement;
            paintSelection = (int)element.userData;
            gridElement.SetPaintArchive(paintSelection);
            RenderPreview();
        }
    }
}