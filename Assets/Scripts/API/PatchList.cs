// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Ferital (ferital@yahoo.fr)
// Contributors:    
// 
// Notes:
//

using System.Collections.Generic;

/// <summary>
/// A list of patches to apply to a single file, grouped by their respective offsets.
/// </summary>
namespace DaggerfallConnect.Utility
{
    /// <summary>
    /// Simple structure used to patch a given offset
    /// with a list of new data values.
    /// </summary>
    public struct Patch
    {
        public readonly int offset;
        public readonly byte[] data;

        public Patch(int offset, ref byte[] data)
        {
            this.offset = offset;
            this.data = data;
        }
    }

    /// <summary>
    /// List of patches to apply to any file.
    /// </summary>
    public class PatchList : List<Patch>
    {
        /// <summary>
        /// Overloaded method for easier initialization.
        /// </summary>
        /// <param name="offset">The offset address to patch.</param>
        /// <param name="data">The new data values.</param>
        public void Add(int offset, params byte[] data)
        {
            Add(new Patch(offset, ref data));
        }
    }
}
