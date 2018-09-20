// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    Numidium, Gavin Clayton

using System;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Macro data source base class. Extend this to provide context for macros.
    /// This abstract class provides default implementations so that only applicable
    /// handlers need to be implemented.
    /// </summary>
    public abstract class MacroDataSource   
    // TODO: extract interface when complete set of handlers done? : IMacroDataSource
    {
        public virtual string GuildTitle()
        {   // %lev %pct
            throw new NotImplementedException();
        }

        public virtual string FactionOrderName()
        {   // %fon
            throw new NotImplementedException();
        }

        public virtual string FactionPC()
        {   // %fpc
            throw new NotImplementedException();
        }

        public virtual string GuildNPC()
        {   // %fnpc
            throw new NotImplementedException();
        }

        public virtual string FactionName()
        {   // %fpa
            throw new NotImplementedException();
        }

        public virtual string Dungeon()
        {   // %dng
            throw new NotImplementedException();
        }

        public virtual string Amount()
        {   // %a
            throw new NotImplementedException();
        }
        public virtual string ShopName()
        {   // %cpn
            throw new NotImplementedException();
        }
        public virtual string MaxLoan()
        {   // %ml
            throw new NotImplementedException();
        }

        public virtual string Str()
        {   // %str
            throw new NotImplementedException();
        }
        public virtual string Int()
        {   // %int
            throw new NotImplementedException();
        }
        public virtual string Wil()
        {   // %wil
            throw new NotImplementedException();
        }
        public virtual string Agi()
        {   // %agi
            throw new NotImplementedException();
        }
        public virtual string End()
        {   // %end
            throw new NotImplementedException();
        }
        public virtual string Per()
        {   // %per
            throw new NotImplementedException();
        }
        public virtual string Spd()
        {   // %spd
            throw new NotImplementedException();
        }
        public virtual string Luck()
        {   // %luc
            throw new NotImplementedException();
        }

        public virtual string AttributeRating()
        {   // %ark
            throw new NotImplementedException();
        }

        public virtual string ItemName()
        {
            throw new NotImplementedException();
        }

        public virtual string Worth()
        {
            throw new NotImplementedException();
        }

        public virtual string Material()
        {   // %mat
            throw new NotImplementedException();
        }

        public virtual string Condition()
        {   // %qua
            throw new NotImplementedException();
        }

        public virtual string Weight()
        {   // %kg
            throw new NotImplementedException();
        }

        public virtual string WeaponDamage()
        {   // %wdm
            throw new NotImplementedException();
        }

        public virtual string Weapon()
        {   // %wep
            throw new NotImplementedException();
        }

        public virtual string ArmourMod()
        {   // %mod
            throw new NotImplementedException();
        }

        public virtual string BookAuthor()
        {   // %ba
            throw new NotImplementedException();
        }

        public virtual string PaintingAdjective()
        {   // %adj
            throw new NotImplementedException();
        }
        public virtual string ArtistName()
        {   // %an
            throw new NotImplementedException();
        }
        public virtual string PaintingPrefix1()
        {   // %pp1
            throw new NotImplementedException();
        }
        public virtual string PaintingPrefix2()
        {   // %pp2
            throw new NotImplementedException();
        }
        public virtual string PaintingSubject()
        {   // %sub
            throw new NotImplementedException();
        }

        public virtual string HeldSoul()
        {   // %hs
            throw new NotImplementedException();
        }

        public virtual string Potion()
        {   // %po
            throw new NotImplementedException();
        }

        public virtual string Pronoun()
        {   // %g & %g1
            throw new NotImplementedException();
        }
        public virtual string Pronoun2()
        {   // %g2
            throw new NotImplementedException();
        }
        public virtual string Pronoun2self()
        {   // %g2self
            throw new NotImplementedException();
        }
        public virtual string Pronoun3()
        {   // %g3
            throw new NotImplementedException();
        }

        public virtual string Honorific()
        {   // %hnr
            throw new NotImplementedException();
        }

        public virtual string QuestDate()
        {   // %qdt
            throw new NotImplementedException();
        }

        public virtual string Oath()
        {   // %oth
            throw new NotImplementedException();
        }

        public virtual string HomeRegion()
        {   // %hrg
            throw new NotImplementedException();
        }

        public virtual string GodDesc()
        {   // %gdd
            throw new NotImplementedException();
        }

        public virtual string God()
        {   // %god
            throw new NotImplementedException();
        }

        public virtual string LocationDirection()
        {   // %di
            throw new NotImplementedException();
        }

        public virtual string DialogHint()
        {   // %hnt
            throw new NotImplementedException();
        }

        public virtual string DialogHint2()
        {   // %hnt2
            throw new NotImplementedException();
        }

        public virtual string AFactionInNews()
        {   // %fx1
            throw new NotImplementedException();
        }

        public virtual string AnotherFactionInNews()
        {   // %fx2
            throw new NotImplementedException();
        }

        public virtual string OldLordOfFaction1()
        {   // %ol1
            throw new NotImplementedException();
        }

        public virtual string LordOfFaction1()
        {   // %fl1
            throw new NotImplementedException();
        }

        public virtual string LordOfFaction2()
        {   // %fl2
            throw new NotImplementedException();
        }

        public virtual string TitleOfLordOfFaction1()
        {   // %lt1
            throw new NotImplementedException();
        }

        public virtual string OldLeaderFate()
        {   // %olf
            throw new NotImplementedException();
        }

        public virtual string RoomHoursLeft()
        {   // %dwr
            throw new NotImplementedException();
        }

        public virtual string PotentialQuestorName()
        {
            //%pqn
            throw new NotImplementedException();
        }

        public virtual string PotentialQuestorLocation()
        {
            // %pqp
            throw new NotImplementedException();
        }


        public virtual TextFile.Token[] PotionRecipeIngredients(TextFile.Formatting format)
        {
            throw new NotImplementedException();
        }

        public virtual TextFile.Token[] MagicPowers(TextFile.Formatting format)
        {   // %mpw
            throw new NotImplementedException();
        }

        public virtual string DurationBase()
        {
            // %bdr
            throw new NotImplementedException();
        }

        public virtual string DurationPlus()
        {
            // %adr
            throw new NotImplementedException();
        }

        public virtual string DurationPerLevel()
        {
            // %cld
            throw new NotImplementedException();
        }

        public virtual string ChanceBase()
        {
            // %bch
            throw new NotImplementedException();
        }

        public virtual string ChancePlus()
        {
            // %ach
            throw new NotImplementedException();
        }

        public virtual string ChancePerLevel()
        {
            // %clc
            throw new NotImplementedException();
        }

        public virtual string MagnitudeBaseMin()
        {
            // %1bm
            throw new NotImplementedException();
        }

        public virtual string MagnitudeBaseMax()
        {
            // %2bm
            throw new NotImplementedException();
        }

        public virtual string MagnitudePlusMin()
        {
            // %1am
            throw new NotImplementedException();
        }

        public virtual string MagnitudePlusMax()
        {
            // %2am
            throw new NotImplementedException();
        }

        public virtual string MagnitudePerLevel()
        {
            // %clc
            throw new NotImplementedException();
        }

        public virtual string CommonersRep()
        {
            // %r1
            throw new NotImplementedException();
        }

        public virtual string MerchantsRep()
        {
            // %r2
            throw new NotImplementedException();
        }

        public virtual string ScholarsRep()
        {
            // %r3
            throw new NotImplementedException();
        }

        public virtual string NobilityRep()
        {
            // %r4
            throw new NotImplementedException();
        }

        public virtual string UnderworldRep()
        {
            // %r5
            throw new NotImplementedException();
        }
    }
}
