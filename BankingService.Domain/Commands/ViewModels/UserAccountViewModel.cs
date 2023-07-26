namespace BankingService.Domain.Commands.ViewModels
{
    public class UserAccountViewModel
    {
        public int UserId { get; set; }

        public int AccountId { get; set; }

        public decimal AccountBalance { get; set; }
    }
}
