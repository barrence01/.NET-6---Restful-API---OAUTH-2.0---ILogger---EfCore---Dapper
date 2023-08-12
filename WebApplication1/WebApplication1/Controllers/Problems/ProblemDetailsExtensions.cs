using Flunt.Notifications;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Controllers.Problems
{
    // Esta classe é uma classe de extensão para reutilizar o código
    // com relação a listagem de erros do Notifications
    public static class ProblemDetailsExtensions
    {
        // o this antes do tipo de retorno indica que é uma classe de extensão.
        // Classe de extensão é uma classe que estende o comportamento de outra classe.
        // Neste caso, a classe IReadOnlyCollection<Notification> é extendida com o método ConvertToProblemDetails
        // ou seja, na lista de metodos disponíveis para notificações, será adicionado o metodo ConvertToProblemDetails
        public static Dictionary<string, string[]> ConvertToProblemDetails(this IReadOnlyCollection<Notification> notifications)
        {
            // LINQ - Agrupará todos os valores por chave e retornará um dicionario com a chave e os valores
            return notifications
                .GroupBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Message).ToArray());

        }
        public static Dictionary<string, string[]> ConvertToProblemDetails(this IEnumerable<IdentityError> error)
        {
            //return error
            //    .GroupBy(g => g.Code)
            //    .ToDictionary(g => g.Key, g => g.Select(x => x.Description).ToArray());

            //Transforma os erros ema lista única de strings ao invés de manter em grupos
            var dictionary = new Dictionary<string, string[]>();
            dictionary.Add("Error", error.Select(x => x.Description).ToArray());
            return dictionary;
        }
    }
}
