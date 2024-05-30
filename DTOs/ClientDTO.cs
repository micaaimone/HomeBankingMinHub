﻿using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.DTOs
{
    public class ClientDTO
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<AccountClientDTO> Accounts { get; set; }

       
        public ClientDTO(Client client)
        {
            FirstName = client.FirstName;
            LastName = client.LastName;
            Email = client.Email;
            Password = client.Password;
            Accounts = client.Accounts.Select(a => new AccountClientDTO(a)).ToList();
        }
    }
}
