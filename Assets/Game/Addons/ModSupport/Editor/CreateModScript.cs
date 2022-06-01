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

using UnityEditor;

namespace DaggerfallWorkshop.Game.Utility
{
    public sealed class CreateModScript
    {
        [MenuItem("Assets/Create/C# Mod Script", false, 81)]
        public static void Create()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/Game/Addons/ModSupport/Editor/ModScript.cs.txt", "NewModScript.cs");
        }
    }
}