using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Controllers.DTO.Usuarios;
using WebApplication1.Controllers.Problems;

namespace WebApplication1.Controllers.Usuarios
{

    [Route("api/Usuario")]
    [ApiController]
    [Authorize]
    [EnableCors]
    public class UsuarioController : ControllerBase
    {
        // Injeção de dependencia do UserManager e SignInManager do Identity para gerenciar os usuarios do sistema 
        private readonly UserManager<IdentityUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;

        public UsuarioController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            //_signInManager = signInManager;
        }

        // POST: api/Usuario/registrar
        [HttpPost("registrar")]
        [AllowAnonymous]
        public async Task<IResult> Registrar(UsuarioPedido usuarioPedido)
        {
            // Obtem o id do usuário atual
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newUser = new IdentityUser
            {
                UserName = usuarioPedido.Email,
                Email = usuarioPedido.Email,
                //EmailConfirmed = true
            };

            // Cria o usuario no banco de dados usando o UserManager
            //var result = _userManager.CreateAsync(newUser, usuarioPedido.Senha).Result; // .Result é usado para esperar o resultado da operação assincrona
            var result = await _userManager.CreateAsync(newUser, usuarioPedido.Senha); // .Result é usado para esperar o resultado da operação assincrona

            if (!result.Succeeded)
                return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());
                //return Results.BadRequest(result.Errors.First());

            // Claim é uma tabela onde você pode colocar os dados do cliente que não são obrigatorios, mas que podem
            // ser usados para identificar o usuario e autorizar o acesso a recursos do sistema com base nesses dados.
            //var claimResult = _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("EmployeeCode", usuarioPedido.EmployeeCode)).Result;
            //if (!claimResult.Succeeded)
            //    return Results.BadRequest(result.Errors.First());

            // Formato de lista para Claims
            var userClaims = new List<System.Security.Claims.Claim>
            {
                new Claim("EmployeeCode", usuarioPedido.EmployeeCode),
                new Claim("Name", usuarioPedido.Name),
                new Claim("CreatedBy", userId)

            };

            var claimResult = await _userManager.AddClaimsAsync(newUser, userClaims);
            if (!claimResult.Succeeded) 
                return Results.BadRequest(result.Errors.First());

            return Results.Created($"/usuarios/{newUser.Id}", newUser.Id);

        }

        [Authorize(Policy = "Employee005Policy")]
        [HttpGet("usuarios")]
        public IResult GetUsuarios()
        {
            var usuarios = _userManager.Users.ToList();
            var employess = new List<UsuarioResposta>();
            foreach(var usuario in usuarios)
            {
                var claims = _userManager.GetClaimsAsync(usuario).Result;
                var claimName = claims.FirstOrDefault(c => c.Type == "Name");
                // Se o usuario não tiver o nome, envia uma string vazia
                var userName = claimName != null ? claimName.Value : string.Empty; 

                employess.Add(new UsuarioResposta(usuario.Email, userName));
            }

            return Results.Ok(employess);
        }

        [HttpGet("usuariosPaginado")]
        public IResult GetUsuariosPaginado(int pagina, int linhas)
        {
            // pagina = numero da pagina
            // linhas = numero de linhas por pagina
            var usuarios = _userManager.Users.Skip((pagina -1) * linhas).Take(linhas).ToList();
            var employess = new List<UsuarioResposta>();
            foreach (var usuario in usuarios)
            {
                var claims = _userManager.GetClaimsAsync(usuario).Result;
                var claimName = claims.FirstOrDefault(c => c.Type == "Name");
                // Se o usuario não tiver o nome, envia uma srting vazia
                var userName = claimName != null ? claimName.Value : string.Empty;

                employess.Add(new UsuarioResposta(usuario.Email, userName));
            }
            
            return Results.Ok(employess);
        }
    }
}
