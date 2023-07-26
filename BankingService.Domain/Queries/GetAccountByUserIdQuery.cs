using BankingService.Domain.Commands.ViewModels;
using MediatR;

namespace BankingService.Domain.Queries.Account
{
    public class GetAccountByUserIdQuery : IRequest<List<UserAccountViewModel>>
    {
        public int UserId { get; set; }
    }
}
