using BankingService.Domain.Commands.Account;
using BankingService.Domain.Commands.ViewModels;
using BankingService.Domain.Queries.Account;

namespace BankingService.Infrastructure.DataPersistance.Contracts
{
    public interface IAccountRepository
    {
        Task<string> AddAccount(AddAccountCommand command);

        Task<string> DeleteAccount(DeleteAccountCommand command);

        Task<List<UserAccountViewModel>> GetAccountByUserId(GetAccountByUserIdQuery query);
    }
}
