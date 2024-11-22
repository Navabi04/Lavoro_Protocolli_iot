using NetCoreClient.Sensors;
using NetCoreClient.Protocols;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class SensorCloudApp
{
    List<ISensorInterface> sensors = new();
    ProtocolInterface protocol;

    public SensorCloudApp()
    {
        // Definisci i sensori
        sensors.Add(new VirtualWaterTempSensor());

        // Imposta l'URL del server cloud (Node.js)
        protocol = new Http("http://50c3-185-122-225-105.ngrok-free.app/water_coolers/123"); // L'endpoint è http://localhost:8011
    }

    public async Task StartSendingDataAsync()
    {
        while (true)
        {
            foreach (ISensorInterface sensor in sensors)
            {
                var sensorValue = sensor.ToJson();
                bool sentSuccessfully = false;

                // Tentativi di invio con retry in caso di errore
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    try
                    {
                        await protocol.Send(sensorValue);  // Ora invio asincrono
                        sentSuccessfully = true;
                        Console.WriteLine("Dati inviati con successo: " + sensorValue);
                        break; // Esce dal ciclo di retry se l'invio ha successo
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Errore di invio al cloud (tentativo {attempt + 1}): " + ex.Message);
                        Thread.Sleep(2000); // Attesa di 2 secondi prima del retry
                    }
                }

                if (!sentSuccessfully)
                {
                    Console.WriteLine("Invio fallito dopo 3 tentativi. Salvando dati per retry futuro.");
                    // Potresti aggiungere un meccanismo per salvare temporaneamente i dati non inviati
                    // E.g., salva in un file, in un database locale, o in una coda
                }

                await Task.Delay(1000); // Attesa asincrona tra le letture
            }
        }
    }
}

class Program
{
    static async Task Main()
    {
        var app = new SensorCloudApp();
        await app.StartSendingDataAsync();  // Esegui il metodo asincrono
    }
}
