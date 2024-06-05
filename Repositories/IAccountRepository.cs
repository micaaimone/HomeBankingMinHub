using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();

        Account FindAccountById(long id);

        void Save(Account account);

        IEnumerable<Account> GetAccountsByClient(long clientId);
    }
}
