// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:
//
// Notes:
//

using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallConnect.Arena2
{
    public partial class BiogFile : IMacroContextProvider
    {
        public MacroDataSource GetMacroDataSource()
        {
            return new BiogFileMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for items in Daggerfall Unity.
        /// </summary>
        private class BiogFileMacroDataSource : MacroDataSource
        {
            private readonly BiogFile parent;

            private string GetChangeStr(short val)
            {
                if (val == 0)
                {
                    return HardStrings.unchanged;
                }
                else if (val < 0)
                {
                    return HardStrings.lower;
                }
                else
                {
                    return HardStrings.higher;
                }
            }

            public BiogFileMacroDataSource(BiogFile biogFile)
            {
                parent = biogFile;
            }

            public override string CommonersRep()
            {
                // %r1
                const int index = 0;
                return GetChangeStr(parent.changedReputations[index]);
            }

            public override string MerchantsRep()
            {
                // %r2
                const int index = 1;
                return GetChangeStr(parent.changedReputations[index]);
            }

            public override string ScholarsRep()
            {
                // %r3
                const int index = 2;
                return GetChangeStr(parent.changedReputations[index]);
            }

            public override string NobilityRep()
            {
                // %r4
                const int index = 3;
                return GetChangeStr(parent.changedReputations[index]);
            }

            public override string UnderworldRep()
            {
                // %r5
                const int index = 4;
                return GetChangeStr(parent.changedReputations[index]);
            }

            public override string HomeProvinceName()
            {
                // %hpn
                switch ((Races)parent.characterDocument.raceTemplate.ID)
                {
                    case Races.Argonian:
                        return HardStrings.blackMarsh;
                    case Races.Breton:
                        return HardStrings.highRock;
                    case Races.DarkElf:
                        return HardStrings.morrowind;
                    case Races.HighElf:
                        return HardStrings.sumurset;
                    case Races.Khajiit:
                        return HardStrings.elsweyr;
                    case Races.Nord:
                        return HardStrings.skyrim;
                    case Races.Redguard:
                        return HardStrings.hammerfell;
                    case Races.WoodElf:
                        return HardStrings.valenwood;
                    default:
                        return null;
                }
            }

            public override string GeographicalFeature()
            {
                // %hpw
                switch ((Races)parent.characterDocument.raceTemplate.ID) // Note: These are educated guesses based on lore.
                {
                    case Races.Argonian:
                        return HardStrings.swamps;
                    case Races.Breton:
                        return HardStrings.rollingHills;
                    case Races.DarkElf:
                        return HardStrings.rollingHills;
                    case Races.HighElf:
                        return HardStrings.shores;
                    case Races.Khajiit:
                        return HardStrings.desertLand;
                    case Races.Nord:
                        return HardStrings.mountains;
                    case Races.Redguard:
                        return HardStrings.desertLand;
                    case Races.WoodElf:
                        return HardStrings.forests;
                    default:
                        return null;
                }
            }

            public override string Q1()
            {
                // %q1
                if (parent.Q1Tokens.Count == 0)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q1Tokens[0]);

                return tokens[0].text;
            }

            public override string Q2()
            {
                // %q2
                if (parent.Q2Tokens.Count == 0)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q2Tokens[0]);

                return tokens[0].text;
            }

            public override string Q3()
            {
                // %q3
                if (parent.Q3Tokens.Count == 0)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q3Tokens[0]);

                return tokens[0].text;
            }

            public override string Q4()
            {
                // %q4
                if (parent.Q4Tokens.Count == 0)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q4Tokens[0]);

                return tokens[0].text;
            }

            public override string Q5()
            {
                // %q5
                if (parent.Q5Tokens.Count == 0)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q5Tokens[0]);

                return tokens[0].text;
            }

            public override string Q6()
            {
                // %q6
                if (parent.Q6Tokens.Count == 0)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q6Tokens[0]);

                return tokens[0].text;
            }

            public override string Q7()
            {
                // %q7
                if (parent.Q7Tokens.Count == 0)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q7Tokens[0]);

                return tokens[0].text;
            }

            public override string Q8()
            {
                // %q8
                if (parent.Q8Tokens.Count == 0)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q8Tokens[0]);

                return tokens[0].text;
            }

            public override string Q9()
            {
                // %q9
                if (parent.Q9Tokens.Count == 0)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q9Tokens[0]);

                return tokens[0].text;
            }

            public override string Q10()
            {
                // %q10
                if (parent.Q10Tokens.Count == 0)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q10Tokens[0]);

                return tokens[0].text;
            }

            public override string Q11()
            {
                // %q11
                if (parent.Q11Tokens.Count == 0)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q11Tokens[0]);

                return tokens[0].text;
            }

            public override string Q12()
            {
                // %q12
                if (parent.Q12Tokens.Count == 0)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q12Tokens[0]);

                return tokens[0].text;
            }

            public override string Q1a()
            {
                // %q1
                if (parent.Q1Tokens.Count < 2)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q1Tokens[1]);

                return tokens[0].text;
            }

            public override string Q2a()
            {
                // %q2
                if (parent.Q2Tokens.Count < 2)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q2Tokens[1]);

                return tokens[0].text;
            }

            public override string Q3a()
            {
                // %q3
                if (parent.Q3Tokens.Count < 2)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q3Tokens[1]);

                return tokens[0].text;
            }

            public override string Q4a()
            {
                // %q4
                if (parent.Q4Tokens.Count < 2)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q4Tokens[1]);

                return tokens[0].text;
            }

            public override string Q5a()
            {
                // %q5
                if (parent.Q5Tokens.Count < 2)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q5Tokens[1]);

                return tokens[0].text;
            }

            public override string Q6a()
            {
                // %q6
                if (parent.Q6Tokens.Count < 2)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q6Tokens[1]);

                return tokens[0].text;
            }

            public override string Q7a()
            {
                // %q7
                if (parent.Q7Tokens.Count < 2)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q7Tokens[1]);

                return tokens[0].text;
            }

            public override string Q8a()
            {
                // %q8
                if (parent.Q8Tokens.Count < 2)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q8Tokens[1]);

                return tokens[0].text;
            }

            public override string Q9a()
            {
                // %q9
                if (parent.Q9Tokens.Count < 2)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q9Tokens[1]);

                return tokens[0].text;
            }

            public override string Q10a()
            {
                // %q10
                if (parent.Q10Tokens.Count < 2)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q10Tokens[1]);

                return tokens[0].text;
            }

            public override string Q11a()
            {
                // %q11
                if (parent.Q11Tokens.Count < 2)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q11Tokens[1]);

                return tokens[0].text;
            }

            public override string Q12a()
            {
                // %q12
                if (parent.Q12Tokens.Count < 2)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q12Tokens[1]);

                return tokens[0].text;
            }

            public override string Q1b()
            {
                // %q1b
                if (parent.Q1Tokens.Count < 3)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q1Tokens[2]);

                return tokens[0].text;
            }

            public override string Q2b()
            {
                // %q2b
                if (parent.Q2Tokens.Count < 3)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q2Tokens[2]);

                return tokens[0].text;
            }

            public override string Q3b()
            {
                // %q3b
                if (parent.Q3Tokens.Count < 3)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q3Tokens[2]);

                return tokens[0].text;
            }

            public override string Q4b()
            {
                // %q4b
                if (parent.Q4Tokens.Count < 3)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q4Tokens[2]);

                return tokens[0].text;
            }

            public override string Q5b()
            {
                // %q5b
                if (parent.Q5Tokens.Count < 3)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q5Tokens[2]);

                return tokens[0].text;
            }

            public override string Q6b()
            {
                // %q6b
                if (parent.Q6Tokens.Count < 3)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q6Tokens[2]);

                return tokens[0].text;
            }

            public override string Q7b()
            {
                // %q7b
                if (parent.Q7Tokens.Count < 3)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q7Tokens[2]);

                return tokens[0].text;
            }

            public override string Q8b()
            {
                // %q8b
                if (parent.Q8Tokens.Count < 3)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q8Tokens[2]);

                return tokens[0].text;
            }

            public override string Q9b()
            {
                // %q9b
                if (parent.Q9Tokens.Count < 3)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q9Tokens[2]);

                return tokens[0].text;
            }

            public override string Q10b()
            {
                // %q10b
                if (parent.Q10Tokens.Count < 3)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q10Tokens[2]);

                return tokens[0].text;
            }

            public override string Q11b()
            {
                // %q11b
                if (parent.Q11Tokens.Count < 3)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q11Tokens[2]);

                return tokens[0].text;
            }

            public override string Q12b()
            {
                // %q12b
                if (parent.Q12Tokens.Count < 3)
                {
                    return null;
                }

                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(parent.Q12Tokens[2]);

                return tokens[0].text;
            }
        }
    }
}
