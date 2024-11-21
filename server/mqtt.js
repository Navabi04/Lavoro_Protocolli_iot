const mqtt = require('mqtt');
// Connetti al broker MQTT
const client = mqtt.connect('mqtt://test.mosquitto.org');

client.on('connect', function () {
    console.log("Connesso al broker MQTT test.mosquitto.org");
    // Iscrizione al topic desiderato
    client.subscribe('testsimoalbytopic/#', function (err) {
        if (!err) {
            console.log("Sottoscritto al topic 'testsimoalbytopic/#'");
        } else {
            console.error("Errore nella sottoscrizione al topic:", err);
        }
    });
});

// Gestisci i messaggi ricevuti
client.on('message', function (topic, message) {
    console.log('TOPIC: ' + topic + "\nMESSAGE: " + message.toString());
});
