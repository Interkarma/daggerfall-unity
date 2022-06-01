// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System.IO;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallConnect.Save
{
    /// <summary>
    /// A basic save game image loaded from RAW data.
    /// Palette must be loaded separately.
    /// </summary>
    public class SaveImage : BaseImageFile
    {
        public const string Filename = "IMAGE.RAW";

        public override string Description
        {
            get { return Filename; }
        }

        public override string PaletteName
        {
            get { return "ART_PAL.COL"; }
        }

        public override int RecordCount
        {
            get { return 1; }
        }

        public override bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Validate filename
            string fn = Path.GetFileName(filePath);
            if (string.Compare(fn, Filename, true) != 0)
                return false;

            // Load file
            if (!managedFile.Load(filePath, usage, readOnly))
                return false;

            return true;
        }

        public override int GetFrameCount(int record)
        {
            if (record == 0)
                return 1;
            else
                return 0;
        }

        public override DFSize GetSize(int record)
        {
            if (record == 0)
                return new DFSize(80, 50);
            else
                return new DFSize();
        }

        public DFBitmap GetDFBitmap()
        {
            return GetDFBitmap(0, 0);
        }

        public override DFBitmap GetDFBitmap(int record, int frame)
        {
            DFBitmap result = new DFBitmap();
            if (record == 0 && frame == 0 && managedFile.Length > 0)
            {
                BinaryReader reader = managedFile.GetReader();
                byte[] data = reader.ReadBytes(managedFile.Length);

                DFSize sz = GetSize(0);
                result.Width = sz.Width;
                result.Height = sz.Height;
                result.Data = data;
            }

            return result;
        }
    }
}
