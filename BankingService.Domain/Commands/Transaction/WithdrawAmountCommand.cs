using MediatR;

namespace BankingService.Domain.Commands.Transaction
{
    public class WithdrawAmountCommand : IRequest<string>
    {
        public int AccountId { get; set; }

        public int UserId { get; set; }

        public decimal WithdrawalAmount { get; set; }
    }
}
