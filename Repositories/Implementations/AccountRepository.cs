using HomeBankingMinHub.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMinHub.Repositories.Implementations
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        //constructor
        public AccountRepository(HomeBankingContext RepositoryContext) : base(RepositoryContext)
        {
            
        }

        public Account FindAccountById(long id)
        {
            return FindByCondition(a => a.Id == id)
                .Include(a => a.Transactions)
                .FirstOrDefault();
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll().Include(a => a.Transactions).ToList();
        }
    }
}
