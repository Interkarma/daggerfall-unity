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

using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Generates building names in city locations.
    /// </summary>
    public static class BuildingNames
    {
        public static string GetName(int seed, DFLocation.BuildingTypes type, int factionID, string locationName, string regionName)
        {
            return FormulaHelper.GenerateBuildingName(seed, type, factionID, locationName, regionName);
        }
    }
}