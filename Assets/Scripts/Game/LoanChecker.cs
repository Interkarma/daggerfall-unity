using System;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public static class LoanChecker
    {
        const int MinutesPerMonth = DaggerfallDateTime.MinutesPerDay * DaggerfallDateTime.DaysPerMonth;

        const float loanReminderHUDDelay = 3;

        public static void CheckOverdueLoans(uint lastGameMinutes)
        {
            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            for (int regionIndex = 0; regionIndex < DaggerfallBankManager.BankAccounts.Length; regionIndex++)
            {
                long paymentDueMinutes = DaggerfallBankManager.GetLoanDueDate(regionIndex);

                if (paymentDueMinutes != 0)
                {
                    if (paymentDueMinutes < gameMinutes)
                    {
                        Debug.Log("loan overdue " + paymentDueMinutes + " < " + gameMinutes);
                        OverdueLoan(regionIndex);
                    }
                    else
                    {
                        long lastRemainingMonths = (paymentDueMinutes - lastGameMinutes) / MinutesPerMonth;
                        long remainingMonths = (paymentDueMinutes - gameMinutes) / MinutesPerMonth;
                        if (remainingMonths < lastRemainingMonths)
                        {
                            // Months left before due date
                            int[] sendReminderMonths = { 6, 3, 1 };
                            if (Array.Exists(sendReminderMonths, month => lastRemainingMonths >= month && remainingMonths < month))
                            {
                                // Send letters before due date instead?
                                DaggerfallUI.AddHUDText(String.Format(TextManager.Instance.GetLocalizedText("loanReminder"),
                                    DaggerfallBankManager.GetLoanedTotal(regionIndex)), loanReminderHUDDelay);
                                DaggerfallUI.AddHUDText(String.Format(TextManager.Instance.GetLocalizedText("loanReminder2"),
                                    remainingMonths + 1, MapsFile.RegionNames[regionIndex]), loanReminderHUDDelay);
                            }
                        }
                    }
                }
            }
        }

        private static void OverdueLoan(int regionIndex)
        {
            // Try to repay the loan off player's account
            int transferAmount = (int)Math.Min(DaggerfallBankManager.GetLoanedTotal(regionIndex), DaggerfallBankManager.GetAccountTotal(regionIndex));
            DaggerfallBankManager.MakeTransaction(TransactionType.Repaying_loan_from_account, transferAmount, regionIndex);
            if (!DaggerfallBankManager.HasLoan(regionIndex))
                return;

            // Only apply reputation drop once
            if (DaggerfallBankManager.HasDefaulted(regionIndex))
                return;

            // Set hasDefaulted flag (Note: Does not seem to ever be set in classic)
            DaggerfallBankManager.SetDefaulted(regionIndex, true);

            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            // Should that be weighted by the amount?
            playerEntity.LowerRepForCrime(regionIndex, PlayerEntity.Crimes.LoanDefault);
        }
    }
}
