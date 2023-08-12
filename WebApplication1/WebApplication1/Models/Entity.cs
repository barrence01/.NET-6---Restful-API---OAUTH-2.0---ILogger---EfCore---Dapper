using Flunt.Notifications;

namespace WebApplication1.Models
{
    // Esta classe é usada para definir as propriedades que serão usadas em todas as entidades do sistema
    // e assim evitar a repetição de codigo
    public abstract class Entity : Notifiable<Notification> // Notifiable é uma classe do Flunt que permite que a classe que a herda possa ter notificações de erros
    {
        public Guid Id { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string EditedBy { get; set; }

        public DateTime EditedOn { get; set; }

        // Construtor da classe
        // Este construtor escolherá um novo id sempre que uma entidade for criada
        public Entity()
        {
            Id = Guid.NewGuid();
            //CreatedOn = DateTime.Now;
            //EditedOn = DateTime.Now;
        }   
    }
}
