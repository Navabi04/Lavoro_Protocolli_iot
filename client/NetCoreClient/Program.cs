using NetCoreClient.Sensors;
using NetCoreClient.Protocols;
using System;
using System.Collections.Generic;
using System.Threading;

namespace WaterCoolerCoapClient
{
    class Program
    {
        static void Main()
        {
            // Definisci i sensori
            List<ISensorInterface> sensors = new List<ISensorInterface>
            {
                new VirtualWaterTempSensor(),
                new IWaterTempSensorInterface()
            };

            // Definisci il protocollo (CoAP in questo caso)
            ProtocolInterface protocol = new Coap("coap://192.168.100.73:5683"); // URL del server CoAP

            // Ciclo per inviare i dati periodicamente
            while (true)
            {
                foreach (ISensorInterface sensor in sensors)
                {
                    // Converti i dati del sensore in formato JSON
                    var sensorValue = sensor.ToJson();

                    // Invia i dati del sensore insieme al suo slug (identificatore)
                    protocol.Send(sensorValue, sensor.GetSlug());  // Aspetta la risposta dal metodo async

                    // Registra i dati inviati nella console
                    Console.WriteLine("Dati inviati: " + sensorValue);

                    // Pausa di 5 secondi prima di inviare i dati successivi
                    Thread.Sleep(5000);
                }
            }
        }
    }
}
