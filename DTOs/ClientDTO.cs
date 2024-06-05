﻿using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.DTOs
{
    public class ClientDTO
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public ICollection<AccountClientDTO> Accounts { get; set; }
        public ICollection<ClientLoanDTO> Loans { get; set; }
        public ICollection<CardDTO> Cards { get; set; }


        public ClientDTO(Client client)
        {
            FirstName = client.FirstName;
            LastName = client.LastName;
            Email = client.Email;
            Accounts = client.Accounts.Select(a => new AccountClientDTO(a)).ToList();
            Loans = client.ClientLoans.Select(c => new ClientLoanDTO(c)).ToList();
            Cards = client.Cards.Select(card => new CardDTO(card)).ToList();
        }
    }
}
