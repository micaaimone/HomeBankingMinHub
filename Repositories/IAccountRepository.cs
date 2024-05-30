﻿using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();

        Account FindAccountById(long id);
    }
}