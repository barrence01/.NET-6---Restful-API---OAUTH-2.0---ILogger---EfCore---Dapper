using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using WebApplication1.Controllers.DTO.Usuarios;
using MySqlConnector;
using Dapper;

namespace WebApplication1.Repository.Infraestrutura.Usuarios;

public class UsuariosRepository
{
    private readonly MySqlConnection connection;

    public UsuariosRepository(MySqlConnection connection)
    {
        this.connection = connection;
    }

    public async Task<IEnumerable<UsuarioResposta>> QueryAllUsersWithClaim(int pagina, int linhas)
    {
        // pagina = numero da pagina
        // linhas = numero de linhas por pagina
        var pulo = (pagina - 1) * linhas;
        var query =
            @"select Email, ClaimValue as Name
                from aspnetusers  u inner join aspnetuserclaims c
                on u.id = c.UserId and claimtype = 'name'
                order by Name
                OFFSET @pulo ROWS FETCH FIRST @linhas ROWS ONLY";

        return await connection.QueryAsync<UsuarioResposta>(
            query, new { pulo, linhas });

    }
}
