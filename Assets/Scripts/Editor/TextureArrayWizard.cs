// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

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
            DisplayWizard<TextureArrayWizard>("Create Texture Array", "Create");
        }

        void OnWizardCreate()
        {
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

        void OnWizardUpdate()
        {
            isValid = !string.IsNullOrEmpty(Name) && Textures != null && Textures.Length > 0;

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
    }
}