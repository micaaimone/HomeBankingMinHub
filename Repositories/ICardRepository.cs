using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAllCards();
        IEnumerable<Card> GetAllCardsByClient(long clientId);
        IEnumerable<Card> GetAllCardsByType(long clientId, string type);
        Card GetCardByNumber(long clientId, string number);
        Card GetCardById(long id);
        void AddCard(Card card);
    }
}
