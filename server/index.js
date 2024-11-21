var restify = require('restify');
const { MongoClient } = require('mongodb');
const mqtt = require('mqtt');

// Configurazione MongoDB
const mongoUrl = 'mongodb://localhost:27017'; // MongoDB in esecuzione su localhost:27017
const dbName = 'mqttlucenotturna';
let db;

// Configurazione MQTT
const brokerUrl = 'mqtt://test.mosquitto.org';
const topic = 'testsimoalbytopic/light';
const mqttClient = mqtt.connect(brokerUrl);

// Connessione a MongoDB
MongoClient.connect(mongoUrl, { useNewUrlParser: true, useUnifiedTopology: true })
    .then((client) => {
        console.log("Connesso a MongoDB");
        db = client.db(mqttlucenotturna);
    })
    .catch((error) => console.error("Errore di connessione a MongoDB:", error));

// Connessione al broker MQTT
mqttClient.on('connect', () => {
    console.log("Connesso al broker MQTT");
    mqttClient.subscribe(topic, (err) => {
        if (err) {
            console.error("Errore nella sottoscrizione al topic MQTT:", err);
        } else {
            console.log(`Sottoscritto al topic ${topic}`);
        }
    });
});

// Ricezione messaggi MQTT
mqttClient.on('message', async (topic, message) => {
    try {
        const payload = JSON.parse(message.toString());
        const data = {
            topic: topic,
            message: payload,
            timestamp: new Date()
        };

        // Salvataggio nel database
        const result = await db.collection('lightnotturna').insertOne(data);
        console.log("Dati ricevuti e salvati:", data);
    } catch (error) {
        console.error("Errore durante la ricezione o il salvataggio dei dati:", error);
    }
});

// Creazione del server
var server = restify.createServer();
server.use(restify.plugins.bodyParser());

// Endpoint per ricevere i dati (es. dai sensori)
server.post('/water_coolers/:id', async function (req, res) {
    const coolerId = req.params['id'];
    let data = req.body;

    // Elaborazione dei dati ricevuti (aggiunta timestamp)
    data = {
        coolerId: coolerId,
        ...data,
        timestamp: new Date() // Aggiunge un timestamp corrente
    };

    // Salvataggio nel database
    try {
        const result = await db.collection('lightnotturna').insertOne(data);
        res.send({ message: 'Dati salvati con successo', result: result });
        console.log("Dati ricevuti e salvati via HTTP:", data);
    } catch (error) {
        res.send(500, { error: 'Errore nel salvataggio dei dati' });
        console.error("Errore nel salvataggio dei dati:", error);
    }
});

// Avvio del server
server.listen(8011, function () {
    console.log('%s in ascolto su %s', server.name, server.url);
});

// Gestione dell'arresto del server
process.on('SIGINT', async () => {
    console.log("\nChiusura del server...");
    if (db) {
        await db.close(); // Chiudi la connessione a MongoDB
        console.log("Connessione a MongoDB chiusa.");
    }
    mqttClient.end(); // Chiudi la connessione MQTT
    console.log("Connessione MQTT chiusa.");
    process.exit(0);
});
