using System;
using System.Threading.Tasks; // Per Task e async/await
using CoAP;
using CoAP.Net;

namespace NetCoreClient.Protocols
{
    class Coap : ProtocolInterface
    {
        private const string PATH_PREFIX = "casette/v1/id_1/sensori";
        private readonly string endpoint;

        public Coap(string endpoint)
        {
            this.endpoint = endpoint; // Esempio di endpoint: "coap://localhost"
        }

        // Metodo asincrono per inviare i dati
        public async Task Send(string data, string sensor)
        {
            try
            {
                // Costruisce l'URI completo per il sensore
                string resourceUri = $"{endpoint}/{PATH_PREFIX}/{sensor}";

                // Crea una richiesta CoAP POST
                var request = new Request(Method.POST)
                {
                    URI = new Uri(resourceUri),
                    PayloadString = data,
                    ContentType = MediaType.ApplicationJson // Imposta il tipo di contenuto su JSON
                };

                Console.WriteLine($"[CoAP] Inviando dati: {data} a {resourceUri}");

                // Invia la richiesta e ottiene la risposta (asincrono)
                var response = request.Send(); // Usa SendAsync se il metodo è asincrono
    
                // Gestisci la risposta
                if (response != null)
                {
                    // Stampa il codice di risposta
                    Console.WriteLine($"[CoAP] Risposta ricevuta: {response.Code}");
                }
                else
                {
                    Console.WriteLine("[CoAP] Nessuna risposta ricevuta.");
                }
            }
            catch (Exception ex)
            {
                // Gestione degli errori
                Console.WriteLine($"Errore durante l'invio del messaggio CoAP: {ex.Message}");
            }
        }
    }
}
