using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetCoreClient.Protocols
{
    class Http : ProtocolInterface
    {
        private readonly string Endpoint;
        private readonly HttpClient client;

        public Http(string endpoint)
        {
            this.Endpoint = endpoint;
            this.client = new HttpClient();
        }

        // Modifica il tipo di ritorno da async void a async Task
        public async Task Send(string data)
        {
            try
            {
                var content = new StringContent(data);

                // Effettua una richiesta POST asincrona
                var response = await client.PostAsync(Endpoint, content);

                // Verifica lo stato della risposta e lo stampa
                Console.Out.WriteLine($"Status Code: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Errore durante l'invio dei dati: {ex.Message}");
            }
        }
    }
}
