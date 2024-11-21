using System;
using System.Text.Json;
using NetCoreClient.Sensors;

namespace NetCoreClient.Sensors
{
    class NightLightSensor : INightLightSensorInterface, ISensorInterface
    {
        private readonly Random Random;

        public NightLightSensor()
        {
            Random = new Random();
        }

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
                Timestamp = DateTime.Now
            };
            return JsonSerializer.Serialize(data);
        }
    }
}
