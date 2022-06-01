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

using System.Globalization;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Read XML files from disk for modding purposes
    /// </summary>
    public class XMLManager
    {
        #region Fields

        const string extension = ".xml";
        readonly XElement xml;

        #endregion

        #region Constructors

        public XMLManager(string path)
        {
            AddExtensionIfMissing(ref path);
            this.xml = XElement.Load(path);
        }

        public XMLManager(TextReader textReader)
        {
            this.xml = XElement.Load(textReader);
        }

        #endregion

        #region Public Methods

        public bool TryGetString(string key, out string value)
        {
            XElement element;
            if (TryGetElement(key, out element))
            {
                value = (string)element;
                return true;
            }

            value = null;
            return false;
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            XElement element;
            return TryGetElement(key, out element) ? bool.Parse((string)element) : defaultValue;
        }

        public bool TryGetFloat(string key, out float value)
        {
            XElement element;
            if (TryGetElement(key, out element))
                return float.TryParse((string)element, NumberStyles.Float, CultureInfo.InvariantCulture, out value);

            value = -1;
            return false;
        }

        public bool TryGetVector2(string keyX, string keyY, out Vector2 vector)
        {
            vector = new Vector2();
            return TryGetFloat(keyX, out vector.x) && TryGetFloat(keyY, out vector.y);
        }

        public Vector2 GetVector2(string keyX, string keyY, Vector2 baseVector)
        {
            float x, y;

            return new Vector2(
                TryGetFloat(keyX, out x) ? x : baseVector.x,
                TryGetFloat(keyY, out y) ? y : baseVector.y
                );
        }

        public Vector3 GetVector3(string keyX, string keyY, Vector3 baseVector)
        {
            float x, y;

            return new Vector3(
                TryGetFloat(keyX, out x) ? x : baseVector.x,
                TryGetFloat(keyY, out y) ? y : baseVector.y,
                baseVector.z
                );
        }

        public bool TryGetColor(out Color color)
        {
            color = new Color();
            return TryGetFloat("r", out color.r) && TryGetFloat("g", out color.g)
                && TryGetFloat("b", out color.b) && TryGetFloat("a", out color.a);
        }

        public bool TryGetColor(float defaultAlpha, out Color color)
        {
            color = new Color();

            if (TryGetFloat("r", out color.r) && TryGetFloat("g", out color.g)
                && TryGetFloat("b", out color.b))
            {
                if (!TryGetFloat("a", out color.a))
                    color.a = defaultAlpha;

                return true;
            }

            return false;
        }

        public Color GetColor(Color baseColor)
        {
            float r, g, b, a;

            return new Color(
                TryGetFloat("r", out r) ? r : baseColor.r,
                TryGetFloat("g", out g) ? g : baseColor.g,
                TryGetFloat("b", out b) ? b : baseColor.b,
                TryGetFloat("a", out a) ? a : baseColor.a
                );
        }

        /// <summary>
        /// Gets a rect and scale all its value to the requested <paramref name="requestedScale"/>.
        /// The xml element can provide an attribute with a specified scale different than one.
        /// </summary>
        /// <param name="name">The name of the rect element.</param>
        /// <param name="baseRect">Default values for the rect.</param>
        /// <param name="requestedScale">A value to scale the rect.</param>
        /// <returns>Rect read from xml file.</returns>
        public Rect GetRect(string name, Rect baseRect, float requestedScale = 1)
        {
            XElement element;
            if (!TryGetElement(name, out element))
                return baseRect;

            float scale = requestedScale / ParseFloat((string)element.Attribute("scale")) ?? 1;
            return new Rect(
                ParseFloat((string)element.Element("x"), scale) ?? baseRect.x,
                ParseFloat((string)element.Element("y"), scale) ?? baseRect.y,
                ParseFloat((string)element.Element("width"), scale) ?? baseRect.width,
                ParseFloat((string)element.Element("height"), scale) ?? baseRect.height
                );
        }

        #endregion

        #region Private Methods

        private bool TryGetElement(string key, out XElement value)
        {
            return (value = xml.Element(key)) != null;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Loose files contain xml ?
        /// </summary>
        /// <param name="path">Path to xml.</param>
        /// <returns>True if xml file exists.</returns>
        public static bool XmlFileExists(string path)
        {
            AddExtensionIfMissing(ref path);
            return File.Exists(path);
        }

        /// <summary>
        /// Seek xml from modding locations.
        /// </summary>
        /// <param name="directory">Directory on disk (loose files only).</param>
        /// <param name="name">Name of xml.</param>
        /// <param name="xml">XMLManager instance from imported xml.</param>
        /// <returns>True if xml found and read.</returns>
        public static bool TryReadXml(string directory, string name, out XMLManager xml)
        {
            AddExtensionIfMissing(ref name);

            // Seek from loose files
            string path = Path.Combine(directory, name);
            if (XmlFileExists(path))
            {
                xml = new XMLManager(path);
                return true;
            }

            // Seek from mods
            if (ModManager.Instance != null)
            {
                TextAsset textAsset;
                if (ModManager.Instance.TryGetAsset(name, false, out textAsset))
                {
                    using (var stringReader = new StringReader(textAsset.text))
                        xml = new XMLManager(stringReader);
                    return true;
                }
            }

            xml = null;
            return false;
        }

        private static void AddExtensionIfMissing(ref string path)
        {
            if (!path.EndsWith(extension))
                path += extension;
        }

        private static float? ParseFloat(string element = null, float scale = 1)
        {
            float value;
            if (element != null && float.TryParse(element, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                return value * scale;

            return null;
        }

        #endregion
    }
}
