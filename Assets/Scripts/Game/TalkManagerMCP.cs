// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Michael Rauter (Nystul)
// Contributors:    Numidium

using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game
{
    public partial class TalkManager : IMacroContextProvider
    {

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

            public override string Name()
            {
                // Used for greeting messages only: 7215, 7216, 7217
                if (!string.IsNullOrEmpty(GameManager.Instance.TalkManager.GreetingNameNPC))
                    return GameManager.Instance.TalkManager.GreetingNameNPC;

                return MacroHelper.GetRandomFullName();
            }

            public override string Direction()
            {
                if (parent.currentQuestionListItem.questionType == QuestionType.LocalBuilding || parent.currentQuestionListItem.questionType == QuestionType.Person)
                {
                    return GameManager.Instance.TalkManager.GetKeySubjectLocationCompassDirection();
                }
                return TextManager.Instance.GetLocalizedText("resolvingError");
            }

            public override string DialogHint()
            {
                if (parent.currentQuestionListItem.questionType == QuestionType.LocalBuilding)
                {
                    return GameManager.Instance.TalkManager.GetKeySubjectBuildingHint();
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
                return TextManager.Instance.GetLocalizedText("resolvingError");
            }

            public override string DialogHint2()
            {
                if (parent.currentQuestionListItem.questionType == QuestionType.LocalBuilding)
                {
                    return GameManager.Instance.TalkManager.GetKeySubjectBuildingHint();
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
                return TextManager.Instance.GetLocalizedText("resolvingError");
            }

            // Oaths are declared by NPC race according to the race index used in FACTION.TXT.
            // Unfortunately, classic never uses this faction race ID but, instead, uses the
            // hardcoded index race of each region, which is not the same. In classic, this
            // results in all NPCs from High Rock saying Nord oaths, while all NPCs in Hammerfell
            // will say Khajiit oaths.
            // Instead, DFU uses the faction race ID to return the correct oath.
            // For the list of oaths, see https://www.imperial-library.info/content/daggerfall-oaths-and-expletives
            public override string Oath()
            {
                // Get NPC race with fallback to race of current region
                Races race = parent.npcRace;
                if (race == Races.None)
                    race = GameManager.Instance.PlayerGPS.GetRaceOfCurrentRegion();

                int oathId = (int)RaceTemplate.GetFactionRaceFromRace(race);

                return DaggerfallUnity.Instance.TextProvider.GetRandomText(201 + oathId);
            }

            // He/She
            public override string Pronoun()
            {
                switch (parent.potentialQuestorGender)
                {
                default:
                case Game.Entity.Genders.Male:
                    return TextManager.Instance.GetLocalizedText("pronounHe");
                case Game.Entity.Genders.Female:
                    return TextManager.Instance.GetLocalizedText("pronounShe");
                }
            }

            // Him/Her
            public override string Pronoun2()
            {
                switch (parent.potentialQuestorGender)
                {
                default:
                case Game.Entity.Genders.Male:
                    return TextManager.Instance.GetLocalizedText("pronounHim");
                case Game.Entity.Genders.Female:
                    return TextManager.Instance.GetLocalizedText("pronounHer");
                }
            }

            // His/Her
            public override string Pronoun3()
            {
                switch (parent.potentialQuestorGender)
                {
                    default:
                    case Game.Entity.Genders.Male:
                        return TextManager.Instance.GetLocalizedText("pronounHis");
                    case Game.Entity.Genders.Female:
                        return TextManager.Instance.GetLocalizedText("pronounHer");
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