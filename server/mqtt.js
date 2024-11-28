const mqtt = require('mqtt');
// Connetti al broker MQTT
const client = mqtt.connect('mqtt://test.mosquitto.org');

// Gestisci la connessione al broker
client.on('connect', function () {
    console.log("Connesso al broker MQTT test.mosquitto.org");
    // Iscrizione al topic desiderato
    client.subscribe('testsimoalby/#', function (err) {
        if (!err) {
            console.log("Sottoscritto al topic 'testsimoalby/#'");
        } else {
            console.error("Errore nella sottoscrizione al topic:", err);
        }
    });
});

// Gestisci gli errori di connessione
client.on('error', function (err) {
    console.error("Errore di connessione:", err);
    client.end(); // Chiude la connessione in caso di errore
});

// Gestisci i messaggi ricevuti
client.on('message', function (topic, message) {
    try {
        console.log(`TOPIC: ${topic}`);
        console.log(`MESSAGE: ${message.toString()}`);
        // Se il messaggio è JSON, puoi deserializzarlo
        try {
            const jsonMessage = JSON.parse(message.toString());
            console.log('Messaggio JSON ricevuto:', jsonMessage);
        } catch (jsonError) {
            console.log('Il messaggio non è in formato JSON valido');
        }
    } catch (err) {
        console.error('Errore durante la gestione del messaggio:', err);
    }
});

// Gestione della disconnessione
client.on('close', function () {
    console.log('Connessione al broker MQTT chiusa');
});

// Gestione della connessione persa
client.on('reconnect', function () {
    console.log('Riconnessione al broker MQTT in corso...');
});
