// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:
// 
// Notes:
//

using System.IO;
using System.Xml.Linq;
using UnityEngine;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Read XML files from disk for modding purposes
    /// </summary>
    static public class XMLManager
    {
        /// <summary>
        /// Get value from Xml file on disk
        /// </summary>
        /// <param name="fileName">Name of texture. Correspond to the name of Xml file</param>
        /// <param name="valueName">Name of value to be searched</param>
        /// <param name="isImage">True if .IMG</param>
        /// <param name="isCif">True if .CIF or .RCI</param>
        /// <param name="record">Texture record</param>
        /// <returns>Integer value</returns>
        static public int GetValue(string fileName, string valueName, bool isImage = false, bool isCif = false, int record = 0)
        {
            // Get path
            string path;
            if (isImage)
                path = TextureReplacement.imgPath;
            else if (isCif)
            {
                path = TextureReplacement.cifPath;
                fileName = fileName + "_" + record + "-0";
            } 
            else
                path = TextureReplacement.texturesPath;

            path = Path.Combine(path, fileName);

            // Get value from xml
            if (File.Exists(path + ".xml"))
            {
                XElement xml = XElement.Load(path + ".xml");
                return (int)xml.Element(valueName);
            }

            // If missing, try to get value from texture
            // This may give the wanted result if texture is same resolution as vanilla
            if (valueName == "width")
            {
                Debug.Log(path + ".xml is missing, get width from " + fileName + ".png");
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(File.ReadAllBytes(path + ".png"));
                return tex.width;
            }
            if (valueName == "height")
            {
                Debug.Log(path + ".xml is missing, get height from " + fileName + ".png");
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(File.ReadAllBytes(path + ".png"));
                return tex.height;
            }

            Debug.LogError(path + ".xml is missing");
            return 0;         
        }
    }
}
