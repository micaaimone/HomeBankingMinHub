using Microsoft.EntityFrameworkCore;

namespace HomeBankingMinHub.Models
{
    //hago esto para poder usar la libreria DbContext
    //(que permite consultar/manipular y almacenar instancias de las entidades de nuestra aplicación en ella)
    public class HomeBankingContext : DbContext
    {
        //constructor
        public HomeBankingContext(DbContextOptions<HomeBankingContext> options) : base(options) { }

        //dbsets
        //representa la tabla de base de datos con objetos de tipo client
        public DbSet<Client> Clientes { get; set; }

        public DbSet<Account> Accounts { get; set; }
    }
}
