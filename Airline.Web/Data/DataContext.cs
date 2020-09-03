using Microsoft.EntityFrameworkCore;
using Airline.Web.Data.Entities;
namespace Airline.Web.Data
{
    using Airline.Web.Data.Entities;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    public class DataContext : IdentityDbContext<User>
    {

        

        // Representação da Tabela de Destinos
        public DbSet<Destination> Destinations { get; set; }

        // Representação da tabela dos Aviões
        public DbSet<Airplaine> Airplaines { get; set; }

        // Representação da tabela dos Países
        public DbSet<Country> Countries { get; set; }


        //Representação da tabela das cidades
        public DbSet<City> Cities { get; set; }

        //Representação da tabela dos departamentos
        public DbSet<Department> Departments { get; set; }

        //Representação da tabela dos detalhes dos departamentos (relação de muitos para muitos a dar origem a uma nova tabela)
        public DbSet<DepartmentDetail> DepartmentDetails { get; set; }

        // Construtor
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Criar um indice único só para o nome do País
            modelBuilder.Entity<Country>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // Criar um indice único só para o nome do Departamento
            modelBuilder.Entity<Department>()
                .HasIndex(c => c.Name)
                .IsUnique();


            // Número de Segurança Social único
            modelBuilder.Entity<User>()
                .HasIndex(b => b.SocialSecurityNumber)
                .IsUnique();

            // Número de Identificação fiscal único
            modelBuilder.Entity<User>()
                .HasIndex(b => b.TaxNumber)
                .IsUnique();


            // Não deixar apagar quando já existem registos (Cascading Delete Rule)
            var cascadeFKs = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }

        


    }
}
