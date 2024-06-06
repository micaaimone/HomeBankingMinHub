using HomeBankingMinHub.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMinHub.Repositories.Implementations
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        //constructor
        public CardRepository(HomeBankingContext RepositoryContext) : base(RepositoryContext)
        {
        }

        //metodos
        public void AddCard(Card card)
        {
            Create(card);
            SaveChanges();
        }

        public IEnumerable<Card> GetAllCards()
        {
            return FindAll().ToList();
        }

        public IEnumerable<Card> GetAllCardsByClient(long clientId)
        {
            return FindByCondition(a => a.ClientId == clientId).ToList();
        }

        public IEnumerable<Card> GetAllCardsByType(long clientId, string type)
        {
            return FindByCondition(a => a.ClientId == clientId && a.Type == type).ToList();
        }

        public Card GetCardById(long id)
        {
            return FindByCondition(a => a.Id == id).FirstOrDefault(); 
        }

        public Card GetCardByNumber(long clientId, string number)
        {
            return FindByCondition(a => a.ClientId == clientId && a.Number == number).FirstOrDefault();
        }
    }
}

