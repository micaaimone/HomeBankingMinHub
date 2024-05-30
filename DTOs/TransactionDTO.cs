﻿using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.DTOs
{
    public class TransactionDTO
    {
        public long Id { get; set; }

        public TransactionType Type { get; set; }

        public double Amount { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public TransactionDTO (Transaction transaction)
        {
            Id = transaction.Id;
            Type = transaction.Type;
            Amount = transaction.Amount;
            Description = transaction.Description;
            Date = transaction.Date;
        }
    }
}