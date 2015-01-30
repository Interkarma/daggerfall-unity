// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace DaggerfallConnect
{

    /// <summary>
    /// Stores sound data.
    /// </summary>
    public struct DFSound
    {

        #region Structure Variables

        /// <summary>
        /// Numerical identifier for this sound.
        /// </summary>
        public string Name;

        /// <summary>
        /// Wave header including DATA prefix preceding raw sound bytes.
        /// </summary>
        public byte[] WaveHeader;

        /// <summary>
        /// Wave file data bytes. 8-bit unsigned mono at 11025 samples per second.
        ///  Can be used as a raw buffer or written to a WAV immediately after WaveHeader.
        /// </summary>
        public byte[] WaveData;

        #endregion

    }

}
