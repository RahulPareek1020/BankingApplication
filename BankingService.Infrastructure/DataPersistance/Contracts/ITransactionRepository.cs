using BankingService.Domain.Commands.Transaction;

namespace BankingService.Infrastructure.DataPersistance.Contracts
{
    public interface ITransactionRepository
    {
        public Task<string> DepositAmount(DepositAmountCommand command);

        public Task<string> WithdrawAmount(WithdrawAmountCommand command);
    }
}
