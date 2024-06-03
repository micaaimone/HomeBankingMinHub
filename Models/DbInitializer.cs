namespace HomeBankingMinHub.Models
{
    public class DbInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clientes.Any())
            {
                var clientes = new Client[]
                {
                    new Client{FirstName = "Juan", LastName = "Sanchez", Email = "juan@gmail.com", Password = "123"},
                    new Client{FirstName = "Pepe", LastName = "Gomez", Email = "pepe@gmail.com", Password = "456"},
                    new Client{FirstName = "Caro", LastName = "Perez", Email = "caro@gmail.com", Password = "789" }
                };
                context.Clientes.AddRange(clientes);

                context.SaveChanges();
            }
           
            if (!context.Accounts.Any())
            {
                Client juanCliente = context.Clientes.FirstOrDefault(cl => cl.Email == "juan@gmail.com");
                if (juanCliente != null)
                {
                    var juanAccounts = new Account[]
                    {
                        new Account {ClientId = juanCliente.Id , CreationDate = DateTime.Now, Number = "VIN001", Balance = 10000 },
                        new Account {ClientId = juanCliente.Id , CreationDate = DateTime.Now, Number = "VIN002", Balance = 150000 }
                    };
                    context.Accounts.AddRange(juanAccounts);
                    context.SaveChanges();
                }
            }

            if (!context.Transactions.Any())
            {
                var account1 = context.Accounts.FirstOrDefault(c => c.Number == "VIN001");
                if (account1 != null)
                {
                    var transactions = new Transaction[]
                    {
                        new Transaction {AccountId= account1.Id, Amount = 30000, Date= DateTime.Now.AddHours(-5), Description = "Transferencia recibida", Type = TransactionType.CREDIT},
                        new Transaction {AccountId = account1.Id,Amount = 100, Date = DateTime.Now.AddHours(-3), Description = "Compra en tienda xxxx", Type = TransactionType.DEBIT},
                        new Transaction {AccountId= account1.Id, Amount = 55000, Date= DateTime.Now.AddHours(-6), Description = "Transferencia recibida", Type = TransactionType.CREDIT},
                        new Transaction {AccountId= account1.Id, Amount = 27000, Date= DateTime.Now.AddHours(-4), Description = "Compra en ML", Type = TransactionType.CREDIT}
                    };
                    context.Transactions.AddRange(transactions);
                    context.SaveChanges();
                }
            }

            if (!context.Loans.Any())
            {
                var loans = new Loan[]
                {
                    new Loan { Name = "Hipotecario", MaxAmount = 500000, Payments = "12,24,36,48,60" },
                    new Loan { Name = "Personal", MaxAmount = 100000, Payments = "6,12,24" },
                    new Loan { Name = "Automotriz", MaxAmount = 300000, Payments = "6,12,24,36" },
                };
                context.Loans.AddRange(loans);
                context.SaveChanges();
            };
            
            if (!context.ClientLoans.Any())
            {
                var client1 = context.Clientes.FirstOrDefault(cl => cl.Email == "juan@gmail.com");
                if(client1 != null)
                {
                    var loan1 = context.Loans.FirstOrDefault(I => I.Name == "Hipotecario");
                    if(loan1 != null)
                    {
                        var clientLoan1 = new ClientLoan
                        {
                            Amount = 400000,
                            ClientId = client1.Id,
                            LoanId = loan1.Id,
                            Payments = "60"
                        };
                        context.ClientLoans.Add(clientLoan1);
                    }
                }
                var loan2 = context.Loans.FirstOrDefault(l => l.Name == "Personal");
                if (loan2 != null)
                {
                    var clientLoan2 = new ClientLoan
                    {
                        Amount = 50000,
                        ClientId = client1.Id,
                        LoanId = loan2.Id,
                        Payments = "12"
                    };
                    context.ClientLoans.Add(clientLoan2);
                }

                var loan3 = context.Loans.FirstOrDefault(l => l.Name == "Automotriz");
                if (loan3 != null)
                {
                    var clientLoan3 = new ClientLoan
                    {
                        Amount = 100000,
                        ClientId = client1.Id,
                        LoanId = loan3.Id,
                        Payments = "24"
                    };
                    context.ClientLoans.Add(clientLoan3);
                }
                context.SaveChanges();
            };

            if(!context.Cards.Any())
            {
                var client1 = context.Clientes.FirstOrDefault(cl => cl.Email == "juan@gmail.com");
                if(client1 != null)
                {
                    var cards = new Card[]
                    {
                        new Card
                        {
                            ClientId= client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.DEBIT.ToString(),
                            Color = CardColor.GOLD.ToString(),
                            Number = "3325-6745-7876-4445",
                            Cvv = 990,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(4),
                        },
                        new Card {
                            ClientId= client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.CREDIT.ToString(),
                            Color = CardColor.TITANIUM.ToString(),
                            Number = "2234-6745-552-7888",
                            Cvv = 750,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(5),
                        },
                    };
                    foreach (Card card in cards)
                    {
                        context.Cards.Add(card);
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
