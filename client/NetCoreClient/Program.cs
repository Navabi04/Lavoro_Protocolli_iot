using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;  
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NetCoreClient.Sensors;  // Assumendo che tu abbia definito la classe VirtualWaterTempSensor altrove

namespace SensorCloudApp
{
    class SensorCloudApp
    {
        private IMqttClient? mqttClient;
        private const string MqttBrokerAddress = "test.mosquitto.org";
        private const string MqttTopic = "testsimoalby/light";
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task StartAsync()
        {
            await ConnectToMqttBrokerAsync();
            await PublishSensorDataContinuouslyAsync();  // Pubblica i dati del sensore in modo continuo
        }

        // Connessione al broker MQTT
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

        // Gestisce i messaggi ricevuti dal broker MQTT
        private async void HandleReceivedData(string message)
        {
            try
            {
                if (double.TryParse(message, out var numericValue))
                {
                    Console.WriteLine($"Messaggio numerico ricevuto: {numericValue}");
                    var data = new { value = numericValue, type = "numeric" };
                    await SendDataToServerAsync(data);
                }
                else
                {
                    try
                    {
                        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(message);
                        if (data != null)
                        {
                            Console.WriteLine($"Dati ricevuti (JSON): {JsonSerializer.Serialize(data)}");
                            await SendDataToServerAsync(data);
                        }
                        else
                        {
                            Console.WriteLine("I dati ricevuti sono nulli.");
                        }
                    }
                    catch (JsonException)
                    {
                        Console.WriteLine($"Messaggio non in formato JSON valido: {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore nella gestione del messaggio ricevuto: " + ex.Message);
            }
        }

        // Funzione per inviare i dati al server Node.js
        private async Task SendDataToServerAsync(object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("http://localhost:8011/sensor-data", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Dati inviati correttamente al server.");
                }
                else
                {
                    Console.WriteLine($"Errore durante l'invio dei dati al server: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore durante l'invio dei dati al server: " + ex.Message);
            }
        }

        // Metodo per pubblicare continuamente i dati del sensore su MQTT
        public async Task PublishSensorDataContinuouslyAsync()
        {
            var sensor = new VirtualWaterTempSensor();  // Instanzia il sensore
            while (true)
            {
                var jsonData = sensor.ToJson();  // Ottieni i dati del sensore come JSON

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(MqttTopic)
                    .WithPayload(jsonData)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)  // Usa QoS ExactlyOnce
                    .Build();

                // Invia il messaggio solo se il client è connesso
                if (mqttClient != null && mqttClient.IsConnected)
                {
                    await mqttClient.PublishAsync(message);
                    Console.WriteLine("Dati sensore inviati al broker MQTT.");
                }

                // Attendere 5 secondi prima di inviare di nuovo
                await Task.Delay(5000);
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
}
