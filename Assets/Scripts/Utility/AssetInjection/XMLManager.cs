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
        #region Constants

        // Files extension
        const string extension = ".xml";

        // Keys
        const string widthKey = "width";
        const string heightKey = "height";
        const string scaleXKey = "scaleX";
        const string scaleYKey = "scaleY";
        const string UvXKey = "uvX";
        const string UvYKey = "uvY";

        #endregion

        #region Public Methods

        /// <summary>
        /// Search for xml file for specified texture.
        /// </summary>
        static public bool XmlFileExist(int archive, int record, int frame = 0)
        {
            return XmlFileExist(TextureReplacement.GetName(archive, record, frame), TextureReplacement.TexturesPath);
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
        /// Get (x,y,z) Vector3 from Xml file for a 2D scale,
        /// setting a fallback scale.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <param name="defaultScale"></param>
        /// <returns></returns>
        static public Vector3 GetScale(string fileName, string path, Vector3 defaultScale)
        {
            Vector3 scale = GetScale(fileName, path, defaultScale.x, defaultScale.y);
            scale.z = defaultScale.z;
            return scale;
        }

        /// <summary>
        /// Get (x,y) scale from Xml file,
        /// setting a fallback scale.
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        /// <param name="path">Path.</param>
        /// <param name="defaultScale">Fallback scale.</param>
        /// <returns>Scale from xml or fallback.</returns>
        static public Vector2 GetScale(string fileName, string path, Vector2 defaultScale)
        {
            return GetScale(fileName, path, defaultScale.x, defaultScale.y);
        }

        /// <summary>
        /// Get (x,y) scale from Xml file.
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        /// <param name="path">Path.</param>
        /// <returns>Scale from xml or fallback.</returns>
        static public Vector2 GetScale (string fileName, string path, float defaultX = 1, float defaultY = 1)
        {
            // Fields
            XElement X, Y;
            float x, y;

            // Get XML file
            XElement xml = GetXmlFile(fileName, path);
            if (xml == null)
                return new Vector2(defaultX, defaultY);

            // Get X
            X = xml.Element(scaleXKey);
            if (X != null)
                x = (float)X;
            else
                x = defaultX;

            // Get Y
            Y = xml.Element(scaleYKey);
            if (Y != null)
                y = (float)Y;
            else
                y = defaultY;

            return new Vector2(x, y);
        }

        /// <summary>
        /// Get Uv from Xml file.
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        /// <param name="path">Path.</param>
        /// <returns>Uv from xml or fallback.</returns>
        static public Vector2 GetUv(string fileName, string path, float defaultX = 0, float defaultY = 0)
        {
            // Fields
            XElement X, Y;
            float x, y;

            // Get XML file
            XElement xml = GetXmlFile(fileName, path);
            if (xml == null)
                return new Vector2(defaultX, defaultY);

            // Get X
            X = xml.Element(UvXKey);
            if (X != null)
                x = (float)X;
            else
                x = defaultX;

            // Get Y
            Y = xml.Element(UvYKey);
            if (Y != null)
                y = (float)Y;
            else
                y = defaultY;

            return new Vector2(x, y);
        }

        /// <summary>
        /// Get size from xml file for specified image.
        /// </summary>
        /// <returns>Size from xml or from original imagedata.</returns>
        static public Vector2 GetSize(string fileName, string path, float additionalScaleX = 1, float additionaScaleY = 1)
        {
            // Get size from xml
            path = Path.Combine(path, fileName) + extension;
            if (File.Exists(path))
            {
                XElement xml = XElement.Load(path);
                if (xml != null)
                {
                    XElement width = xml.Element(widthKey);
                    if (width != null)
                    {
                        XElement height = xml.Element(heightKey);
                        if (height != null)
                            return new Vector2((float)width * additionalScaleX, (float)height * additionaScaleY);
                    }
                }
            }

            // Fallback: get size from imagedata
            ImageData imageData = ImageReader.GetImageData(fileName, createTexture: false);
            return new Vector2(imageData.width * additionalScaleX, imageData.height * additionaScaleY);
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
            path = Path.Combine(path, fileName) + extension;

            // Get XML file
            if (File.Exists(path))
            {
                return XElement.Load(path);
            }
            else
            {
                Debug.LogError(path + " is missing");
                return null;
            }
        }

        #endregion
    }
}
