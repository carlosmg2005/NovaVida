using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace NovaVida.Pages
{
    public class IndexModel : PageModel
    {
        private readonly MeuContextoBancoDados _contexto;

        public IndexModel(MeuContextoBancoDados contexto)
        {
            _contexto = contexto;
        }

        public List<ResultadoPesquisa> Resultados { get; set; }

        public async Task OnGet(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var url = "https://www.kabum.com.br/busca/" + term;
                var httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

                var html = await httpClient.GetStringAsync(url);

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var resultados = htmlDocument.DocumentNode.Descendants("a")
                    .Where(a => a.GetAttributeValue("class", "").Contains("productLink"))
                    .Select(a => new ResultadoPesquisa
                    {
                        Nome = a.Descendants("span")
                            .FirstOrDefault(span => span.GetAttributeValue("class", "").Contains("sc-89c457ac-0 jKSmuI sc-d55b419d-16 fMikXK nameCard"))
                            ?.InnerText.Trim(),
                        Preco = a.Descendants("span")
                            .FirstOrDefault(span => span.GetAttributeValue("class", "").Contains("sc-3b515ca1-2 gybgF priceCard"))
                            ?.InnerText.Trim(),
                        ProdutoId = a.GetAttributeValue("href", "").Replace("/produto/", "")
                    })
                    .Where(resultado => resultado.Nome != null && resultado.Preco != null)
                    .ToList();

                Resultados = resultados;

                // Salvar resultados no banco de dados
                _contexto.ResultadosPesquisa.AddRange(resultados);
                await _contexto.SaveChangesAsync();
            }
        }
    }
}
