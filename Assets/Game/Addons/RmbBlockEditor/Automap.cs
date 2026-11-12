// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    public class Automap : MonoBehaviour
    {
        public Byte[] automapData;
        private MeshRenderer renderer;

        public void CreateObject(Byte[] automapData)
        {
            this.automapData = automapData;
            var groundPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            groundPlane.name = "plane";
            groundPlane.transform.parent = transform;
            groundPlane.transform.localScale = new Vector3(10.24f, 1f, 10.24f);
            groundPlane.transform.position = new Vector3(51.2f, 0, -51.2f);
            groundPlane.transform.rotation = Quaternion.Euler(0, 0, 0);
            renderer = groundPlane.GetComponent<MeshRenderer>();
            Update();
        }

        public void Update()
        {
            Texture2D tex = new Texture2D(64, 64, TextureFormat.ARGB32, false);
            Color32[] colors = new Color32[64 * 64];
            for (var i = 0; i < automapData.Length; i++)
            {
                var lineMax = (Math.Floor(i / 64f) + 1) * 64;
                var previousLineMax = Math.Floor(i / 64f) * 64;
                var colorIndex = (int)(lineMax - i - 1 + previousLineMax);
                switch (automapData[i])
                {
                    // guilds
                    case 12: // guildhall
                    case 15: // temple
                        colors[colorIndex] = DaggerfallUnity.Settings.AutomapTempleColor;
                        break;
                    // shops
                    case 1: // alchemist
                    case 3: // armorer
                    case 4: // bank
                    case 6: // bookseller
                    case 7: // clothing store
                    case 9: // gem store
                    case 10: // general store
                    case 11: // library
                    case 13: // pawn shop
                    case 14: // weapon smith
                        colors[colorIndex] = DaggerfallUnity.Settings.AutomapShopColor;
                        break;
                    case 16: // tavern
                        colors[colorIndex] = DaggerfallUnity.Settings.AutomapTavernColor;
                        break;
                    // common
                    case 2: // house for sale
                    case 5: // town4
                    case 8: // furniture store
                    case 17: // palace
                    case 18: // house 1
                    case 19: // house 2
                    case 20: // house 3
                    case 21: // house 4
                    case 22: // house 5 (hedge)
                    case 23: // house 6
                    case 24: // town23
                        colors[colorIndex] = DaggerfallUnity.Settings.AutomapHouseColor;
                        break;
                    case 25: // ship
                    case 117: // special 1
                    case 224: // special 2
                    case 250: // special 3
                    case 251: // special 4
                        break;
                    case 0:
                        colors[colorIndex].r = 0;
                        colors[colorIndex].g = 0;
                        colors[colorIndex].b = 0;
                        colors[colorIndex].a = 0;
                        break;
                    default: // unknown
                        colors[colorIndex].r = 255;
                        colors[colorIndex].g = 0;
                        colors[colorIndex].b = automapData[i];
                        colors[colorIndex].a = 255;
                        break;
                }
            }

            tex.SetPixels32(0, 0, 64, 64, colors);
            tex.Apply();
            renderer.sharedMaterial.mainTexture = tex;
        }
    }
    #endif
}