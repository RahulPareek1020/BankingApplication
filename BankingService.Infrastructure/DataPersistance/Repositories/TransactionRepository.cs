using BankingService.Domain.Commands.Transaction;
using BankingService.Infrastructure.DataPersistance.Contracts;
using BankingService.Infrastructure.Entities;

namespace BankingService.Infrastructure.DataPersistance.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private const decimal MinBalance = 100;
        private const decimal MaxWithdrawPercent = 0.9m;
        private const decimal MaxDepositAmount = 10000m;

        public BankDbContext context;

        public TransactionRepository(BankDbContext context)
        {
            this.context = context;
        }

        public Task<string> DepositAmount(DepositAmountCommand command)
        {
            string message = string.Empty;
            var account = this.context.UserAccounts.First(x => x.AccountId == command.AccountId && x.UserId == command.UserId);

            //validation
            if (command.AmountToDeposit > MaxDepositAmount)
                message = "Deposit amount exceeds the limit.";
            else
            {
                account.AccountBalance += command.AmountToDeposit;
                this.context.SaveChanges();
                message = "Deposit Successful.";
            }

            return Task.FromResult(message);
        }

        public Task<string> WithdrawAmount(WithdrawAmountCommand command)
        {
            string message = string.Empty;
            var account = this.context.UserAccounts.First(x => x.AccountId == command.AccountId && x.UserId == command.UserId);

            //validation
            if (command.WithdrawalAmount > account.AccountBalance * MaxWithdrawPercent)
                message = "Withdraw amount exceeds the limit.";

            else if (account.AccountBalance - command.WithdrawalAmount < MinBalance)
                message = "Cannot withdraw. Minimum balance requirement not met.";

            else
            {
                account.AccountBalance -= command.WithdrawalAmount;
                this.context.SaveChanges();
                message = "Withdrawal Successful.";
            }

            return Task.FromResult(message);
        }
    }
}
