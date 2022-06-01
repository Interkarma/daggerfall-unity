// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:
//
// Notes:
//

using System.IO;
using System.Collections.Generic;

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Connects to a Daggerfall BIO.DAT file and extracts the player biography text.
    /// </summary>
    public class BioFile
    {
        public BioFile()
        {
            Lines = new List<string>();
        }

        public bool Load(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            if (reader == null)
            {
                return false;
            }
            string text = reader.ReadToEnd();
            reader.Close();
            string[] textLines = text.Split(new char[] { '\0' });
            for (int i = 0; i < textLines.Length; i++)
            {
                Lines.Add(textLines[i]);
            }

            return true;
        }

        public List<string> Lines { get; set; }
    }
}
