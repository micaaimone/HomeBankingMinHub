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

        //con este agarro las transacciones 
        public Account FindByNumber(string number)
        {
            return FindByCondition(account => account.Number.ToUpper() == number.ToUpper())
                       .Include(account => account.Transactions)
                       .FirstOrDefault();
        }

        //con este agarro la cuenta en si 
        public Account GetAccountByNumber(string number)
        {
            return FindByCondition(n => n.Number == number).FirstOrDefault();
        }

        public IEnumerable<Account> GetAccountsByClient(long clientId)
        {
            return FindByCondition(a => a.ClientId == clientId)
                .Include(a => a.Transactions)
                .ToList();
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll().Include(a => a.Transactions).ToList();
        }

        public void Save(Account account)
        {
            Create(account);
            SaveChanges();
        }

        public void UpdateAccount(Account account)
        {
            Update(account);
            SaveChanges();
            //lo hacemos para que no se quede en cache esperando mas datos
            RepositoryContext.ChangeTracker.Clear();
        }
    }
}
