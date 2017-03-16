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
        const string extension = ".xml";

        #region Public Methods

        /// <summary>
        /// Search for xml file for specified texture.
        /// </summary>
        static public bool XmlFileExist(int archive, int record, int frame = 0)
        {
            return XmlFileExist(TextureReplacement.GetName(archive, record, frame), TextureReplacement.texturesPath);
        }

        /// <summary>
        /// Search for xml file.
        /// </summary>
        /// <param name="fileName">Name of file without extension.</param>
        /// <param name="path">Path.</param>
        /// <returns></returns>
        static public bool XmlFileExist(string fileName, string path)
        {
            path = Path.Combine(path, fileName);
            return File.Exists(path + extension);
        }

        /// <summary>
        /// Get a generic element from xml.
        /// Return null if is not found.
        /// </summary>
        static public XElement GetElement(string fileName, string key, string path)
        {
            // Get XML file
            XElement xml = GetXmlFile(fileName, path);
            if (xml == null)
                return null;

            // Get Element 
            return xml.Element(key);
        }


        /// <summary>
        /// Get a float value from xml.
        /// </summary>
        /// <param name="defaultValue">Fallback value.</param>
        /// <returns>Float from xml or fallback.</returns>
        static public float GetFloat(string fileName, string key, string path, float defaultValue = 0)
        {
            var element = GetElement(fileName, key, path);
            if (element != null)
                return (float)element;
            else
                return defaultValue;
        }

        /// <summary>
        /// Try to get a float value from xml.
        /// </summary>
        /// <returns>True if key is found.</returns>
        static public bool TryGetFloat(string fileName, string key, out float value, string path)
        {
            var element = GetElement(fileName, key, path);
            if (element != null)
            {
                value = (float)element;
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        /// <summary>
        /// Get a string from xml.
        /// </summary>
        /// <param name="defaultString">Fallback string.</param>
        /// <returns>String from xml or fallback.</returns>
        static public string GetString(string fileName, string key, string path, string defaultString = "")
        {
            var element = GetElement(fileName, key, path);
            if (element != null)
                return (string)element;
            else
                return defaultString;
        }

        /// <summary>
        /// Try to get a string from xml.
        /// </summary>
        /// <returns>True if key is found.</returns>
        static public bool TryGetString(string fileName, string key, out string value, string path)
        {
            var element = GetElement(fileName, key, path);
            if (element != null)
            {
                value = (string)element;
                return true;
            }
            else
            {
                value = "";
                return false;
            }
        }

        /// <summary>
        /// Get value from Xml file on disk
        /// This is useful to get additional informations for custom textures.
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
            string originalName = fileName;
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
            
            // If missing, try to get value from Daggerfall vanilla texture
            if (isImage)
            {
                ImageData imageData = ImageReader.GetImageData(fileName, createTexture: false);
                if (valueName == "width")
                    return imageData.width;
                else if (valueName == "height")
                    return imageData.height;
            }
            else if (isCif)
            {
                ImageData imageData = ImageReader.GetImageData(originalName, record, createTexture: false);
                if (valueName == "width")
                    return imageData.width;
                else if (valueName == "height")
                    return imageData.height;
            }
             
            // If missing, try to get value from custom texture
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

        /// <summary>
        /// Get float value from Xml file on disk.
        /// This is useful to import rgba values.
        /// </summary>
        /// <param name="fileName">Name of texture. Correspond to the name of Xml file</param>
        /// <param name="valueName">Name of value to be searched</param>
        /// <returns>Float value</returns>
        static public float GetColorValue(string fileName, string valueName)
        {
            // Get path
            string path = Path.Combine(TextureReplacement.texturesPath, fileName);

            // Get value from xml
            if (File.Exists(path + ".xml"))
            {
                XElement xml = XElement.Load(path + ".xml");
                return (float)xml.Element(valueName);
            }

            Debug.LogError(path + ".xml is missing");
            return 0;
        }

        /// <summary>
        /// Create a new color from rgba value on xml file.
        /// Default color will be used if xml file contains invalid data.
        /// Alpha channel is set to A value from default color if omitted.
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        /// <param name="path">Path.</param>
        /// <param name="defaultColor">Fallback color.</param>
        /// <returns>Color from xml or fallback.</returns>
        static public Color GetColor(string fileName, string path, Color defaultColor)
        {
            return (GetColor(fileName, path, defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a));
        }

        /// <summary>
        /// Create a new color from rgba value on xml file.
        /// Alpha channel is set to full opacity if omitted.
        /// Black color is used as fallback if xml file contains invalid data
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        /// <param name="path">Path.</param>
        /// <returns>Color from xml or fallback.</returns>
        static public Color GetColor(string fileName, string path, float R = 0, float G = 0, float B = 0, float A = 1)
        {
            // Fields
            var defaultColor = new Color(R, G, B, A);
            XElement r, g, b, a;
            float alpha;
            
            // Get XML file
            XElement xml = GetXmlFile(fileName, path);
            if (xml == null)
                return defaultColor;

            // Alpha channel
            a = xml.Element("a");
            if (a != null)
                alpha = (float)a;
            else
                alpha = defaultColor.a;

            // Colors
            r = xml.Element("r");
            if (r != null)
            {
                g = xml.Element("g");
                if (g != null)
                {
                    b = xml.Element("b");
                    if (g != null)
                        return new Color((float)r, (float)g, (float)b, alpha);
                }
            }

            Debug.LogError("Failed to read color from " + Path.Combine(path, fileName + extension));
            return defaultColor;
        }

        /// <summary>
        /// Get (x,y) scale from Xml file,
        /// setting a fallback scale.
        /// Z is set from Z value of fallback.
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        /// <param name="path">Path.</param>
        /// <param name="defaultScale">Fallback scale.</param>
        /// <returns>Scale from xml or fallback.</returns>
        static public Vector3 GetScale(string fileName, string path, Vector3 defaultScale)
        {
            return GetScale(fileName, path, defaultScale.x, defaultScale.y, defaultScale.z);
        }

        /// <summary>
        /// Get (x,y) scale from Xml file.
        /// Z is set to 1;
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        /// <param name="path">Path.</param>
        /// <returns>Scale from xml or fallback.</returns>
        static public Vector3 GetScale (string fileName, string path, float defaultX = 1, float defaultY = 1, float defaultZ = 1)
        {
            // Fields
            XElement X, Y;
            float x, y;

            // Get XML file
            XElement xml = GetXmlFile(fileName, path);
            if (xml == null)
                return new Vector3(defaultX, defaultY, defaultZ);

            // Get X
            X = xml.Element("scaleX");
            if (X != null)
                x = (float)X;
            else
                x = defaultX;

            // Get Y
            Y = xml.Element("scaleY");
            if (Y != null)
                y = (float)Y;
            else
                y = defaultY;

            return new Vector3(x, y, defaultZ);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get XML file.
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        /// <param name="path">Path.</param>
        /// <returns>File as XElement.</returns>
        static private XElement GetXmlFile(string fileName, string path)
        {
            // Get path
            path = Path.Combine(path, fileName);

            // Get XML file
            if (File.Exists(path + extension))
            {
                return XElement.Load(path + extension);
            }
            else
            {
                Debug.LogError(path + extension + " is missing");
                return null;
            }
        }

        #endregion
    }
}
