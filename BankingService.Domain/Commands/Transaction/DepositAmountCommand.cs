using MediatR;

namespace BankingService.Domain.Commands.Transaction
{
    public class DepositAmountCommand : IRequest<string>
    {
        public int UserId { get; set; }

        public int AccountId { get; set; }

        public decimal AmountToDeposit { get; set; }
    }
}
