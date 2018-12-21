// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Michael Rauter (Nystul)
// Contributors:    Numidium

using System;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Arena2;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using UnityEngine;
using DaggerfallConnect.FallExe;

namespace DaggerfallWorkshop.Game
{
    public partial class TalkManager : IMacroContextProvider
    {
        /// <summary>
        /// Oaths by race.
        /// </summary>
        enum RacialOaths
        {
            None = 0,
            Nord = 201,
            Khajiit = 202,
            Redguard = 203,
            Breton = 204,
            Argonian = 205,
            Bosmer = 206,
            Altmer = 207,
            Dunmer = 208,
        }

        public class TalkManagerContext
        {
            public ListItem currentQuestionListItem;
            public Races npcRace;
            public Genders potentialQuestorGender;
        }

        public MacroDataSource GetMacroDataSource()
        {
            TalkManagerContext context = new TalkManagerContext();
            context.currentQuestionListItem = currentQuestionListItem;
            context.npcRace = this.npcData.race;
            if (currentQuestionListItem != null && currentQuestionListItem.questionType == QuestionType.Work)
            {
                context.potentialQuestorGender = TalkManager.Instance.GetQuestorGender();
            }
            return new TalkManagerDataSource(context);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for items in Daggerfall Unity.
        /// </summary>
        private class TalkManagerDataSource : MacroDataSource
        {

            private TalkManagerContext parent;
            public TalkManagerDataSource(TalkManagerContext context)
            {
                this.parent = context;
            }

            public override string LocationDirection()
            {
                if (parent.currentQuestionListItem.questionType == QuestionType.LocalBuilding || parent.currentQuestionListItem.questionType == QuestionType.Person)
                {
                    return GameManager.Instance.TalkManager.GetKeySubjectLocationCompassDirection();
                }
                return TextManager.Instance.GetText(textDatabase, "resolvingError");
            }

            public override string DialogHint()
            {
                if (parent.currentQuestionListItem.questionType == QuestionType.LocalBuilding)
                {
                    return GameManager.Instance.TalkManager.GetKeySubjectLocationHint();
                }
                else if (parent.currentQuestionListItem.questionType == QuestionType.Person)
                {
                    return GameManager.Instance.TalkManager.GetKeySubjectPersonHint();
                }
                else if (parent.currentQuestionListItem.questionType == QuestionType.QuestLocation || parent.currentQuestionListItem.questionType == QuestionType.QuestPerson || parent.currentQuestionListItem.questionType == QuestionType.QuestItem)
                {
                    return GameManager.Instance.TalkManager.GetDialogHint(parent.currentQuestionListItem);
                }
                else if (parent.currentQuestionListItem.questionType == QuestionType.OrganizationInfo)
                {
                    return GameManager.Instance.TalkManager.GetOrganizationInfo(parent.currentQuestionListItem);
                }
                return TextManager.Instance.GetText(textDatabase, "resolvingError");
            }

            public override string DialogHint2()
            {
                if (parent.currentQuestionListItem.questionType == QuestionType.LocalBuilding)
                {
                    return GameManager.Instance.TalkManager.GetKeySubjectLocationHint();
                }
                else if (parent.currentQuestionListItem.questionType == QuestionType.Person)
                {
                    return GameManager.Instance.TalkManager.GetKeySubjectPersonHint();
                }
                else if (parent.currentQuestionListItem.questionType == QuestionType.QuestLocation || parent.currentQuestionListItem.questionType == QuestionType.QuestPerson || parent.currentQuestionListItem.questionType == QuestionType.QuestItem)
                {
                    return GameManager.Instance.TalkManager.GetDialogHint2(parent.currentQuestionListItem);
                }
                else if (parent.currentQuestionListItem.questionType == QuestionType.OrganizationInfo)
                {
                    return GameManager.Instance.TalkManager.GetOrganizationInfo(parent.currentQuestionListItem);
                }
                return TextManager.Instance.GetText(textDatabase, "resolvingError");
            }
            public override string Oath()
            {
                RacialOaths whichOath = RacialOaths.None;
                switch (parent.npcRace)
                {
                    case Races.Argonian:
                        whichOath = RacialOaths.Argonian;
                        break;
                    case Races.Breton:
                        whichOath = RacialOaths.Breton;
                        break;
                    case Races.DarkElf:
                        whichOath = RacialOaths.Dunmer;
                        break;
                    case Races.HighElf:
                        whichOath = RacialOaths.Altmer;
                        break;
                    case Races.Khajiit:
                        whichOath = RacialOaths.Khajiit;
                        break;
                    case Races.Nord:
                        whichOath = RacialOaths.Nord;
                        break;
                    case Races.Redguard:
                        whichOath = RacialOaths.Redguard;
                        break;
                    //case Races.Vampire:                       // TODO: Restore this via racial override effect
                    //    whichOath = RacialOaths.Dunmer;
                    //    break;
                    case Races.WoodElf:
                        whichOath = RacialOaths.Bosmer;
                        break;
                }
                return DaggerfallUnity.Instance.TextProvider.GetRandomText((int)whichOath);
            }

            // He/She
            public override string Pronoun()
            {
                switch (parent.potentialQuestorGender)
                {
                default:
                case Game.Entity.Genders.Male:
                    return HardStrings.pronounHe;
                case Game.Entity.Genders.Female:
                    return HardStrings.pronounShe;
                }
            }

            // Him/Her
            public override string Pronoun2()
            {
                switch (parent.potentialQuestorGender)
                {
                default:
                case Game.Entity.Genders.Male:
                    return HardStrings.pronounHim;
                case Game.Entity.Genders.Female:
                    return HardStrings.pronounHer;
                }
            }

            public override string PotentialQuestorName()
            {
                return TalkManager.Instance.GetQuestorName();
            }

            public override string PotentialQuestorLocation()
            {
                return TalkManager.Instance.GetQuestorLocation();
            }
        }
    }
}