using System;
using System.Text.Json;

namespace NetCoreClient.Sensors
{
    class NightLightSensor : INightLightSensorInterface, ISensorInterface
    {
        private static readonly Random Random = new Random();

        public NightLightSensor() { }

        // Metodo che genera un valore casuale per l'intensità della luce (0-100)
        public int LightIntensity()
        {
            // Genera un valore casuale tra 0 e 100 (intensità della luce)
            return Random.Next(0, 101);
        }

        // Metodo per serializzare il valore dell'intensità della luce in formato JSON
        public string ToJson()
        {
            var data = new
            {
                SensorType = "NightLightSensor",
                LightIntensity = LightIntensity(),
                Timestamp = DateTime.UtcNow.ToString("o") // Formato ISO 8601 per la data
            };
            return JsonSerializer.Serialize(data);
        }
    }
}

