using System;
using System.Collections.Generic;
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
                long paymentDueMinutes = DaggerfallBankManager.GetLoanDueDate(regionIndex);

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
                        long lastRemainingMonths = (paymentDueMinutes - lastGameMinutes) / MinutesPerMonth;
                        long remainingMonths = (paymentDueMinutes - gameMinutes) / MinutesPerMonth;
                        if (remainingMonths < lastRemainingMonths)
                        {
                            // Send letters 6, 3 and 1 months before due date instead?
                            DaggerfallUI.AddHUDText(String.Format(TextManager.Instance.GetText(textDatabase, "loanReminder"),
                                DaggerfallBankManager.GetLoanedTotal(regionIndex)), loanReminderHUDDelay);
                            DaggerfallUI.AddHUDText(String.Format(TextManager.Instance.GetText(textDatabase, "loanReminder2"),
                                remainingMonths + 1, MapsFile.RegionNames[regionIndex]), loanReminderHUDDelay);
                        }
                    }
                }
            }
        }

        private static void OverdueLoan(int regionIndex)
        {
            int transferAmount = (int)Math.Min(DaggerfallBankManager.GetLoanedTotal(regionIndex), DaggerfallBankManager.GetAccountTotal(regionIndex));
            DaggerfallBankManager.MakeTransaction(TransactionType.Repaying_loan_from_account, transferAmount, regionIndex);
            if (!DaggerfallBankManager.HasLoan(regionIndex))
                return;

            // Set hasDefaulted flag (Note: Does not seem to ever be set in classic)
            DaggerfallBankManager.SetDefaulted(regionIndex, true);
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            playerEntity.LowerRepForCrime(regionIndex, PlayerEntity.Crimes.LoanDefault);
        }

        internal static List<TextFile.Token[]> GetMessages()
        {
            List<TextFile.Token[]> messages = new List<TextFile.Token[]>();
            bool found = false;
            messages.Add(GetLoansLine("Region", "Amount", "Due Date"));
            messages.Add(new TextFile.Token[] { });
            for (int regionIndex = 0; regionIndex < DaggerfallBankManager.BankAccounts.Length; regionIndex++)
            {
                if (DaggerfallBankManager.HasLoan(regionIndex))
                {
                    messages.Add(GetLoansLine(MapsFile.RegionNames[regionIndex], DaggerfallBankManager.GetLoanedTotal(regionIndex).ToString(), DaggerfallBankManager.GetLoanDueDateString(regionIndex)));
                    found = true;
                }
            }
            if (!found)
            {
                TextFile.Token noneToken = TextFile.CreateTextToken("None");
                messages.Add(new TextFile.Token[] { noneToken });
            }
            return messages;
        }

        private static TextFile.Token[] GetLoansLine(string region, string amount, string duedate)
        {
            List<TextFile.Token> tokens = new List<TextFile.Token>();

            TextFile.Token positioningToken = TextFile.TabToken;

            tokens.Add(TextFile.CreateTextToken(region));
            positioningToken.x = 60;
            tokens.Add(positioningToken);
            tokens.Add(TextFile.CreateTextToken(amount));
            positioningToken.x = 130;
            tokens.Add(positioningToken);
            tokens.Add(TextFile.CreateTextToken(duedate));
            tokens.Add(TextFile.NewLineToken);
            return tokens.ToArray();
        }
    }
}
