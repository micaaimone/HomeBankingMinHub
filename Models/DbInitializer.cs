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
        }
    }
}
