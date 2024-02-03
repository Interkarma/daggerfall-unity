// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class GroundGridElement : VisualElement
    {
        private DFBlock.RmbGroundTiles[,] tiles;
        private readonly bool isPaintMode;
        private int paintRecord;
        private int paintRotation;
        private bool isDragging;
        private Vector2 coords;
        private Vector2 paintCoords;
        public Func<Vector2, Vector2> OnSelection;
        public static int BlankTextureRecord = 63;

        public GroundGridElement(DFBlock.RmbGroundTiles[,] tiles)
        {
            this.tiles = tiles;
            isPaintMode = false;
            isDragging = false;
            RenderTemplate();
        }

        public GroundGridElement(DFBlock.RmbGroundTiles[,] tiles, int paintRecord, int paintRotation)
        {
            this.tiles = tiles;
            this.paintRecord = paintRecord;
            this.paintRotation = paintRotation;
            isPaintMode = true;
            isDragging = false;
            paintCoords = Vector2.zero;
            RenderTemplate();
        }

        public static Texture2D GetBlankTexture()
        {
            var colorArray = new Color[1024];
            for (var i = 0; i < colorArray.Length; i++)
            {
                colorArray[i] = Color.magenta;
            }

            var blankTexture = new Texture2D(32, 32, TextureFormat.RGB24, false);
            blankTexture.SetPixels(0, 0, 32, 32, colorArray);
            return blankTexture;
        }

        public static Texture2D GetGroundTexture(int record)
        {
            var climate = PersistedSettings.ClimateBases();
            var season = PersistedSettings.ClimateSeason();
            var archive = GetArchive(climate, season);
            var dfUnity = DaggerfallUnity.Instance;
            var textureReader = new TextureReader(dfUnity.Arena2Path);
            var texture = record == BlankTextureRecord
                ? GetBlankTexture()
                : textureReader.GetTexture2D(archive, record, 0, 0);
            return texture;
        }

        public void RotateSelected(int rotation)
        {
            var tile = tiles[(int)coords.y, (int)coords.x];
            var newRotation = GetRotationDeg(tile.IsRotated, tile.IsFlipped);
            newRotation += rotation;
            tiles[(int)coords.y, (int)coords.x].IsFlipped = GetIsFlipped(newRotation);
            tiles[(int)coords.y, (int)coords.x].IsRotated = GetIsRotated(newRotation);
            CalculateBitField(ref tiles[(int)coords.y, (int)coords.x]);
            RenderCell((int)coords.x, (int)coords.y);
        }

        public void SetPaintRotation(int paintRotation)
        {
            this.paintRotation = paintRotation;
        }

        public void SetPaintArchive(int paintRecord)
        {
            this.paintRecord = paintRecord;
        }

        public void ChangeSelectedRecord(int record)
        {
            if (record != BlankTextureRecord && record < 0)
            {
                record = 55;
            }
            else if (record != BlankTextureRecord && record > 55)
            {
                record = 0;
            }

            tiles[(int)coords.y, (int)coords.x].TextureRecord = record;
            CalculateBitField(ref tiles[(int)coords.y, (int)coords.x]);
            RenderCell((int)coords.x, (int)coords.y);
        }

        private void RenderTemplate()
        {
            var rmbPropsTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Elements/GroundGrid/Template.uxml");
            Add(rmbPropsTree.CloneTree());
            RenderGrid();
        }

        private void RenderGrid()
        {
            for (var i = 0; i < 16; i++)
            {
                for (var j = 0; j < 16; j++)
                {
                    RenderCell(i, j);
                }
            }
        }

        private void RenderCell(int row, int col)
        {
            var currentCoords = new Vector2(row, col);
            var tile = tiles[col, row];
            Texture2D texture;
            if (isPaintMode && isDragging && paintCoords.Equals(new Vector2(row, col)))
            {
                texture = GetGroundTexture(paintRecord);
            }
            else
            {
                texture = GetGroundTexture(tile.TextureRecord);
            }

            var cellContainer = this.Query<VisualElement>("rmb-ground-grid-cell-" + row + "-" + col).First();
            cellContainer.UnregisterCallback<MouseDownEvent>(MouseDownHandler);
            cellContainer.UnregisterCallback<MouseUpEvent>(MouseUpHandler);
            cellContainer.UnregisterCallback<MouseEnterEvent>(MouseEnterHandler);
            cellContainer.userData = currentCoords;
            cellContainer.RegisterCallback<MouseDownEvent>(MouseDownHandler, TrickleDown.TrickleDown);
            cellContainer.RegisterCallback<MouseUpEvent>(MouseUpHandler, TrickleDown.TrickleDown);
            cellContainer.RegisterCallback<MouseEnterEvent>(MouseEnterHandler, TrickleDown.TrickleDown);

            var cell = new IMGUIContainer(() =>
            {
                var cellIsSelected = coords.Equals(currentCoords);
                var rotation = (isPaintMode && isDragging && paintCoords.Equals(new Vector2(row, col)))
                    ? paintRotation
                    : GetRotationDeg(tile.IsRotated, tile.IsFlipped);

                var style = new GUIStyle();
                style.padding = new RectOffset(0, 0, 0, 0);

                var tileRect = new Rect(0, 0, 32, 32);

                GUIUtility.RotateAroundPivot(rotation, new Vector2(16.0f, 16.0f));
                if (!isPaintMode && cellIsSelected)
                {
                    GUI.color = Color.red;
                }

                GUI.Button(tileRect, texture, style);
            });
            cellContainer.Add(cell);
        }

        private void RenderCurrentCell()
        {
            var row = (int)coords.x;
            var col = (int)coords.y;
            RenderCell(row, col);
        }

        private void MouseDownHandler(MouseDownEvent evt)
        {
            if (isPaintMode)
            {
                var element = evt.target as VisualElement;
                coords = (Vector2)element.userData;
                isDragging = true;
                PaintAtCoords(coords);
            }
            else
            {
                var element = evt.target as VisualElement;
                coords = (Vector2)element.userData;
                if (OnSelection != null)
                {
                    OnSelection(coords);
                }

                RenderCurrentCell();
            }
        }

        private void MouseUpHandler(MouseUpEvent evt)
        {
            isDragging = false;
        }

        private void MouseEnterHandler(MouseEnterEvent evt)
        {
            if (!isDragging || !isPaintMode)
                return;

            var element = evt.target as VisualElement;
            coords = (Vector2)element.userData;
            PaintAtCoords(coords);
        }

        private void PaintAtCoords(Vector2 coords)
        {
            paintCoords = coords;
            tiles[(int)coords.y, (int)coords.x].TextureRecord = paintRecord;
            tiles[(int)coords.y, (int)coords.x].IsFlipped = GetIsFlipped(paintRotation);
            tiles[(int)coords.y, (int)coords.x].IsRotated = GetIsRotated(paintRotation);
            CalculateBitField(ref tiles[(int)coords.y, (int)coords.x]);
            RenderCell((int)coords.x, (int)coords.y);
        }

        private static int GetRotationDeg(bool isRotated, bool isFlipped)
        {
            var rotation = 0;

            if (isRotated)
            {
                rotation -= 90;
            }

            if (isFlipped)
            {
                rotation += 180;
            }

            if (rotation == -90)
            {
                rotation = 270;
            }

            return rotation;
        }

        private static bool GetIsRotated(int deg)
        {
            if (deg < 0)
            {
                deg += 360;
            }

            var abs = deg % 360;
            return abs == 90 || abs == 270;
        }

        private static bool GetIsFlipped(int deg)
        {
            if (deg < 0)
            {
                deg += 360;
            }

            var abs = deg % 360;
            return abs == 90 || abs == 180;
        }

        private static void CalculateBitField(ref DFBlock.RmbGroundTiles tile)
        {
            var rotatedHash = tile.IsRotated ? 64 : 0;
            var flippedHash = tile.IsFlipped ? 128 : 0;
            tile.TileBitfield = (byte)(tile.TextureRecord + rotatedHash + flippedHash);
        }

        private static int GetArchive(ClimateBases climate, ClimateSeason season)
        {
            switch (climate)
            {
                case ClimateBases.Desert:
                    return int.Parse("00" + ((int)season + 2));
                case ClimateBases.Mountain:
                    return int.Parse("10" + ((int)season + 2));
                case ClimateBases.Swamp:
                    return int.Parse("40" + ((int)season + 2));
                case ClimateBases.Temperate:
                default:
                    return int.Parse("30" + ((int)season + 2));
            }
        }
    }
}