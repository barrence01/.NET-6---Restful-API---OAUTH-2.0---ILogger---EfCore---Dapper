using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using WebApplication1.Controllers.DTO.Usuarios;
using WebApplication1.Repository.Infraestrutura.Usuarios;

namespace WebApplication1.Controllers.Usuarios
{
    // Esta classe utilizará o dapper para fazer as consultas no banco de dados
    // Teoricamente, o dapper é mais rápido que o entity framework
    // Como o dappper não tem acesso ao dbcontext, ele não pode fazer as operações
    // sem antes receber uma conexão com o banco de dados
    [Route("api/UsuarioDapper")]
    [ApiController]
    [Authorize]
    [EnableCors]
    public class UsuarioDapperController : ControllerBase
    {
        private readonly MySqlConnection connection;
        private readonly UsuariosRepository usuariosRepository;

        public UsuarioDapperController(MySqlConnection connection)
        {
            this.connection = connection;
            this.usuariosRepository = new UsuariosRepository(connection);
        }

        // IConfiguration é uma interface que permite acessar o arquivo appsettings.json
        // e pegar as informações de conexão com o banco de dados
        [HttpGet("usuariosDapper")]
        public IResult GetAllUsuariosDapper()
        {
            var employees = connection.Query<UsuarioResposta>(
                @"select Email, ClaimValue as Name
                from aspnetusers  u inner join aspnetuserclaims c
                on u.id = c.UserId and claimtype = 'name'
                order by Name"
                );

            return Results.Ok(employees);
        }


        [HttpGet("usuariosDapperPaginado")]
        public async Task<IResult> GetUsuariosPaginadoDapper(int? pagina, int? linhas)
        {
            // pagina = numero da pagina
            // linhas = numero de linhas por pagina
            var resultado = await usuariosRepository.QueryAllUsersWithClaim( pagina.Value, linhas.Value);

            return Results.Ok(resultado);
        }
    }
}
