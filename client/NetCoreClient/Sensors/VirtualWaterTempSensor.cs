using System;
using System.Text.Json;
using NetCoreClient.Sensors;
using NetCoreClient.ValueObjects;

namespace NetCoreClient.Sensors
{
    class VirtualWaterTempSensor : IWaterTempSensorInterface, ISensorInterface
    {
        private readonly Random Random;

        public VirtualWaterTempSensor()
        {
            Random = new Random();
        }

        // Metodo che genera una temperatura casuale e restituisce il valore incapsulato
        public int WaterTemperature()
        {
            // Genera un valore casuale tra 0 e 20 gradi Celsius
            return new WaterTemperature(Random.Next(0, 21)).Value;
        }

        // Metodo per serializzare il valore della temperatura in formato JSON
        public string ToJson()
        {
            var data = new
            {
                SensorType = "WaterTempSensor",
                Temperature = WaterTemperature(),
                Timestamp = DateTime.Now
            };
            return JsonSerializer.Serialize(data);
        }
    }
}
