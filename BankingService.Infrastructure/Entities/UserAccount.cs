using System.ComponentModel.DataAnnotations;

namespace Infrastucture.Entities
{
    public class UserAccount
    {
        [Key]
        public int AccountId { get; set; }

        public int UserId { get; set; }

        public decimal AccountBalance { get; set; }
    }
}
