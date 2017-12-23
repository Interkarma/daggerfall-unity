// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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

        #region Public Methods

        public XMLManager(string path)
        {
            if (!path.EndsWith(extension))
                path += extension;

            this.xml = XElement.Load(path);
        }

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

        #endregion

        #region Private Methods

        private bool TryGetElement(string key, out XElement value)
        {
            return (value = xml.Element(key)) != null;
        }

        #endregion

        #region Static Methods

        public static bool XmlFileExists(string path)
        {
            if (!path.EndsWith(extension))
                path += extension;

            return File.Exists(path);
        }

        #endregion
    }
}
