// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
        public virtual string Name()
        {   // %n %nam %bn
            throw new NotImplementedException();
        }

        public virtual string GuildTitle()
        {   // %lev %pct
            throw new NotImplementedException();
        }

        public virtual string FactionOrderName()
        {   // %fon %kno
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

        public virtual string Daedra()
        {   // %dae
            throw new NotImplementedException();
        }

        public virtual string Direction()
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

        public virtual string HomeProvinceName()
        {
            // %hpn
            throw new NotImplementedException();
        }

        public virtual string GeographicalFeature()
        {
            // %hpw
            throw new NotImplementedException();
        }

        public virtual string VampireNpcClan()
        {   // %vcn
            throw new NotImplementedException();
        }

        public virtual string Q1()
        {
            // %q1
            throw new NotImplementedException();
        }

        public virtual string Q2()
        {
            // %q2
            throw new NotImplementedException();
        }

        public virtual string Q3()
        {
            // %q3
            throw new NotImplementedException();
        }

        public virtual string Q4()
        {
            // %q4
            throw new NotImplementedException();
        }

        public virtual string Q5()
        {
            // %q5
            throw new NotImplementedException();
        }

        public virtual string Q6()
        {
            // %q6
            throw new NotImplementedException();
        }

        public virtual string Q7()
        {
            // %q7
            throw new NotImplementedException();
        }

        public virtual string Q8()
        {
            // %q8
            throw new NotImplementedException();
        }

        public virtual string Q9()
        {
            // %q9
            throw new NotImplementedException();
        }

        public virtual string Q10()
        {
            // %q10
            throw new NotImplementedException();
        }

        public virtual string Q11()
        {
            // %q11
            throw new NotImplementedException();
        }

        public virtual string Q12()
        {
            // %q12
            throw new NotImplementedException();
        }

        public virtual string Q1a()
        {
            // %q1a
            throw new NotImplementedException();
        }

        public virtual string Q2a()
        {
            // %q2a
            throw new NotImplementedException();
        }

        public virtual string Q3a()
        {
            // %q3a
            throw new NotImplementedException();
        }

        public virtual string Q4a()
        {
            // %q4a
            throw new NotImplementedException();
        }

        public virtual string Q5a()
        {
            // %q5a
            throw new NotImplementedException();
        }

        public virtual string Q6a()
        {
            // %q6a
            throw new NotImplementedException();
        }

        public virtual string Q7a()
        {
            // %q7a
            throw new NotImplementedException();
        }

        public virtual string Q8a()
        {
            // %q8a
            throw new NotImplementedException();
        }

        public virtual string Q9a()
        {
            // %q9a
            throw new NotImplementedException();
        }

        public virtual string Q10a()
        {
            // %q10a
            throw new NotImplementedException();
        }

        public virtual string Q11a()
        {
            // %q11a
            throw new NotImplementedException();
        }

        public virtual string Q12a()
        {
            // %q12a
            throw new NotImplementedException();
        }

        public virtual string Q1b()
        {
            // %q1b
            throw new NotImplementedException();
        }

        public virtual string Q2b()
        {
            // %q2b
            throw new NotImplementedException();
        }

        public virtual string Q3b()
        {
            // %q3b
            throw new NotImplementedException();
        }

        public virtual string Q4b()
        {
            // %q4b
            throw new NotImplementedException();
        }

        public virtual string Q5b()
        {
            // %q5b
            throw new NotImplementedException();
        }

        public virtual string Q6b()
        {
            // %q6b
            throw new NotImplementedException();
        }

        public virtual string Q7b()
        {
            // %q7b
            throw new NotImplementedException();
        }

        public virtual string Q8b()
        {
            // %q8b
            throw new NotImplementedException();
        }

        public virtual string Q9b()
        {
            // %q9b
            throw new NotImplementedException();
        }

        public virtual string Q10b()
        {
            // %q10b
            throw new NotImplementedException();
        }

        public virtual string Q11b()
        {
            // %q11b
            throw new NotImplementedException();
        }

        public virtual string Q12b()
        {
            // %q12b
            throw new NotImplementedException();
        }

        public virtual string ImperialName()
        {
            // %imp
            throw new NotImplementedException();
        }

        public virtual string FemaleName()
        {
            // %fn %fn2
            throw new NotImplementedException();
        }

        public virtual string MaleName()
        {
            // %mn %mn2
            throw new NotImplementedException();
        }
    }
}
