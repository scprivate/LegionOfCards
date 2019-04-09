/**
 * This is a utils class which is just for cleaning up the code.
 */
class LocUtils {

    /**
     * Creates and returns the path to the assets of the given file name.
     * @param {string} fileName
     */
    static getAsset(fileName) {
        return '../assets/' + fileName;
    }

    static getServerUrl() {
        return '';
    }
}

/**
 * This is the connector for the backend. It offers a WebSocket which is the connection to the Backend-Legion of Cards-Servers (or short, Blocs).
 */
class LocBackend {

    constructor(url) {
        this.handler = new LocBackendHandler();
        this.connection = new WebSocket(url);
        this.connection.onopen = onConnect;
        this.connection.onclose = onDisconnect;
        this.connection.onerror = onError;
        this.connection.onmessage = onReceive;
    }

    // ---------- CALLBACKS ----------- \\
    onReceive(event) {
        let packet = JSON.parse(event.data);
        this.handler.execute(packet.key, packet.args);
    }

    onConnect(event) {

    }

    onDisconnect(event) {

    }

    onError(event) {
        alert("An error occurred in the LocBackend-Blocs-Connection: " + event);
    }
    // ---------- CALLBACKS ----------- \\

    /**
     * @returns {WebSocket}
     */
    get getConnection() {
        return this.connection;
    }

    get events() {
        return this.handler;
    }

    callRemote(packetKey, ...packetArgs) {
        let packet = {
            key: packetKey,
            args: packetArgs
        };
        this.connection.send(JSON.stringify(packet));
    }
}

class LocBackendHandler {

    constructor() {
        this.events = {};
    }

    register(name, callback) {
        this.events[name] = callback;
    }

    execute(name, data) {
        let args = [];
        for (let key in data) {
            if (data.hasOwnProperty(key)) {
                args.push(data[key]);
            }
        }

        if (this.events[name]) {
            this.events[name].apply(null, args);
        }
    }
}