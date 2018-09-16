using System;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

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
            private BiogFile parent;
            
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
        }
    }
}