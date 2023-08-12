using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Controllers.DTO.Categories;
using WebApplication1.Controllers.Problems;
using WebApplication1.Models.Products;
using WebApplication1.Repository.Context;

namespace WebApplication1.Controllers.Categories;

[Route("api/Categories")]
[ApiController]
[Authorize]
[EnableCors]
public class CategoryController : Controller
{

    // Injeção de dependencia do UserManager e SignInManager do Identity para gerenciar os usuarios do sistema 
    private readonly DataBaseContext _context;


    public CategoryController(DataBaseContext context)
    {
        _context = context;
    }

    // GET: api/Categories
    [HttpGet("categories")]
    public IResult CategoryGetAll()
    {
        var categories = _context.Categories.ToList();

        // Cria uma lista com os dados Name e Active de cada categoria
        var resposta = categories.Select(c => new CategoryResposta(c.Id, c.Name, c.Active)).ToList();

        return Results.Ok(resposta);
    }

    // POST: api/Categories
    [HttpPost("categories")]
    public async Task<IResult> CategoryPost(CategoryPedido categoryPedido)
    {
        // Obtem o id do usuário atual
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


        var category = new Category(categoryPedido.Name, userId, userId);
        // Ao abrir chave na instancia de uma classe, você pode definir os valores das propriedades
        // diretamente
        //{
        //    Name = categoryPedido.Name,
        //    CreatedBy = "Test",
        //    EditedBy = "Test"
        //};

        // LINQ - Agrupará todos os valores por chave e retornará um dicionario com a chave e os valores
        if (!category.IsValid)
            return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails()); 


        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        return Results.Created($"/categories/{category.Id}", category.Id);
    }

    // PUT: api/Categories/{id}
    [HttpPut("categories/{id:guid}")]
    public IResult CategoryPut([FromRoute] Guid id,CategoryPedido categoryPedido)
    {
        // Obtem o id do usuário atual
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
        if(category == null)
            return Results.NotFound();

        category.EditInfo(categoryPedido.Name, categoryPedido.Active, userId);

        if(!category.IsValid)
            return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());

        // Atualiza o registro no banco de dados
        // Ao alterar o context diretamente, ao dar save change, o entity framework vai gerar um update para o registro
        _context.SaveChanges();




        return Results.Ok();
    }

}
