// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 
// Notes:
//

using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /**
     * Guild service UI for guild training.
     * Note this is not a real UI window, and is not actually pushed onto the stack. This
     * is so replacements are not constrained what to present first.
     */
    public class DaggerfallGuildServiceTraining : DaggerfallPopupWindow
    {
        public const int TrainingOfferId = 8;
        public const int TrainingTooSkilledId = 4022;
        public const int TrainingToSoonId = 4023;
        public const int TrainSkillId = 5221;

        protected GuildNpcServices NpcService { get; private set; }
        protected IGuild Guild { get; private set; }

        protected PlayerEntity playerEntity;

        public DaggerfallGuildServiceTraining(IUserInterfaceManager uiManager, GuildNpcServices npcService, IGuild guild)
            : base(uiManager, uiManager.TopWindow)
        {
            NpcService = npcService;
            Guild = guild;

            playerEntity = GameManager.Instance.PlayerEntity;

            TrainingService();
        }

        protected virtual void TrainingService()
        {
            // Check enough time has passed since last trained
            DaggerfallDateTime now = DaggerfallUnity.Instance.WorldTime.Now;
            if ((now.ToClassicDaggerfallTime() - playerEntity.TimeOfLastSkillTraining) < 720)
            {
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(TrainingToSoonId);
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                messageBox.SetTextTokens(tokens);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
            }
            else
            {   // Offer training price
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(TrainingOfferId);
                messageBox.SetTextTokens(tokens, Guild);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                messageBox.OnButtonClick += ConfirmTraining_OnButtonClick;
                messageBox.Show();
            }
        }

        protected virtual void ConfirmTraining_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                if (playerEntity.GetGoldAmount() >= Guild.GetTrainingPrice())
                {
                    // Show skill picker loaded with guild training skills
                    DaggerfallListPickerWindow skillPicker = new DaggerfallListPickerWindow(uiManager, this);
                    skillPicker.OnItemPicked += TrainingSkill_OnItemPicked;

                    foreach (DFCareer.Skills skill in GetTrainingSkills())
                        skillPicker.ListBox.AddItem(DaggerfallUnity.Instance.TextProvider.GetSkillName(skill));

                    uiManager.PushWindow(skillPicker);
                }
                else
                    DaggerfallUI.MessageBox(DaggerfallTradeWindow.NotEnoughGoldId);
            }
        }

        protected virtual void TrainingSkill_OnItemPicked(int index, string skillName)
        {
            CloseWindow();
            List<DFCareer.Skills> trainingSkills = GetTrainingSkills();
            DFCareer.Skills skillToTrain = trainingSkills[index];

            if (playerEntity.Skills.GetPermanentSkillValue(skillToTrain) > Guild.GetTrainingMax(skillToTrain))
            {
                // Inform player they're too skilled to train
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(TrainingTooSkilledId);
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                messageBox.SetTextTokens(tokens, Guild);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
            }
            else
            {
                // Take payment.
                playerEntity.DeductGoldAmount(Guild.GetTrainingPrice());
                // Train the skill
                TrainSkill(skillToTrain);
            }
        }

        protected void TrainSkill(DFCareer.Skills skillToTrain)
        {
            DaggerfallDateTime now = DaggerfallUnity.Instance.WorldTime.Now;
            playerEntity.TimeOfLastSkillTraining = now.ToClassicDaggerfallTime();
            now.RaiseTime(DaggerfallDateTime.SecondsPerHour * 3);
            playerEntity.DecreaseFatigue(PlayerEntity.DefaultFatigueLoss * 180);
            int skillAdvancementMultiplier = DaggerfallSkills.GetAdvancementMultiplier(skillToTrain);
            short tallyAmount = (short)(UnityEngine.Random.Range(10, 20 + 1) * skillAdvancementMultiplier);
            playerEntity.TallySkill(skillToTrain, tallyAmount);
            DaggerfallUI.MessageBox(TrainSkillId);
        }

        protected virtual List<DFCareer.Skills> GetTrainingSkills()
        {
            switch (NpcService)
            {
                // Handle Temples even when not a member
                case GuildNpcServices.TAk_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Akatosh);
                case GuildNpcServices.TAr_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Arkay);
                case GuildNpcServices.TDi_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Dibella);
                case GuildNpcServices.TJu_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Julianos);
                case GuildNpcServices.TKy_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Kynareth);
                case GuildNpcServices.TMa_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Mara);
                case GuildNpcServices.TSt_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Stendarr);
                case GuildNpcServices.TZe_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Zenithar);
                default:
                    return Guild.TrainingSkills;
            }
        }

    }
}
