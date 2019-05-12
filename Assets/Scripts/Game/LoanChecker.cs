using System;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public class LoanChecker : MonoBehaviour
    {
        const int MinutesPerMonth = DaggerfallDateTime.MinutesPerDay * DaggerfallDateTime.DaysPerMonth;

        const string textDatabase = "DaggerfallUI";
        const float loanReminderHUDDelay = 3;

        private LoanChecker()
        {
        }

        public static void CheckOverdueLoans(uint lastGameMinutes)
        {
            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            for (int regionIndex = 0; regionIndex < DaggerfallBankManager.BankAccounts.Length; regionIndex++)
            {
                uint paymentDueMinutes = DaggerfallBankManager.BankAccounts[regionIndex].loanDueDate;

                if (paymentDueMinutes != 0)
                {
                    Debug.Log("Loan in region " + regionIndex);
                    if (paymentDueMinutes < gameMinutes)
                    {
                        Debug.Log("loan overdue " + paymentDueMinutes + " < " + gameMinutes);
                        OverdueLoan(regionIndex);
                    }
                    else
                    {
                        uint lastRemainingMonths = (paymentDueMinutes - lastGameMinutes) / MinutesPerMonth;
                        uint remainingMonths = (paymentDueMinutes - gameMinutes) / MinutesPerMonth;
                        if (remainingMonths < lastRemainingMonths)
                        {
                            // Send letters 5, 3 and 1 months before due date instead?
                            DaggerfallUI.AddHUDText(String.Format(TextManager.Instance.GetText(textDatabase, "loanReminder"), 
                                DaggerfallBankManager.BankAccounts[regionIndex].loanTotal, remainingMonths, MapsFile.RegionNames[regionIndex]), loanReminderHUDDelay);
                        }
                    }
                }
            }
        }

        private static void OverdueLoan(int regionIndex)
        {
            Serialization.BankRecordData_v1 account = DaggerfallBankManager.BankAccounts[regionIndex];
            int transferAmount = Mathf.Min(account.loanTotal, account.accountGold);
            DaggerfallBankManager.MakeTransaction(TransactionType.Repaying_loan_from_account, transferAmount, regionIndex);
            if (!DaggerfallBankManager.HasLoan(regionIndex))
                return;

            // Set hasDefaulted flag (Note: Does not seem to ever be set in classic)
            account.hasDefaulted = true;
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            playerEntity.LowerRepForCrime(regionIndex, PlayerEntity.Crimes.LoanDefault);
        }
    }
}
