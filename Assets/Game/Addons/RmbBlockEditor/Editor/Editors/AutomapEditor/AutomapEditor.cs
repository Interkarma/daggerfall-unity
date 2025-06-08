// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using DaggerfallConnect;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class AutomapEditor
    {
        public VisualElement visualElement;
        public Automap automap;
        private GameObject gameObject;
        private byte paletteSelection;

        public AutomapEditor(GameObject go)
        {
            gameObject = go;
            automap = gameObject.GetComponent<Automap>();
            visualElement = new VisualElement();
        }

        public VisualElement Render()
        {
            visualElement.Clear();
            RenderTemplate();
            RegisterButtonCallbacks();
            RenderPalette();
            BindPaletteCells();
            RenderGrid();
            UnbindGridCells();
            return visualElement;
        }

        private void RenderTemplate()
        {
            var tree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/AutomapEditor/Template.uxml");
            visualElement.Add(tree.CloneTree());
        }

        private void RegisterButtonCallbacks()
        {
            // Automap
            var generateAutomapButton = visualElement.Query<Button>("generate").First();
            generateAutomapButton.RegisterCallback<MouseUpEvent>(evt => GenerateAutomap(), TrickleDown.TrickleDown);
        }

        private void RenderPalette()
        {
            var palette = visualElement.Query<VisualElement>(null, "automap-palette-cell");
            palette.ForEach((cell) =>
            {
                if (cell.name == "palette-" + paletteSelection)
                {
                    cell.AddToClassList("selected");
                }
                else
                {
                    cell.RemoveFromClassList("selected");
                }
            });
        }

        private void BindPaletteCells()
        {
            var palette = visualElement.Query<VisualElement>(null, "automap-palette-cell");
            palette.ForEach((cell) =>
            {
                cell.RegisterCallback<MouseUpEvent>((evt) =>
                {
                    var item = (VisualElement)evt.target;
                    var cellName = item.name;
                    paletteSelection = byte.Parse(cellName.Replace("palette-", ""));
                    RenderPalette();
                }, TrickleDown.TrickleDown);
            });
        }

        private void RenderGrid()
        {
            UnbindGridCells();
            var gridContainer = visualElement.Query<VisualElement>("grid").First();
            gridContainer.Clear();
            var data = automap.automapData;

            for (var i = 0; i < 64; i++)
            {
                var row = new VisualElement();
                row.AddToClassList("row");
                row.AddToClassList("center");
                for (var j = 0; j < 64; j++)
                {
                    var cellIndex = i * 64 + j;
                    var type = data[cellIndex];
                    var color = GetColor(type);
                    var cell = new VisualElement();
                    cell.userData = cellIndex;
                    cell.AddToClassList("automap-cell");
                    cell.AddToClassList(color);
                    cell.tooltip = Enum.GetName(typeof(DFLocation.BuildingTypes), type - 1);
                    cell.RegisterCallback<MouseMoveEvent>(OnDragPaint, TrickleDown.TrickleDown);
                    cell.RegisterCallback<MouseDownEvent>(OnClickPaint, TrickleDown.TrickleDown);
                    row.Add(cell);
                }

                gridContainer.Add(row);
            }
        }

        private void UnbindGridCells()
        {
            var cells = visualElement.Query<VisualElement>("automap-cell");
            cells.ForEach((cell) =>
            {
                cell.UnregisterCallback<MouseMoveEvent>(OnDragPaint);
                cell.UnregisterCallback<MouseDownEvent>(OnClickPaint);
            });
        }

        private void OnClickPaint(MouseDownEvent evt)
        {
            if (evt.pressedButtons != 1)
            {
                return;
            }

            var element = evt.target as VisualElement;
            OnPaint(element);
        }

        private void OnDragPaint(MouseMoveEvent evt)
        {
            if (evt.pressedButtons != 1)
            {
                return;
            }

            var element = evt.target as VisualElement;
            OnPaint(element);
        }

        private void OnPaint(VisualElement element)
        {
            var index = (int)element.userData;
            var newColor = GetColor(paletteSelection);
            var oldColor = GetColor(automap.automapData[index]);
            automap.automapData[index] = paletteSelection;
            element.RemoveFromClassList(oldColor);
            element.AddToClassList(newColor);
        }

        private void GenerateAutomap()
        {
            Byte[] map = new byte[64 * 64];

            for (var i = 0; i < 64; i++)
            {
                for (var j = 0; j < 64; j++)
                {
                    var mapIndex = i * 64 + j;
                    var rayPos = new Vector3(j * 1.6f + 0.8f, 100, -(i * 1.6f + 0.8f));
                    RaycastHit hit;
                    if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
                    {
                        var building = hit.collider.gameObject.GetComponentInParent<Building>();
                        var misc3d = hit.collider.gameObject.GetComponentInParent<Misc3d>();
                        if (building != null)
                        {
                            map[mapIndex] = (byte)(building.BuildingType + 1);
                        }
                        else if (misc3d != null)
                        {
                            map[mapIndex] = 117;
                        }
                        else
                        {
                            map[mapIndex] = 0;
                        }
                    }
                }
            }

            automap.automapData = map;
            automap.Update();
            RenderGrid();
        }

        private string GetColor(byte buildingType)
        {
            switch (buildingType)
            {
                case 0:
                    return "black";
                case 16:
                    return "green";
                case 12:
                case 15:
                    return "blue";
                case 1:
                case 3:
                case 4:
                case 6:
                case 7:
                case 9:
                case 10:
                case 11:
                case 13:
                case 14:
                    return "orange";
                case 2:
                case 5:
                case 8:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                    return "tan";
                case 117:
                case 224:
                case 250:
                case 251:
                    return "gray";
                default:
                    return "magenta";
            }
        }
    }
}