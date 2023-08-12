namespace WebApplication1.Controllers.DTO.Usuarios
{
    // DTO(Data Transfer Object) é um objeto que carrega dados entre processos.
    // Esta é uma forma resumida de declarar uma classe com propriedades.
    public record UsuarioPedido(string Email, string Senha, string Name, string EmployeeCode);
}
