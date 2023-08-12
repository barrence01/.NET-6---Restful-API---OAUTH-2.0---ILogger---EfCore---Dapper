using Flunt.Validations;

namespace WebApplication1.Models.Products;

public class Category : Entity
{
    public string Name { get; private set; }

    public bool Active { get; private set; }

    // Construtor da classe
    // Normalmente utilizado em metodos POST
    public Category(string name, string createdBy, string editedBy)
    {
        Name = name;
        Active = true;
        CreatedBy = createdBy;
        EditedBy = editedBy;
        CreatedOn = DateTime.Now;
        EditedOn = DateTime.Now;

        Validate();
    }

    // Criado ao selecionar o metodo e ctrl+r+m
    private void Validate()
    {
        var contract = new Contract<Category>()
            .IsNotNullOrEmpty(Name, "Name", "Nome é obrigatório") // Verifica se o nome é nulo, caso seja nulo, adiciona uma notificação.
            .IsGreaterOrEqualsThan(Name, 3, "Name", "Nome deve ter no mínimo 3 caracteres")
            .IsNotNullOrEmpty(CreatedBy, "CreatedBy", "CreatedBy é obrigatório")
            .IsNotNullOrEmpty(EditedBy, "EditedBy", "EditedBy é obrigatório");
        AddNotifications(contract); // Adiciona as notificações ao objeto.
    }

    // Normalmente utilizado em metodos PUT
    public void EditInfo(string name, bool active, string editedBy)
    {
        Active = active;
        Name = name;
        EditedBy = editedBy;
        EditedOn = DateTime.Now;

        Validate();
    }
}
