using HomeBankingMinHub.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMinHub.Repositories.Implementations
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {

        //constructor
        public TransactionRepository(HomeBankingContext RepositoryContext) : base(RepositoryContext)
        {
            
        }
        public IEnumerable<Transaction> GetAllTransactions()
        {
            return FindAll().Include(t => t.Amount).ToList();
        }

        public Transaction GetTransactionById(int id)
        {
            return FindByCondition(t => t.Id == id)
                            .Include(t => t.Amount)
                            .FirstOrDefault();
        }

        public void Save(Transaction transaction)
        {
            Create(transaction);
            SaveChanges();
        }
    }
}
