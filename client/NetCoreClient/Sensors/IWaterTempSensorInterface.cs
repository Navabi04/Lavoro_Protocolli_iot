namespace NetCoreClient.Sensors
{
    public class IWaterTempSensorInterface : ISensorInterface
    {
        public string ToJson()
        {
            return "{\"temperature\": 23.0}";  // Esempio di dati sensore in formato JSON
        }

        public string GetSlug()
        {
            return "real_water_temp";  // Identificatore unico per questo sensore
        }
    }
}
