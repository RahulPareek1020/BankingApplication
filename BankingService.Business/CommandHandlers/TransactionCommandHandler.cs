using BankingService.Domain.Commands.Transaction;
using BankingService.Infrastructure.DataPersistance.Contracts;
using MediatR;

namespace BankingService.Business.CommandHandlers
{
    //public class TransactionCommandHandler : IRequestHandler<DepositAmountCommand, DepositAmountCommand>,
    //                                         IRequestHandler<WithdrawAmountCommand, WithdrawAmountCommand>
    public class TransactionCommandHandler : IRequestHandler<DepositAmountCommand, string>,
                                             IRequestHandler<WithdrawAmountCommand, string>
    {
        private readonly ITransactionRepository transactionRepository;
        //private readonly IMapper mapper;

        public TransactionCommandHandler(ITransactionRepository transactionRepository)
        {
            this.transactionRepository = transactionRepository;
        }

        //public Task<DepositAmountCommand> Handle(DepositAmountCommand command, CancellationToken cancellationToken)
        public Task<string> Handle(DepositAmountCommand command, CancellationToken cancellationToken)
        {
            var result = this.transactionRepository.DepositAmount(command);
            return result;
        }

        //public Task<WithdrawAmountCommand> Handle(WithdrawAmountCommand command, CancellationToken cancellationToken)
        public Task<string> Handle(WithdrawAmountCommand command, CancellationToken cancellationToken)
        {
            //command.Validate();
            var result = this.transactionRepository.WithdrawAmount(command);
            return result;
        }
    }
}
