using Flunt.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Products;

namespace WebApplication1.Repository.Context
{
    // Classe para gerenciar o banco de dados
    public class DataBaseContext : IdentityDbContext<IdentityUser> //Substitui o DbContext para usar o Identity
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }


        protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
        {
            // Configura o tamanho maximo de uma string no banco de dados como padrão
            // a menos que seja especificado um tamanho diferente na classe
            // ou no metodo OnModelCreating
            configuration.Properties<string>()
                .HaveMaxLength(100);
        }

        // Este metodo é chamado quando o modelo é criado pela primeira vez
        // e é usado para configurar o modelo para atender às necessidades do aplicativo.
        // Este método é chamado para cada tipo de entidade no modelo, mas pode ser suprimido para o modelo de banco de dados.
        // O método OnModelCreating é chamado após o método OnConfiguring.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Faz com que a modelgaem da classe pai(IdentityDbContext) seja adicionada ao modelo
            base.OnModelCreating(modelBuilder);

            // Exemplo de como renomear uma coluna no identity
            //modelBuilder.Entity<IdentityUser>(
            //iu => iu.Property(c => c.Email).HasColumnName("usuarioEmail")
            //);

            modelBuilder.Ignore<Notification>(); // Ignora a classe Notification do Flunt para que ela não seja mapeada no banco de dados como se fosse uma entidade

            modelBuilder.Entity<Product>()
                .Property(p => p.Name).IsRequired();
            modelBuilder.Entity<Product>()
                .Property(p => p.Description).HasMaxLength(255);

            modelBuilder.Entity<Category>()
                .Property(p => p.Name).IsRequired();
        }


        public DataBaseContext(DbContextOptions options) : base(options)
        {

        }

        protected DataBaseContext()
        {
        }
    }
}
