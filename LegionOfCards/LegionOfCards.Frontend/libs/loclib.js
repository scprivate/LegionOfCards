/**
 * This is a utils class which is just for cleaning up the code.
 */
class LocUtils {

    /**
     * Creates and returns the path to the assets of the given file name.
     * @param {string} fileName
     */
    static getAsset(fileName) {
        return 'assets/' + fileName;
    }

    static getState(stateName) {
        return 'states/' + stateName + '.js';
    }

    static getServerUrl(ip) {
        return 'ws://' + ip + ":25319/locgcapi";
    }
}

/**
 * This is the connector for the backend. It offers a WebSocket which is the connection to the Backend-Legion of Cards-Servers (or short, Blocs).
 */
var LocBackend = function(url) {

    var handler = new LocBackendHandler();
    this.handler = handler;

    this.connection = new WebSocket(url);
    this.connection.onmessage = function(event) {
        let packet = JSON.parse(event.data);
        handler.execute(packet.Key, packet.Args);
    };

    this.getConnection = function() {
        return this.connection;
    }

    this.events = function() {
        return this.handler;
    }

    this.callRemote = function(packetKey, ...packetArgs) {
        let packet = {
            Key: packetKey,
            Args: packetArgs
        };
        this.connection.send(JSON.stringify(packet));
    }
};

var LocBackendHandler = function () {
    
    this.events = {};

    this.register = function(name, callback) {
        this.events[name] = callback;
    }

    this.execute = function(name, data) {
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
};