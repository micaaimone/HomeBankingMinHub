using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetAllTransactions();

        Transaction GetTransactionById(int id);

        void Save(Transaction transaction);

    }
}
