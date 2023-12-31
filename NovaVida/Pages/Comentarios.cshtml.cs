using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HtmlAgilityPack;

namespace NovaVida.Pages
{
    public class ComentariosModel : PageModel
    {
        public List<string> Comentarios { get; set; }

        public async Task OnGet(string produtoId, string nome)
        {
            if (!string.IsNullOrEmpty(produtoId) && !string.IsNullOrEmpty(nome))
            {
                Comentarios = await BuscarComentariosProduto(produtoId, nome);
            }
        }

        private async Task<List<string>> BuscarComentariosProduto(string produtoId, string nome)
        {
            var url = $"https://www.kabum.com.br/produto/{produtoId}/{nome}";
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var comentarios = htmlDocument.DocumentNode.Descendants("div")
                .Where(div => div.GetAttributeValue("class", "").Contains("sc-6975f759-3 laVkFt"))
                .Select(div => div.InnerText.Trim())
                .Take(5)
                .ToList();

            return comentarios;
        }
    }
}
