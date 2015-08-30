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
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.Player
{
    /// <summary>
    /// The character sheet is a document filled in by the character creation process.
    /// </summary>
    public class CharacterSheet
    {
        public RaceTemplate race;
        public Genders gender;
        public DFClass dfClass;
        public string name;
        public int faceIndex;

        public CharacterSheet()
        {
            SetDefaultValues();
        }

        // Set some default values for testing during development
        void SetDefaultValues()
        {
            race = GetRaceTemplate(Races.Breton);
            gender = Genders.Male;
            dfClass = GetClassTemplate(Classes.Mage);
            name = "Test McTest";
            faceIndex = 0;
        }

        public static RaceTemplate GetRaceTemplate(Races race)
        {
            switch (race)
            {
                default:
                case Races.Breton:
                    return new Breton();
                case Races.Redguard:
                    return new Redguard();
                case Races.Nord:
                    return new Nord();
                case Races.DarkElf:
                    return new DarkElf();
                case Races.HighElf:
                    return new HighElf();
                case Races.WoodElf:
                    return new WoodElf();
                case Races.Khajiit:
                    return new Khajiit();
                case Races.Argonian:
                    return new Argonian();
            }
        }

        public static DFClass GetClassTemplate(Classes classTemplate)
        {
            string filename = string.Format("CLASS{0:00}.CFG", (int)classTemplate);
            ClassFile file = new ClassFile();
            if (!file.Load(Path.Combine(DaggerfallUnity.Instance.Arena2Path, filename)))
                return null;

            return file.DFClass;
        }
    }
}