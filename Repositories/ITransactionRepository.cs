using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetAllTransactions();

        Transaction GetTransaction(int id);
    }
}
