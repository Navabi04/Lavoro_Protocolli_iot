using MQTTnet;
using MQTTnet.Client;
using NetCoreClient.Sensors;
using NetCoreClient.Protocols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class SensorCloudApp
{
    private IMqttClient? mqttClient;
    private const string MqttBrokerAddress = "test.mosquitto.org";
    private const string MqttTopic = "testsimoalbytopic/light";

    public async Task StartAsync()
    {
        await ConnectToMqttBrokerAsync();
    }

    private async Task ConnectToMqttBrokerAsync()
    {
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(MqttBrokerAddress)
            .WithCleanSession()
            .Build();

        // Gestione connessione
        mqttClient.ConnectedAsync += async e =>
        {
            Console.WriteLine("Connesso al broker MQTT.");
            await mqttClient.SubscribeAsync(MqttTopic);
            Console.WriteLine($"Sottoscritto al topic: {MqttTopic}");
        };

        // Gestione disconnessione
        mqttClient.DisconnectedAsync += e =>
        {
            Console.WriteLine("Disconnesso dal broker MQTT.");
            return Task.CompletedTask;
        };

        // Gestione messaggi ricevuti
        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            string message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Console.WriteLine($"Messaggio ricevuto sul topic {e.ApplicationMessage.Topic}: {message}");
            HandleReceivedData(message);
            return Task.CompletedTask;
        };

        await mqttClient.ConnectAsync(options);
    }

    private void HandleReceivedData(string message)
    {
        try
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(message);
            Console.WriteLine($"Dati ricevuti: {JsonSerializer.Serialize(data)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore nella gestione del messaggio ricevuto: " + ex.Message);
        }
    }
}

class Program
{
    static async Task Main()
    {
        var app = new SensorCloudApp();
        await app.StartAsync();
    }
}
