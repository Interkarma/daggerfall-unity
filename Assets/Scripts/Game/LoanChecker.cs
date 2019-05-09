using System;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallWorkshop.Game.Entity;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public class LoanChecker : MonoBehaviour
    {
        public LoanChecker()
        {
        }

        public void Start()
        {
            WorldTime.OnMidnight += CheckOverdueLoans;
        }

        private void CheckOverdueLoans()
        {
            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            for (int regionIndex = 0; regionIndex < DaggerfallBankManager.BankAccounts.Length; regionIndex++)
            {
                uint paymentDueMinutes = DaggerfallBankManager.BankAccounts[regionIndex].loanDueDate;

                if (paymentDueMinutes != 0 && paymentDueMinutes < gameMinutes)
                {
                    Debug.Log("loan due in region " + regionIndex + ": " + paymentDueMinutes + " < " + gameMinutes);
                    DueLoan(regionIndex);
                }
            }
        }

        private static void DueLoan(int regionIndex)
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
