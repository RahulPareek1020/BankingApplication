using BankingService.Domain.Commands.ViewModels;
using BankingService.Domain.Queries.Account;
using BankingService.Infrastructure.DataPersistance.Contracts;
using MediatR;

namespace BankingService.Business.QueryHandlers
{
    public class AccountQueryHandler : IRequestHandler<GetAccountByUserIdQuery, List<UserAccountViewModel>>
    {
        private readonly IAccountRepository accountRepository;
        //private readonly IMapper mapper;

        public AccountQueryHandler(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public async Task<List<UserAccountViewModel>> Handle(GetAccountByUserIdQuery query, CancellationToken cancellationToken)
        {
            var result = await this.accountRepository.GetAccountByUserId(query);

            return result;
        }
    }
}
