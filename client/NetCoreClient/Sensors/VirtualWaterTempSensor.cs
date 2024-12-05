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
        public double WaterTemperature()
        {
            // Genera un valore casuale tra 0.0 e 20.0 gradi Celsius
            return Math.Round(Random.NextDouble() * 20, 2);  // Temperatura casuale con 2 decimali
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
