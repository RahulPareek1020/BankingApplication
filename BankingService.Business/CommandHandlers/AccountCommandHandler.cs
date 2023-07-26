using AutoMapper;
using BankingService.Domain.Commands.Account;
using BankingService.Infrastructure.DataPersistance.Contracts;
using MediatR;

namespace BankingService.Business.CommandHandlers
{
    //public class AccountCommandHandler : IRequestHandler<AddAccountCommand, List<UserAccountViewModel>, 
    //                                     IRequestHandler<DeleteAccountCommand, List<UserAccountViewModel>>
    public class AccountCommandHandler : IRequestHandler<AddAccountCommand, string>,
                                         IRequestHandler<DeleteAccountCommand, string>
    {
        private readonly IAccountRepository accountRepository;
        private readonly IMapper mapper;

        public AccountCommandHandler(IAccountRepository accountRepository, IMapper mapper)
        {
            this.accountRepository = accountRepository;
            this.mapper = mapper;
        }

        public Task<string> Handle(AddAccountCommand command, CancellationToken cancellationToken)
        {
            //command.Validate();
            var result = this.accountRepository.AddAccount(command);

            //return this.mapper.Map<List<UserAccountViewModel>>(result);
            return result;
        }

        public Task<string> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
        {
            //command.Validate();
            var result = this.accountRepository.DeleteAccount(command);
            return result;
        }
    }
}