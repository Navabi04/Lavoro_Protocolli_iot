const coap = require('coap');

// Creazione del server CoAP
const coapServer = coap.createServer();

// Gestione delle richieste
coapServer.on('request', async (req, res) => {
    // Controlla che il metodo sia POST
    if (req.method === 'POST') {
        try {
            // Estrai l'ID del cooler dalla URL
            const coolerId = req.url.split('/')[2];
            let data = JSON.parse(req.payload.toString());

            // Aggiungi un timestamp ai dati ricevuti
            data = {
                coolerId: coolerId,
                ...data,
                timestamp: new Date(),
            };

            // Rispondi al client con un messaggio di successo
            res.end('Dati ricevuti con successo');
            console.log("Dati ricevuti via CoAP:", data);
        } catch (error) {
            // Gestisci eventuali errori di parsing
            res.code = '5.00';
            res.end('Errore nell\'elaborazione dei dati');
            console.error("Errore nell'elaborazione dei dati via CoAP:", error);
        }
    } else {
        // Gestione di metodi non supportati (diversi da POST)
        res.code = '4.05';
        res.end('Metodo non supportato');
    }
});

// Avvio del server CoAP sulla porta 5683
coapServer.listen(5683, () => {
    console.log('Server CoAP in ascolto sulla porta 5683');
});

// Gestione dell'arresto del server (SIGINT)
process.on('SIGINT', async () => {
    console.log("\nChiusura del server...");
    process.exit(0);
});
