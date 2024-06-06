using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();

        Account FindAccountById(long id);

        Account GetAccountByNumber (string number);

        void Save(Account account);

        IEnumerable<Account> GetAccountsByClient(long clientId);


    }
}
