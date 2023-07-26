using System.ComponentModel.DataAnnotations;

namespace Infrastucture.Entities
{
    public class User
    {

        public User()
        {
            Accounts = new HashSet<UserAccount>();
        }

        [Key]
        public int UserId { get; set; }

        public string Name { get; set; }


        public virtual ICollection<UserAccount> Accounts { get; set; }
    }
}
