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
        /// <returns>Integer value</returns>
        static public int GetValue(string fileName, string valueName, bool isImage = false, bool isCif=false)
        {
            // Get path
            string path;
            if (isImage)
                path = TextureReplacement.imgPath;
            else if (isCif)
                path = TextureReplacement.cifPath;
            else
                path = TextureReplacement.texturesPath;

            // Get value
            XElement xml = XElement.Load(Path.Combine(path, fileName + ".xml"));
            return (int)xml.Element(valueName);
        }

    }
}
