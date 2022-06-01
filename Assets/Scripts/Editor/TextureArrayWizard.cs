// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    [Serializable]
    public class TextureArrayWizardOptions
    {
        [Tooltip("Keep the texture readable.")]
        public bool ReadWriteEnabled = false;

        [Tooltip("Wrap mode for the texture array.")]
        public TextureWrapMode WrapMode = TextureWrapMode.Clamp;

        [Tooltip("Filtermode for the texture array.")]
        public FilterMode FilterMode = FilterMode.Bilinear;

        [Tooltip("Anisotropic filtering level for the texture array.")]
        [Range(0, 16)]
        public int AnisoLevel = 1;
    }

    /// <summary>
    /// A basic wizard for texture array creation.
    /// </summary>
    public sealed class TextureArrayWizard : ScriptableWizard
    {
        [Tooltip("The directory where the texture array is created.")]
        public string Directory;

        [Tooltip("The name of the texture array asset.")]
        public string Name = "Example-TexArray";

        [Space]

        public TextureArrayWizardOptions Advanced;

        [Space]

        [Tooltip("Textures to be included in the array in the given order.")]
        public Texture2D[] Textures;

        [MenuItem("Daggerfall Tools/Texture Array Creator")]
        static void CreateWizard()
        {
            DisplayWizard<TextureArrayWizard>("Create Texture Array", "Create", "Add from selection");
        }

        void OnWizardCreate()
        {
            if (string.IsNullOrEmpty(Name) || Textures == null || Textures.Length == 0)
            {
                Debug.LogError("No name or textures assigned to texture array.");
                return;
            }

            if (!SystemInfo.supports2DArrayTextures)
            {
                Debug.LogError("Array textures are not supported!");
                return;
            }

            Texture2D first = Textures[0];
            Texture2DArray textureArray = new Texture2DArray(first.width, first.height, Textures.Length, first.format, first.mipmapCount > 1);

            if (!Advanced.ReadWriteEnabled && (SystemInfo.copyTextureSupport & CopyTextureSupport.DifferentTypes) == CopyTextureSupport.DifferentTypes)
            {
                textureArray.Apply(false, true);

                for (int i = 0; i < Textures.Length; i++)
                    Graphics.CopyTexture(Textures[i], 0, textureArray, i);
            }
            else
            {
                for (int i = 0; i < Textures.Length; i++)
                    textureArray.SetPixels32(Textures[i].GetPixels32(), i);

                textureArray.Apply(true, !Advanced.ReadWriteEnabled);
            }

            textureArray.filterMode = Advanced.FilterMode;
            textureArray.anisoLevel = Advanced.AnisoLevel;
            textureArray.wrapMode = Advanced.WrapMode;

            System.IO.Directory.CreateDirectory(Directory);
            AssetDatabase.CreateAsset(textureArray, Path.Combine(Directory, Name + ".asset"));
        }

        void OnWizardOtherButton()
        {
            var selection = Selection.objects;
            if (selection.Length == 0)
                return;

            var regex = new Regex("(?<archive>[0-9]+)_(?<record>[0-9]+)-(?<frame>[0-9]+)(_[A-Za-z]+)?");
            if (!regex.IsMatch(selection[0].name))
            {
                SetTextures(selection.Select(x => x as Texture2D));
                return;
            }

            var layers = new List<DaggerfallWorkshop.Utility.Tuple<Texture2D, DaggerfallTextureIndex>>();

            foreach (var tex in selection)
            {
                Match match = regex.Match(tex.name);
                if (match.Groups.Count == 0)
                {
                    Debug.LogErrorFormat("Failed to parse {0}", tex.name);
                    return;
                }

                layers.Add(new DaggerfallWorkshop.Utility.Tuple<Texture2D, DaggerfallTextureIndex>(tex as Texture2D, new DaggerfallTextureIndex
                {
                    archive = int.Parse(match.Groups["archive"].Value),
                    record = int.Parse(match.Groups["record"].Value),
                    frame = int.Parse(match.Groups["frame"].Value)
                }));
            }

            layers.Sort((a, b) =>
            {
                int value = a.Second.archive - b.Second.archive;
                if (value == 0)
                {
                    value = a.Second.record - b.Second.record;
                    if (value == 0)
                        value = a.Second.frame - b.Second.frame;
                }

                return value;
            });

            SetTextures(layers.Select(x => x.First));
        }

        void OnWizardUpdate()
        {
            helpString = "Create a texture array from individual textures with the same size, format and mipmaps state.";
            if (Textures != null && Textures.Length > 0)
            {
                Texture2D tex = Textures[0];
                if (tex)
                {
                    helpString += string.Format("\n\nTarget: {0}x{1} (x{2}) {3} (mipmaps {4})",
                        tex.width,
                        tex.height,
                        Textures.Length,
                        tex.format,
                        tex.mipmapCount > 0 ? "enabled" : "disabled");

                    if (string.IsNullOrEmpty(Directory))
                        Directory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(tex));
                }
            }
        }

        private void SetTextures(IEnumerable<Texture2D> textures)
        {
            Textures = textures.ToArray();
            Texture2D tex = Textures[0];
            
            Advanced.WrapMode = tex.wrapMode;
            Advanced.FilterMode = tex.filterMode;
            Advanced.AnisoLevel = tex.anisoLevel;

            string path = AssetDatabase.GetAssetPath(tex);

            var textureImporter = (TextureImporter)TextureImporter.GetAtPath(path);
            Advanced.ReadWriteEnabled = textureImporter.isReadable;

            if (string.IsNullOrEmpty(Directory))
                Directory = Path.GetDirectoryName(path);
        }
    }
}