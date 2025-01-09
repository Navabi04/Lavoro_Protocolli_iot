namespace NetCoreClient.Sensors
{
    public interface ISensorInterface
    {
        string ToJson();  // Metodo per convertire i dati del sensore in formato JSON
        string GetSlug(); // Metodo per ottenere un identificatore unico per il sensore
    }
}
