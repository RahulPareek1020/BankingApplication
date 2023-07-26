using MediatR;

namespace BankingService.Domain.Commands.Account
{
    public class AddAccountCommand : IRequest<string>
    {
        public int UserId { get; set; }

        public decimal Balance { get; set; }

        public void Validate()
        {
            if (Balance <= 0)
            {
                throw new Exception(string.Format("The minimum ammount to add should be greater than 0"));
            }
        }
    }
}
