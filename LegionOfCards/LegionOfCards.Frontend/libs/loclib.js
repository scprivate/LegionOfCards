const BLOCS_CONNECTION_FAILED = -1;
const BLOCS_SESSION_FOUND = 0;
const BLOCS_NEED_LOGIN = 1;
const BLOCS_DISCONNECTED = 2;
const BLOCS_CONNECTING = 3;

/**
 * This is a utils class which is just for cleaning up the code.
 */
class LocUtils {

    static getHandlers() {
        return data;
    }

    /**
     * Creates and returns the path to the assets of the given file name.
     * @param {string} fileName
     */
    static getAsset(fileName) {
        return 'assets/' + fileName;
    }

    static getServerUrl(ip) {
        return 'ws://' + ip + ":25319/locgcapi";
    }

    static centerGame(game) {
        game.scale.pageAlignHorizontally = true;
        game.scale.pageAlignVertically = true;
        game.scale.refresh();
    }

    static addDefaultUIAssets(loader) {
        loader.image('los_banner', '../assets/ui/banner.png');
        loader.image('background', '../assets/ui/background.png');
        loader.image('trans_icon', '../assets/ui/trans_icon.png');
        loader.image('button_raw', '../assets/ui/button_raw.png');
        loader.image('button_active', '../assets/ui/button_active.png');
        loader.image('button_clicked', '../assets/ui/button_clicked.png');
        loader.image('text_box', '../assets/ui/text_box.png');
        loader.image('text_box_active', '../assets/ui/text_box_active.png');
        loader.image('blur', '../assets/ui/blur.png');
        loader.image('panel', '../assets/ui/panel.png');
    }
}

let textBoxes = {};
let textBoxEventHooked = false;

class LocControls {

    static createButton(add, x, y, textStr, callback, w = 150, h = 50, size = 20) {
        let design = add.sprite(x, y, "button_raw");
        design.setDisplaySize(w, h);
        design.setInteractive();
        let text = add.text(x, y - Math.ceil(size / 2), textStr, {
            fontFamily: '"Roboto", serif',
            fontSize: size + 'px'
        });
        text.align = "center";
        text.x = x - text.width / 2;
        design.setInteractive();
        design.on('pointerover', () => {
            design.setTexture('button_active');
        });
        design.on('pointerout', () => {
            design.setTexture('button_raw');
        });
        design.on('pointerdown', () => callback());
        return [design, text];
    }

    static clearTextBoxes() {
        for (let key in textBoxes) {
            textBoxes[key].design.destroy();
            textBoxes[key].text.destroy();
        }
        textBoxes = {};
    }

    static isKeyAllowed(key) {
        return key != "Control" && key != "Alt" && key != "Shift" && key != "Tab" && key != "CapsLock";
    }

    static createTextBox(scene, x, y, placeHolder, w = 150, h = 50, size = 20, pw = false) {
        if (textBoxEventHooked == false) {
            textBoxEventHooked = true;
            scene.input.keyboard.on('keydown', event => {
                for (let key in textBoxes) {
                    if (textBoxes[key].focus == true) {
                        if (event.key == "Backspace") {
                            textBoxes[key].text.text = textBoxes[key].text.text.length <= 1 ? ""
                                : textBoxes[key].text.text.substring(0, textBoxes[key].text.text.length - 1);
                            if (textBoxes[key].isPw) {
                                textBoxes[key].secretText = textBoxes[key].secretText.length <= 1 ? ""
                                    : textBoxes[key].secretText.substring(0, textBoxes[key].secretText.length - 1);
                            }
                        } else {
                            if (this.isKeyAllowed(event.key)) {
                                if (textBoxes[key].isPw) {
                                    textBoxes[key].secretText += event.key;
                                    textBoxes[key].text.text += "*";
                                } else {
                                    textBoxes[key].text.text += event.key;
                                }
                            }
                            //TODO Resize box if over
                        }
                        textBoxes[key].text.x = x - textBoxes[key].text.width / 2;
                        break;
                    }
                }
            });
        }
        let design = scene.add.sprite(x, y, "text_box");
        design.setDisplaySize(w, h);
        design.setInteractive();
        design.on('pointerdown', () => {
            for (let key in textBoxes) {
                textBoxes[key].focus = false;
                textBoxes[key].outline.setTexture('text_box');
                textBoxes[key].text.style.color = "#a5a5a5";
                if (textBoxes[key].text.text == "") {
                    textBoxes[key].text.text = textBoxes[key].placeholder;
                    textBoxes[key].text.x = x - textBoxes[key].text.width / 2;
                }
            }
            textBoxes[placeHolder].focus = true;
            textBoxes[placeHolder].text.style.color = "#ffffff";
            textBoxes[placeHolder].outline.setTexture('text_box_active');
            if (textBoxes[placeHolder].text.text == placeHolder) {
                textBoxes[placeHolder].text.text = "";
            }
        });
        let text = scene.add.text(x, y - Math.ceil(size / 2), placeHolder, {
            fontFamily: '"Roboto", serif',
            fontSize: size + 'px',
            fill: '#a5a5a5'
        });
        text.align = "center";
        text.x = x - text.width / 2;
        textBoxes[placeHolder] = {
            focus: false,
            isPw: pw,
            outline: design,
            text: text,
            secretText: "",
            design: design,
            placeholder: placeHolder
        };
        return text;
    }

    static getSecret(placeHolder) {
        if (textBoxes[placeHolder]) {
            return textBoxes[placeHolder].secretText;
        }
        return "";
    }

    static containsBox(x, y, w, h) {
        return (event.x >= x && event.x <= x + w) && (event.y >= y && event.y <= y + h);
    }
}

/**
 * This is the connector for the backend. It offers a WebSocket which is the connection to the Backend-Legion of Cards-Servers (or short, Blocs).
 */
var LocBackend = function (url) {

    var handler = new LocBackendHandler();
    this.handler = handler;

    this.connection = new WebSocket(url);
    this.connection.onmessage = function (event) {
        let packet = JSON.parse(event.data);
        handler.execute(packet.Key, packet.Args);
    };

    this.getConnection = function () {
        return this.connection;
    }

    this.events = function () {
        return this.handler;
    }

    this.callRemote = function (packetKey, ...packetArgs) {
        let packet = {
            Key: packetKey,
            Args: packetArgs
        };
        this.connection.send(JSON.stringify(packet));
    }
};

var LocBackendHandler = function () {

    this.events = {};

    this.register = function (name, callback) {
        this.events[name] = callback;
    }

    this.execute = function (name, data) {
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

var AccountManager = function (backend, sessionManager) {

    this.backend = backend;
    this.sessionManager = sessionManager;

    this.backend.events().register('creation_data-check_result',
        (resultCode) => {
            if (resultCode == 0) {
                this.backend.callRemote('create-account', this.data.username, this.data.email, this.data.password);
                delete this.data;
            } else if (resultCode == 1) {
                console.log("Email existiert!");
            } else if (resultCode == 2) {
                console.log("Benutzername existiert!");
            } else if (resultCode == 3) {
                console.log("Email und Benutzername existieren!");
            }
        });

    this.backend.events().register('account-creation_result',
        (success) => {
            console.log(success ? "Account wurde erstellt!" : "Account-Erstellung fehlgeschlagen!");
        });

    this.backend.events().register('send-discord_verify_url',
        (url) => {
            window.location.href = url;
        });

    this.backend.events().register('account-deletion_failure',
        () => {
            console.log("Ein fehler ist bei der Löschung aufgetreten!");
        });

    this.create = function (username, email, password) { //TODO Adding account creation callback
        this.data = {username: username, email: email, password: password};
        this.backend.callRemote('check-creation_data', username, email);
    }

    this.verifyDiscord = function () {
        if (this.sessionManager.session) {
            if (this.sessionManager.session.accessToken) {
                this.backend.callRemote('request-discord_verification', this.sessionManager.session.accessToken);
            }
        }
    }

    this.delete = function () {
        if (this.sessionManager.session) {
            if (this.sessionManager.session.accessToken) {
                this.backend.callRemote('delete-account', this.sessionManager.session.accessToken);
            }
        }
    }
}

var SessionManager = function (backend) {

    this.backend = backend;

    this.register = function (userID, token) {
        this.session = {userID: userID, accessToken: token};
        setCookie("loc_session_access_token", token, 1);
    }

    this.backend.events().register('session-result',
        (success, userID, token) => {
            if (success) {
                this.register(userID, token);
            }

            if (this.loginCallback)
                this.loginCallback(success);
        });

    this.isOpen = function () {
        return this.token != undefined;
    }

    this.login = function (identifier, password, callback) {
        this.loginCallback = callback;
        this.backend.callRemote('check-password', identifier, password);
    }

    this.logout = function () {
        if (this.session)
            this.backend.callRemote('logout-session', this.session.accessToken);
    }

    this.destroy = function () {
        delete this.session;
        eraseCookie("loc_session_access_token");
    }
};

function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    document.cookie = name + '=; Max-Age=-99999999;';
}

var data;

var GameManager = function (sessionManager, accountManager, backend) {

    data = {
        session: sessionManager,
        account: accountManager,
        backend: backend,
        manager: this
    };

    const phaserConfig = {
        type: Phaser.AUTO,
        title: "Legion of Cards",
        url: "legionfofsensei.de/loc",
        width: 1366,
        height: 768,
        version: "pre1.0A"
    };

    this.game = new Phaser.Game(phaserConfig);

    this.init = function () {
        this.state = BLOCS_CONNECTING;
        this.game.scene.add("splash_screen", SplashScreen);
        this.startScene("splash_screen");
    }

    this.startScene = function (name) {
        this.game.scene.start(name, data);
    }

    this.getGame = function () {
        return this.game;
    }

    this.setStatus = function (state) {
        this.state = state;
    }

    this.getStatus = function () {
        return this.state;
    }
};