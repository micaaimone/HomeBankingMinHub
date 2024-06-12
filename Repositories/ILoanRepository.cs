using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public interface ILoanRepository
    {
        IEnumerable<Loan> GetAll();
        Loan GetLoanById(long id);
    }
}
