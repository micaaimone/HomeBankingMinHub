using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories.Implementations
{
    public class ClientLoanRepository : RepositoryBase<ClientLoan>, IClientLoanRepository
    {
        public ClientLoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {

        }
        public void SaveClientLoan(ClientLoan clientLoan)
        {
            Create(clientLoan);
            SaveChanges();
        }
    }
}
