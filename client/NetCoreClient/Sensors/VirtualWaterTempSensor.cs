namespace NetCoreClient.Sensors
{
    public class VirtualWaterTempSensor : ISensorInterface
    {
        public string ToJson()
        {
            return "{\"temperature\": 22.5}";  // Esempio di dati sensore in formato JSON
        }

        public string GetSlug()
        {
            return "virtual_water_temp";  // Identificatore unico per il sensore
        }
    }
}
