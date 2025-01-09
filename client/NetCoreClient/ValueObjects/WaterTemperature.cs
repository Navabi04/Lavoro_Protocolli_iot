namespace NetCoreClient.ValueObjects
{
    internal class WaterTemperature
    {
        // Proprietà read-only per il valore della temperatura
        public int Value { get; }

        // Costruttore che accetta un valore intero e verifica che sia entro un range valido
        public WaterTemperature(int value)
        {
            if (value < 0 || value > 100) // Range valido di esempio: 0-100 gradi Celsius
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Il valore della temperatura deve essere compreso tra 0 e 100.");
            }
            Value = value;
        }

        // Metodo override di ToString per una comoda rappresentazione testuale
        public override string ToString()
        {
            return $"{Value} °C";
        }
    }
}
