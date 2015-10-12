// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Utility;

namespace DaggerfallConnect.Save
{
    /// <summary>
    /// Spell record.
    /// SaveTreeRecordTypes = 0x09
    /// </summary>
    public class SpellRecord : SaveTreeBaseRecord
    {
        public SpellRecord()
        {
        }

        public SpellRecord(BinaryReader reader, int length)
            : base(reader, length)
        {
        }
    }
}