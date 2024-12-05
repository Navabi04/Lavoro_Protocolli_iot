const { MongoClient } = require('mongodb');
const restify = require('restify');
const amqp = require('amqplib');

// Configurazione MongoDB
const mongoUrl = 'mongodb://localhost:27017';
const dbName = 'iotamqp';
let db;

// Configurazione RabbitMQ
const rabbitMqUrl = 'amqps://fhshurll:N3mUUPbhrsa4wIHpJILagc9DZeeYjObD@cow.rmq2.cloudamqp.com/fhshurll';
const exchangeName = 'water_cooler_exchange';
const routingKey = 'es.aa123.literdispensed';
let channel;

// Connessione a MongoDB
MongoClient.connect(mongoUrl)
    .then((client) => {
        console.log("Connesso a MongoDB");
        db = client.db(dbName);
    })
    .catch((error) => console.error("Errore di connessione a MongoDB:", error));

// Connessione a RabbitMQ
async function connectToRabbitMq() {
    try {
        const connection = await amqp.connect(rabbitMqUrl);
        channel = await connection.createChannel();

        // Dichiarazione dell'exchange (tipo topic)
        await channel.assertExchange(exchangeName, 'topic', { durable: true });

        // Creazione di una coda e binding con la routing key
        const queue = await channel.assertQueue('', { exclusive: true }); // Coda temporanea esclusiva
        await channel.bindQueue(queue.queue, exchangeName, 'es.aa123.#');

        console.log("Connesso a RabbitMQ e in ascolto dei messaggi...");

        // Ascolto dei messaggi dalla coda
        channel.consume(queue.queue, async (msg) => {
            if (msg) {
                const messageContent = msg.content.toString();
                console.log(`Messaggio ricevuto: ${messageContent}`);

                try {
                    const payload = JSON.parse(messageContent);
                    const data = {
                        topic: msg.fields.routingKey,
                        message: payload,
                        timestamp: new Date(),
                    };

                    // Salvataggio nel database
                    const result = await db.collection('collection').insertOne(data);
                    console.log("Dati ricevuti e salvati:", data);
                } catch (error) {
                    console.error("Errore durante la ricezione o il salvataggio dei dati:", error);
                }

                // Conferma del messaggio
                channel.ack(msg);
            }
        });
    } catch (error) {
        console.error("Errore di connessione a RabbitMQ:", error);
    }
}

connectToRabbitMq();

// Creazione del server Restify
var server = restify.createServer();
server.use(restify.plugins.bodyParser());

// Endpoint per ricevere i dati via HTTP
server.post('/water_coolers/:id', async function (req, res) {
    const coolerId = req.params['id'];
    let data = req.body;

    // Elaborazione dei dati ricevuti (aggiunta timestamp)
    data = {
        coolerId: coolerId,
        ...data,
        timestamp: new Date(),
    };

    // Salvataggio nel database
    try {
        const result = await db.collection('collection').insertOne(data);
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
        await db.client.close(); // Chiudi la connessione a MongoDB
        console.log("Connessione a MongoDB chiusa.");
    }
    if (channel) {
        await channel.close(); // Chiudi il canale RabbitMQ
        console.log("Connessione RabbitMQ chiusa.");
    }
    process.exit(0);
});
