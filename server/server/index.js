var restify = require('restify');
const { MongoClient } = require('mongodb');

// URL di connessione a MongoDB
const url = 'mongodb://localhost:27017'; // MongoDB in esecuzione su localhost:27017
const dbName = 'waterCoolerDB';
let db;

// Connessione a MongoDB
MongoClient.connect(url, { useNewUrlParser: true, useUnifiedTopology: true })
    .then((client) => {
        console.log("Connesso a MongoDB");
        db = client.db(dbName);
    })
    .catch((error) => console.error("Errore di connessione a MongoDB:", error));

// Creazione del server
var server = restify.createServer();
server.use(restify.plugins.bodyParser());

// Endpoint per ricevere i dati (es. dai sensori)
server.post('/water_coolers/:id', async function(req, res) {
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
        const result = await db.collection('coolerData').insertOne(data);
        res.send({ message: 'Dati salvati con successo', result: result });
        console.log("Dati ricevuti e salvati:", data);
    } catch (error) {
        res.send(500, { error: 'Errore nel salvataggio dei dati' });
        console.error("Errore nel salvataggio dei dati:", error);
    }
});

// Avvio del server
server.listen(8011, function() {
    console.log('%s in ascolto su %s', server.name, server.url);
});
