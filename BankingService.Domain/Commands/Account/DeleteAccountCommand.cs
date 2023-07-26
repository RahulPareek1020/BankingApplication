using MediatR;

namespace BankingService.Domain.Commands.Account
{
    public class DeleteAccountCommand : IRequest<string>
    {
        public int UserId { get; set; }
        public int AccountId { get; set; }
    }
}
