using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Controllers.DTO.Security;

namespace WebApplication1.Controllers.Security
{
    [Route("api/security")]
    [ApiController]
    [AllowAnonymous]
    [EnableCors]
    public class TokenController
    {
        // Injeção de dependencia do UserManager do Identity para gerenciar os usuarios do sistema 
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly ILogger<TokenController> logger;

        public TokenController(UserManager<IdentityUser> userManager, IConfiguration configuration, ILogger<TokenController> logger)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.logger = logger;
        }

        // POST: api/token
        [HttpPost("token")]
        public IResult Post(LoginPedido loginPedido)
        {
            logger.LogInformation("Gerando token para o usuario {0}\n", loginPedido.Email);
            logger.LogWarning("warning");
            logger.LogError("error");

            var user = userManager.FindByEmailAsync(loginPedido.Email).Result;
            if (user == null)
                return Results.BadRequest("Usuário ou senha inválidos");
            if (!userManager.CheckPasswordAsync(user, loginPedido.Senha).Result)
                return Results.BadRequest("Usuário ou senha inválidos");

            var claims = userManager.GetClaimsAsync(user).Result;
            var subject = new ClaimsIdentity(new Claim[]
                {
                    // Estamos colocando o email do usuario no token para que possamos identificar o usuario
                    new Claim(ClaimTypes.Email, loginPedido.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)

                });
            subject.AddClaims(claims);
                
 

            var key = Encoding.ASCII.GetBytes(configuration["JwtBearerTokenSettings:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                SigningCredentials =
                new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = configuration["JwtBearerTokenSettings:Audience"], // Quem pode usar o token
                Issuer = configuration["JwtBearerTokenSettings:Issuer"],    // Quem emitiu o token
                Expires = System.DateTime.UtcNow.AddMinutes(60)             // Tempo de expiração do token
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return Results.Ok( new { token = tokenHandler.WriteToken(token)});
        }


    }
}
