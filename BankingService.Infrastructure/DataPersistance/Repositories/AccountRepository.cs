using BankingService.Domain.Commands.Account;
using BankingService.Domain.Commands.ViewModels;
using BankingService.Domain.Queries.Account;
using BankingService.Infrastructure.DataPersistance.Contracts;
using BankingService.Infrastructure.Entities;
using Infrastucture.Entities;

namespace BankingService.Infrastructure.DataPersistance.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public List<UserAccount> accounts;
        public List<User> users;
        public BankDbContext context;

        public AccountRepository(BankDbContext context)
        {
            this.SeedData(context);

            this.context = context;
        }

        private void SeedData(BankDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(new List<User>()
                {
                     new User { UserId = 1 , Name = "User1" },
                     new User { UserId = 2 , Name = "User2" },
                });

                context.UserAccounts.AddRange(new List<UserAccount>()
                {
                    new UserAccount { UserId = 1, AccountId = 11, AccountBalance = 100000 },
                     new UserAccount { UserId = 1, AccountId = 12, AccountBalance = 500000 },
                     new UserAccount { UserId = 2, AccountId = 13, AccountBalance = 200000 },
                });

                context.SaveChanges();
            }
        }

        public Task<string> AddAccount(AddAccountCommand command)
        {
            var user = this.context.Users.First(u => u.UserId == command.UserId);

            if (user != null)
            {
                int nextAccountId = this.context.UserAccounts.Max(c => c.AccountId) + 1;

                this.context.UserAccounts.Add(new UserAccount
                {
                    UserId = command.UserId,
                    AccountBalance = command.Balance,
                    AccountId = nextAccountId
                });

                this.context.SaveChanges();
            }

            return Task.FromResult("Account Added Successfully");
        }

        public Task<string> DeleteAccount(DeleteAccountCommand command)
        {
            var account = this.context.UserAccounts.FirstOrDefault(acc => acc.AccountId == command.AccountId && acc.UserId == command.UserId);
            if (account != null)
            {
                this.context.UserAccounts.Remove(account);
                this.context.SaveChanges();
            }

            return Task.FromResult("Account Deleted Successfully");
        }


        public Task<List<UserAccountViewModel>> GetAccountByUserId(GetAccountByUserIdQuery query)
        {
            var userAccounts = from account in this.context.UserAccounts
                               where account.UserId == query.UserId
                               select new UserAccountViewModel
                               {
                                   UserId = account.UserId,
                                   AccountId = account.AccountId,
                                   AccountBalance = account.AccountBalance
                               };

            return Task.FromResult(userAccounts.ToList());
        }
    }
}
