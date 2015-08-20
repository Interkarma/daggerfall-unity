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
using UnityEngine;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Effects;

namespace DaggerfallWorkshop.Game.Player
{
    /// <summary>
    /// Every race is defined by a common template.
    /// This is only used during character creation.
    /// </summary>
    public class RaceTemplate
    {
        public int ID;                                      // A unique id for this race. Default race IDs match colour picker index on TAMRIEL2.IMG
        public string Name;                                 // Name of this race in singular, e.g. "Dark Elf"
        public int DescriptionID;                           // TEXT.RSC ID text to display on race selection
        public int ClipID;                                  // DAGGER.SND clip ID to play at race selection

        public string PaperDollBackground;                  // IMG filename of paper doll background

        public string PaperDollBodyMaleUnclothed;           // IMG filename of male paper doll body - unclothed
        public string PaperDollBodyMaleClothed;             // IMG filename of male paper doll body - clothed
        public string PaperDollBodyFemaleUnclothed;         // IMG filename of female paper doll body - unclothed
        public string PaperDollBodyFemaleClothed;           // IMG filename of female paper doll body - clothed

        public string PaperDollHeadsMale;                   // CIF filename of male head selection
        public string PaperDollHeadsFemale;                 // CIF filename of female head selection

        public EffectFlags ResistanceFlags;                 // Racial resistances
        public EffectFlags ImmunityFlags;                   // Racial immunity
        public EffectFlags LowToleranceFlags;               // Racial low tolerance
        public EffectFlags CriticalWeaknessFlags;           // Racial critical weakness
        public SpecialAbilityFlags SpecialAbilities;        // Racial special abilities
    }

    #region Default Race Templates

    public class Breton : RaceTemplate
    {
        public Breton()
        {
            ID = 1;
            Name = "Breton";
            DescriptionID = 2003;
            ClipID = 209;

            PaperDollBackground = "";

            PaperDollBodyMaleUnclothed = "BODY00I0.IMG";
            PaperDollBodyMaleClothed = "BODY00I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY10I0.IMG";
            PaperDollBodyFemaleClothed = "BODY10I1.IMG";

            PaperDollHeadsMale = "";
            PaperDollHeadsFemale = "";
        }
    }

    public class Redguard : RaceTemplate
    {
        public Redguard()
        {
            ID = 2;
            Name = "Redguard";
            DescriptionID = 2002;
            ClipID = 210;

            PaperDollBackground = "";

            PaperDollBodyMaleUnclothed = "BODY01I0.IMG";
            PaperDollBodyMaleClothed = "BODY01I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY11I0.IMG";
            PaperDollBodyFemaleClothed = "BODY11I1.IMG";

            PaperDollHeadsMale = "";
            PaperDollHeadsFemale = "";
        }
    }

    public class Nord : RaceTemplate
    {
        public Nord()
        {
            ID = 3;
            Name = "Nord";
            DescriptionID = 2000;
            ClipID = 211;

            PaperDollBackground = "";

            PaperDollBodyMaleUnclothed = "BODY02I0.IMG";
            PaperDollBodyMaleClothed = "BODY02I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY12I0.IMG";
            PaperDollBodyFemaleClothed = "BODY12I1.IMG";

            PaperDollHeadsMale = "";
            PaperDollHeadsFemale = "";

            ResistanceFlags = EffectFlags.Frost;
        }
    }

    public class DarkElf : RaceTemplate
    {
        public DarkElf()
        {
            ID = 4;
            Name = "Dark Elf";
            DescriptionID = 2007;
            ClipID = 212;

            PaperDollBackground = "";

            PaperDollBodyMaleUnclothed = "BODY03I0.IMG";
            PaperDollBodyMaleClothed = "BODY03I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY13I0.IMG";
            PaperDollBodyFemaleClothed = "BODY13I1.IMG";

            PaperDollHeadsMale = "";
            PaperDollHeadsFemale = "";
        }
    }

    public class HighElf : RaceTemplate
    {
        public HighElf()
        {
            ID = 5;
            Name = "High Elf";
            DescriptionID = 2006;
            ClipID = 213;

            PaperDollBackground = "";

            PaperDollBodyMaleUnclothed = "BODY04I0.IMG";
            PaperDollBodyMaleClothed = "BODY04I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY14I0.IMG";
            PaperDollBodyFemaleClothed = "BODY14I1.IMG";

            PaperDollHeadsMale = "";
            PaperDollHeadsFemale = "";

            ImmunityFlags = EffectFlags.Paralysis;
        }
    }

    public class WoodElf : RaceTemplate
    {
        public WoodElf()
        {
            ID = 6;
            Name = "Wood Elf";
            DescriptionID = 2005;
            ClipID = 214;

            PaperDollBackground = "";

            PaperDollBodyMaleUnclothed = "BODY05I0.IMG";
            PaperDollBodyMaleClothed = "BODY05I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY15I0.IMG";
            PaperDollBodyFemaleClothed = "BODY15I1.IMG";

            PaperDollHeadsMale = "";
            PaperDollHeadsFemale = "";
        }
    }

    public class Khajiit : RaceTemplate
    {
        public Khajiit()
        {
            ID = 7;
            Name = "Khajiit";
            DescriptionID = 2001;
            ClipID = 215;

            PaperDollBackground = "";

            PaperDollBodyMaleUnclothed = "BODY06I0.IMG";
            PaperDollBodyMaleClothed = "BODY06I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY16I0.IMG";
            PaperDollBodyFemaleClothed = "BODY16I1.IMG";

            PaperDollHeadsMale = "";
            PaperDollHeadsFemale = "";
        }
    }

    public class Argonian : RaceTemplate
    {
        public Argonian()
        {
            ID = 8;
            Name = "Argonian";
            DescriptionID = 2004;
            ClipID = 216;

            PaperDollBackground = "";

            PaperDollBodyMaleUnclothed = "BODY07I0.IMG";
            PaperDollBodyMaleClothed = "BODY07I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY17I0.IMG";
            PaperDollBodyFemaleClothed = "BODY17I1.IMG";

            PaperDollHeadsMale = "";
            PaperDollHeadsFemale = "";
        }
    }

    #endregion
}