using Capa.Entidades;
using System.Net.Http.Json;

namespace Capa.Datos
{
    public class ExternoDAO
    {
        private readonly HttpClient _httpClient;

        public ExternoDAO()
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<TipoCambio> ObtenerDolar()
        {
            try
            {
                // Usando llave activa: 9f9f641b8e3635d28c54ef95
                string url = "https://v6.exchangerate-api.com/v6/9f9f641b8e3635d28c54ef95/pair/USD/PEN";
                return await _httpClient.GetFromJsonAsync<TipoCambio>(url);
            }
            catch { return null; }
        }
    }
}