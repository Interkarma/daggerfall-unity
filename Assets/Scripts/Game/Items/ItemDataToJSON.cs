// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.FallExe;
using FullSerializer;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game.Items
{
    /// <summary>
    /// Handles exporting item template data from FALL.EXE to JSON.
    /// </summary>
    public class ItemDataToJSON
    {
        /// <summary>
        /// Initial implementation just dumps ItemDescription to JSON.
        /// </summary>
        /// <param name="fallExePath">Path to FALL.EXE containing item database.</param>
        /// <param name="outputPath">Output path for JSON file.</param>
        public static void CreateJSON(string fallExePath, string outputPath)
        {
            ItemsFile itemsFile = new ItemsFile(fallExePath);
            List<ItemTemplate> itemDescriptions = new List<ItemTemplate>(itemsFile.ItemsCount);
            for (int i = 0; i < itemsFile.ItemsCount; i++)
            {
                itemDescriptions.Add(itemsFile.GetItemDescription(i));
            }

            string json = SaveLoadManager.Serialize(itemDescriptions.GetType(), itemDescriptions);
            File.WriteAllText(outputPath, json);
        }
    }
}