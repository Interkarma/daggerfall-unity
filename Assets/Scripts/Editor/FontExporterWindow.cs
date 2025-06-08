// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using UnityEditor;
using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Editor window to view and export Daggerfall .FNT font files.
    /// </summary>
    public class FontExporterWindow : EditorWindow
    {
        const string windowTitle = "Font Exporter";
        const string menuPath = "Daggerfall Tools/Font Exporter [Beta]";

        public FontSelection fontSelection = FontSelection.FONT0001;
        public Color BackgroundColor = Color.clear;
        public Color TextColor = Color.white;
        public FilterMode TextureFilterMode = FilterMode.Bilinear;
        public int CharacterSpacing = 1;
        public string ResourcesPath = "Assets/Resources/";

        DaggerfallUnity dfUnity;
        FntFile fntFile = new FntFile();
        Texture2D fontPreviewTexture;
        FontSelection lastFontSelection = (FontSelection)(-1);
        Color lastBackgroundColor = Color.clear;
        Color lastTextColor = Color.white;
        FilterMode lastFilterMode = FilterMode.Bilinear;

        public enum FontSelection
        {
            FONT0000,
            FONT0001,
            FONT0002,
            FONT0003,
            FONT0004,
        }

        public string FontName
        {
            get { return fontSelection.ToString(); }
        }

        [MenuItem(menuPath)]
        static void Init()
        {
            FontExporterWindow window = (FontExporterWindow)EditorWindow.GetWindow(typeof(FontExporterWindow));
            window.titleContent = new GUIContent(windowTitle);
        }

        void OnGUI()
        {
            if (!IsReady())
            {
                EditorGUILayout.HelpBox("DaggerfallUnity instance not ready. Have you set your Arena2 path?", MessageType.Info);
                return;
            }

            // Load font when changed or not loaded
            if (lastFontSelection != fontSelection ||
                lastBackgroundColor != BackgroundColor ||
                lastTextColor != TextColor ||
                lastFilterMode != TextureFilterMode ||
                !fntFile.IsLoaded)
            {
                LoadFont();
                UpdateFontPreview();
            }

            // Exit if font not loaded for some reason
            if (!fntFile.IsLoaded || !fontPreviewTexture)
            {
                EditorGUILayout.HelpBox("Could not load font.", MessageType.Info);
                return;
            }

            // Font selection
            EditorGUILayout.Space();
            fontSelection = (FontSelection)EditorGUILayout.EnumPopup(new GUIContent("Font", "Select one of the supported Daggerfall fonts."), (FontSelection)fontSelection);

            // Font colours
            BackgroundColor = EditorGUILayout.ColorField(new GUIContent("Background Color", "Background color of font."), BackgroundColor);
            TextColor = EditorGUILayout.ColorField(new GUIContent("Text Color", "Text color of font."), TextColor);

            // Preview filtering
            TextureFilterMode = (FilterMode)EditorGUILayout.EnumPopup(new GUIContent("Filter Mode", "FilterMode of generated texture."), (FilterMode)TextureFilterMode);

            // Character spacing
            CharacterSpacing = EditorGUILayout.IntSlider(new GUIContent("Character Spacing", "Number of pixels between characters."), CharacterSpacing, 0, 4);

            // Resources path
            ResourcesPath = EditorGUILayout.TextField(new GUIContent("Resources Path", "Target Resources path. Must be a Resources folder."), ResourcesPath.Trim());

            // Buttons
            GUILayoutHelper.Horizontal(() =>
            {
                if (GUILayout.Button("Reset Colors"))
                {
                    BackgroundColor = Color.clear;
                    TextColor = Color.white;
                }
                if (GUILayout.Button("Generate Custom Font"))
                {
                    Texture2D fontAtlas;
                    Rect[] fontRects;
                    if (SaveFontTextureAsset(this.FontName, out fontAtlas, out fontRects))
                    {
                        Font font;
                        Material material;
                        SaveOtherFontAssets(this.FontName, fontAtlas, out font, out material);
                        //ImportFontSettings(fntFile,font, fontRects, CharacterSpacing);
                    }
                }
            });

            // Note for exporters
            string path = Path.Combine(ResourcesPath, this.FontName);
            EditorGUILayout.HelpBox(string.Format("Font will be saved to {0}...", path), MessageType.Info);

            // Font preview
            EditorGUILayout.Space();
            DrawFontPreview();
        }

        void DrawFontPreview()
        {
            // Exit if texture not present
            if (!fontPreviewTexture)
                return;

            // Draw scaled preview texture
            int height = (int)(((float)Screen.width / (float)fontPreviewTexture.width) * (float)fontPreviewTexture.height);
            Rect previewRect = EditorGUILayout.GetControlRect(false, height);
            GUI.DrawTextureWithTexCoords(previewRect, fontPreviewTexture, new Rect(0, 0, 1, -1));
        }

        #region Private Methods

        bool IsReady()
        {
            if (!dfUnity)
                dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return false;

            return true;
        }

        void LoadFont()
        {
            string filename = fontSelection.ToString() + ".FNT";
            if (fntFile.Load(Path.Combine(dfUnity.Arena2Path, filename), FileUsage.UseMemory, true))
            {
                lastFontSelection = fontSelection;
            }
        }

        void UpdateFontPreview()
        {
            // Get font atlas for preview texture
            Rect[] rects;
            ImageProcessing.CreateFontAtlas(fntFile, BackgroundColor, TextColor, out fontPreviewTexture, out rects);
            fontPreviewTexture.filterMode = this.TextureFilterMode;

            // Update last settings
            lastBackgroundColor = BackgroundColor;
            lastTextColor = TextColor;
            lastFilterMode = TextureFilterMode;
        }

        #endregion

        #region Font Saving

        bool SaveFontTextureAsset(string fontName, out Texture2D fontAtlas, out Rect[] fontRects)
        {
            string assetPath = Path.Combine(ResourcesPath, fontName);
            string filePath = assetPath + ".png";

            // Get font atlas
            ImageProcessing.CreateFontAtlas(fntFile, BackgroundColor, TextColor, out fontAtlas, out fontRects);

            // Save atlas texture
            byte[] fontAtlasPNG = fontAtlas.EncodeToPNG();
            File.WriteAllBytes(filePath, fontAtlasPNG);

            // Loading back asset to modify importer properties
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            fontAtlas = Resources.Load<Texture2D>(fontName);
            string assetTexturePath = AssetDatabase.GetAssetPath(fontAtlas);

            // Modify asset importer properties
            TextureImporter importer = AssetImporter.GetAtPath(assetTexturePath) as TextureImporter;
            if (importer == null)
            {
                DaggerfallUnity.LogMessage("FontGeneratorWindow: Failed to get TextureImporter. Ensure your target folder is called 'Resources'.", true);
                return false;
            }
            importer.textureType = TextureImporterType.Default;
            importer.maxTextureSize = 256;
            importer.mipmapEnabled = false;
            importer.isReadable = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.filterMode = this.TextureFilterMode;

            // Reimport asset with new importer settings
            AssetDatabase.ImportAsset(assetTexturePath, ImportAssetOptions.ForceUpdate);

            // Finish up
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return true;
        }

        void SaveOtherFontAssets(string fontName, Texture2D fontAtlas, out Font fontOut, out Material materialOut)
        {
            string assetPath = Path.Combine(ResourcesPath, fontName);
            string fontPath = assetPath + ".fontsettings";
            string materialPath = assetPath + ".mat";

            fontOut = new Font();
            AssetDatabase.CreateAsset(fontOut, fontPath);

            materialOut = new Material(Shader.Find("Unlit/Transparent"));
            materialOut.SetTexture("_MainTex", fontAtlas);
            AssetDatabase.CreateAsset(materialOut, materialPath);
            fontOut.material = materialOut;
        }

        //public void ImportFontSettings(FntFile fntFile, Font font, Rect[] fontRects, int spacing)
        //{
        //    const int asciiOffset = 32;

        //    List<CharacterInfo> infoList = new List<CharacterInfo>();

        //    // Add missing space character
        //    CharacterInfo space = new CharacterInfo();
        //    space.width = fntFile.FixedWidth / 2;
        //    infoList.Add(space);

        //    // Add Daggerfall characters
        //    for (int i = 0; i < FntFile.MaxGlyphCount; i++)
        //    {
        //        int width = fntFile.GetGlyphWidth(i);
        //        int height = fntFile.FixedHeight;

        //        CharacterInfo info = new CharacterInfo();
        //        info.uv = fontRects[i];
        //        info.vert.x = 0;
        //        info.vert.y = 0;
        //        info.vert.width = width;
        //        info.vert.height = -height;
        //        info.width = width + spacing;
        //        info.index = infoList.Count;
        //        infoList.Add(info);
        //    }

        //    this.SetAsciiStartOffset(font, asciiOffset);
        //    font.characterInfo = infoList.ToArray();
        //}

        public void SetAsciiStartOffset(Font font, int asciiStartOffset)
        {
            Editor editor = Editor.CreateEditor(font);

            SerializedProperty startOffsetProperty = editor.serializedObject.FindProperty("m_AsciiStartOffset");
            startOffsetProperty.intValue = asciiStartOffset;

            editor.serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}