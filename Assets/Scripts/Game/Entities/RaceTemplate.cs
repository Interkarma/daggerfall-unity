// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System.Collections.Generic;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Every race is defined by a common template.
    /// This will likely be changed to a text file system later.
    /// </summary>
    public class RaceTemplate
    {
        protected const string textDatabase = "Races";

        public int ID;                                          // A unique id for this race. Default race IDs match colour picker index on TAMRIEL2.IMG
        public string Name;                                     // Name of this race in singular, e.g. "Dark Elf"
        public int DescriptionID;                               // TEXT.RSC ID text to display on race selection
        public int ClipID;                                      // DAGGER.SND clip ID to play at race selection

        public string PaperDollBackground;                      // IMG filename of paper doll background

        public string PaperDollBodyMaleUnclothed;               // IMG filename of male paper doll body - unclothed
        public string PaperDollBodyMaleClothed;                 // IMG filename of male paper doll body - clothed
        public string PaperDollBodyFemaleUnclothed;             // IMG filename of female paper doll body - unclothed
        public string PaperDollBodyFemaleClothed;               // IMG filename of female paper doll body - clothed

        public string PaperDollHeadsMale;                       // CIF filename of male head selection
        public string PaperDollHeadsFemale;                     // CIF filename of female head selection

        public DFCareer.EffectFlags ResistanceFlags;            // Racial resistances
        public DFCareer.EffectFlags ImmunityFlags;              // Racial immunity
        public DFCareer.EffectFlags LowToleranceFlags;          // Racial low tolerance
        public DFCareer.EffectFlags CriticalWeaknessFlags;      // Racial critical weakness
        public DFCareer.SpecialAbilityFlags SpecialAbilities;   // Racial special abilities

        /// <summary>
        /// Clones this race template.
        /// </summary>
        /// <returns>Cloned RaceTemplate reference.</returns>
        public RaceTemplate Clone()
        {
            RaceTemplate clone = new RaceTemplate();
            clone.ID = ID;
            clone.Name = Name;
            clone.DescriptionID = DescriptionID;
            clone.ClipID = ClipID;
            clone.PaperDollBackground = PaperDollBackground;
            clone.PaperDollBodyMaleUnclothed = PaperDollBodyMaleUnclothed;
            clone.PaperDollBodyMaleClothed = PaperDollBodyMaleClothed;
            clone.PaperDollBodyFemaleUnclothed = PaperDollBodyFemaleUnclothed;
            clone.PaperDollBodyFemaleClothed = PaperDollBodyFemaleClothed;
            clone.PaperDollHeadsMale = PaperDollHeadsMale;
            clone.PaperDollHeadsFemale = PaperDollHeadsFemale;
            clone.ResistanceFlags = ResistanceFlags;
            clone.ImmunityFlags = ImmunityFlags;
            clone.LowToleranceFlags = LowToleranceFlags;
            clone.CriticalWeaknessFlags = CriticalWeaknessFlags;
            clone.SpecialAbilities = SpecialAbilities;

            return clone;
        }

        /// <summary>
        /// Populates a race dictionary with standard RaceTemplate definitions.
        /// This is only temporary until loading race definitions from file is implemented.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, RaceTemplate> GetRaceDictionary()
        {
            Dictionary<int, RaceTemplate> raceDict = new Dictionary<int, RaceTemplate>();

            // Instantiate race templates
            Breton breton = new Breton();
            Redguard redguard = new Redguard();
            Nord nord = new Nord();
            DarkElf darkElf = new DarkElf();
            HighElf highElf = new HighElf();
            WoodElf woodElf = new WoodElf();
            Khajiit khajiit = new Khajiit();
            Argonian argonian = new Argonian();

            // Populate dictionary
            raceDict.Add(breton.ID, breton);
            raceDict.Add(redguard.ID, redguard);
            raceDict.Add(nord.ID, nord);
            raceDict.Add(darkElf.ID, darkElf);
            raceDict.Add(highElf.ID, highElf);
            raceDict.Add(woodElf.ID, woodElf);
            raceDict.Add(khajiit.ID, khajiit);
            raceDict.Add(argonian.ID, argonian);

            return raceDict;
        }
    }

    #region Default Race Templates

    public class Breton : RaceTemplate
    {
        public Breton()
        {
            ID = (int)Races.Breton;
            Name = TextManager.Instance.GetText(textDatabase, "breton");
            DescriptionID = 2003;
            ClipID = 209;

            PaperDollBackground = "SCBG00I0.IMG";

            PaperDollBodyMaleUnclothed = "BODY00I0.IMG";
            PaperDollBodyMaleClothed = "BODY00I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY10I0.IMG";
            PaperDollBodyFemaleClothed = "BODY10I1.IMG";

            PaperDollHeadsMale = "FACE00I0.CIF";
            PaperDollHeadsFemale = "FACE10I0.CIF";
        }
    }

    public class Redguard : RaceTemplate
    {
        public Redguard()
        {
            ID = (int)Races.Redguard;
            Name = TextManager.Instance.GetText(textDatabase, "redguard");
            DescriptionID = 2002;
            ClipID = 210;

            PaperDollBackground = "SCBG01I0.IMG";

            PaperDollBodyMaleUnclothed = "BODY01I0.IMG";
            PaperDollBodyMaleClothed = "BODY01I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY11I0.IMG";
            PaperDollBodyFemaleClothed = "BODY11I1.IMG";

            PaperDollHeadsMale = "FACE01I0.CIF";
            PaperDollHeadsFemale = "FACE11I0.CIF";
        }
    }

    public class Nord : RaceTemplate
    {
        public Nord()
        {
            ID = (int)Races.Nord;
            Name = TextManager.Instance.GetText(textDatabase, "nord");
            DescriptionID = 2000;
            ClipID = 211;

            PaperDollBackground = "SCBG02I0.IMG";

            PaperDollBodyMaleUnclothed = "BODY02I0.IMG";
            PaperDollBodyMaleClothed = "BODY02I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY12I0.IMG";
            PaperDollBodyFemaleClothed = "BODY12I1.IMG";

            PaperDollHeadsMale = "FACE02I0.CIF";
            PaperDollHeadsFemale = "FACE12I0.CIF";

            ResistanceFlags = DFCareer.EffectFlags.Frost;
        }
    }

    public class DarkElf : RaceTemplate
    {
        public DarkElf()
        {
            ID = (int)Races.DarkElf;
            Name = TextManager.Instance.GetText(textDatabase, "darkElf");
            DescriptionID = 2007;
            ClipID = 212;

            PaperDollBackground = "SCBG03I0.IMG";

            PaperDollBodyMaleUnclothed = "BODY03I0.IMG";
            PaperDollBodyMaleClothed = "BODY03I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY13I0.IMG";
            PaperDollBodyFemaleClothed = "BODY13I1.IMG";

            PaperDollHeadsMale = "FACE03I0.CIF";
            PaperDollHeadsFemale = "FACE13I0.CIF";
        }
    }

    public class HighElf : RaceTemplate
    {
        public HighElf()
        {
            ID = (int)Races.HighElf;
            Name = TextManager.Instance.GetText(textDatabase, "highElf");
            DescriptionID = 2006;
            ClipID = 213;

            PaperDollBackground = "SCBG04I0.IMG";

            PaperDollBodyMaleUnclothed = "BODY04I0.IMG";
            PaperDollBodyMaleClothed = "BODY04I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY14I0.IMG";
            PaperDollBodyFemaleClothed = "BODY14I1.IMG";

            PaperDollHeadsMale = "FACE04I0.CIF";
            PaperDollHeadsFemale = "FACE14I0.CIF";

            ImmunityFlags = DFCareer.EffectFlags.Paralysis;
        }
    }

    public class WoodElf : RaceTemplate
    {
        public WoodElf()
        {
            ID = (int)Races.WoodElf;
            Name = TextManager.Instance.GetText(textDatabase, "woodElf");
            DescriptionID = 2005;
            ClipID = 214;

            PaperDollBackground = "SCBG05I0.IMG";

            PaperDollBodyMaleUnclothed = "BODY05I0.IMG";
            PaperDollBodyMaleClothed = "BODY05I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY15I0.IMG";
            PaperDollBodyFemaleClothed = "BODY15I1.IMG";

            PaperDollHeadsMale = "FACE05I0.CIF";
            PaperDollHeadsFemale = "FACE15I0.CIF";
        }
    }

    public class Khajiit : RaceTemplate
    {
        public Khajiit()
        {
            ID = (int)Races.Khajiit;
            Name = TextManager.Instance.GetText(textDatabase, "khajiit");
            DescriptionID = 2001;
            ClipID = 215;

            PaperDollBackground = "SCBG06I0.IMG";

            PaperDollBodyMaleUnclothed = "BODY06I0.IMG";
            PaperDollBodyMaleClothed = "BODY06I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY16I0.IMG";
            PaperDollBodyFemaleClothed = "BODY16I1.IMG";

            PaperDollHeadsMale = "FACE06I0.CIF";
            PaperDollHeadsFemale = "FACE16I0.CIF";
        }
    }

    public class Argonian : RaceTemplate
    {
        public Argonian()
        {
            ID = (int)Races.Argonian;
            Name = TextManager.Instance.GetText(textDatabase, "argonian");
            DescriptionID = 2004;
            ClipID = 216;

            PaperDollBackground = "SCBG07I0.IMG";

            PaperDollBodyMaleUnclothed = "BODY07I0.IMG";
            PaperDollBodyMaleClothed = "BODY07I1.IMG";
            PaperDollBodyFemaleUnclothed = "BODY17I0.IMG";
            PaperDollBodyFemaleClothed = "BODY17I1.IMG";

            PaperDollHeadsMale = "FACE07I0.CIF";
            PaperDollHeadsFemale = "FACE17I0.CIF";
        }
    }

    #endregion
}
