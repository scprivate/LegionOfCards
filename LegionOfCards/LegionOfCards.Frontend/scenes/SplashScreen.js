class SplashScreen extends Phaser.Scene {

    constructor() {
        super({key: "splash_screen"})
    }

    init(data) {
        this.handlers = {
            session: data.session,
            account: data.account,
            backend: data.backend,
            manager: data.manager
        };
    }

    preload() {
        LocUtils.addDefaultUIAssets(this.load);
    }

    create() {
        let backgroundSprite = this.add.sprite(this.game.config.width / 2, this.game.config.height / 2, "background");
        backgroundSprite.setDisplaySize(this.game.config.width, this.game.config.height);

        let icon = this.add.sprite(this.game.config.width / 2, this.game.config.height / 2, "trans_icon");
        icon.setDisplaySize(500, 400);
        icon.alpha = 0;

        let byBanner = this.add.sprite(this.game.config.width / 2, this.game.config.height / 2, "los_banner");
        byBanner.setDisplaySize(600, 300);

        this.blur = this.add.sprite(this.game.config.width / 2, this.game.config.height / 2, "blur");
        this.blur.setDisplaySize(this.game.config.width, this.game.config.height);
        this.blur.alpha = 0;

        this.panel = this.add.sprite(this.game.config.width / 2, this.game.config.height / 2, "panel");
        this.panel.alpha = 0;

        this.fadeSprite(byBanner, this.game.config.width / 2, this.game.config.height / 2, () => {
            this.fadeSprite(icon, this.game.config.width / 2, this.game.config.height / 2, () => {
                this.completeLoad();
            });
        });
    }

    completeLoad() {
        switch (this.handlers.manager.getStatus()) {
            case BLOCS_SESSION_FOUND:
                this.scene.start('main_menu');
                return;
            case BLOCS_NEED_LOGIN:
                this.loginPanel();
                return;
            case BLOCS_DISCONNECTED:
            case BLOCS_CONNECTION_FAILED:
                this.blur.alpha = 1;
                this.showPanel(450, 150);
                this.add.text(this.game.config.width / 2 - 145, this.game.config.height / 2 - 70, "Verbindungsfehler", {
                    fontFamily: '"Roboto", serif',
                    fontSize: '35px',
                    fontStyle: 'bold'
                });
                this.add.text(this.game.config.width / 2 - 200, this.game.config.height / 2 - 30, "Es konnte keine Verbindung zu den Legion of Cards-Servern \nhergestellt werden. \n\nBitte versuche es zu einem späteren Zeitpunkt erneut, \noder kontaktiere uns über unseren Support-Discord!", {
                    fontFamily: '"Roboto", serif',
                    fontSize: '15px',
                    align: "center"
                });
                return;
            default:
                setTimeout(() => this.completeLoad(), 5000); //TODO Show kind of loading animation
                return;
        }
    }

    loginPanel() {
        this.showPanel(450, 300);
        let title = this.add.text(this.game.config.width / 2 - 40, this.game.config.height / 2 - 140, "Login", {
            fontFamily: '"Roboto", serif',
            fontSize: '35px',
            fontStyle: 'bold'
        });

        let identifier = LocControls.createTextBox(this, this.game.config.width / 2, this.game.config.height / 2 - 50, "Benutzername oder Email", 300);
        LocControls.createTextBox(this, this.game.config.width / 2, this.game.config.height / 2 + 25, "Passwort", 300, 50, 20, true);

        let login = LocControls.createButton(this.add, this.game.config.width / 2 + 125, this.game.config.height / 2 + 100, "Einloggen", () => {
            this.handlers.session.login(identifier.text, LocControls.getSecret("Passwort"), (success) => {
                if (success) {
                    this.game.scene.add("main_menu", MainMenu, true, {
                        session: this.handlers.session,
                        account: this.handlers.account,
                        backend: this.handlers.backend,
                        manager: this.handlers.manager
                    });
                } else {
                    alert("Die angebenen Daten stimmen nich überein!");
                }
            });
        });
        let register = LocControls.createButton(this.add, this.game.config.width / 2 - 125, this.game.config.height / 2 + 100, "Account erstellen", () => {
            title.destroy();
            login[0].destroy();
            login[1].destroy();
            register[0].destroy();
            register[1].destroy();
            this.registerPanel();
        }, 150, 50, 15);
    }

    registerPanel() {
        this.showPanel(450, 400);
        let title = this.add.text(this.game.config.width / 2 - 130, this.game.config.height / 2 - 185, "Account erstellen", {
            fontFamily: '"Roboto", serif',
            fontSize: '35px',
            fontStyle: 'bold'
        });
        let register = LocControls.createButton(this.add, this.game.config.width / 2 + 125, this.game.config.height / 2 + 150, "Registrieren", () => {

        });
        let login = LocControls.createButton(this.add, this.game.config.width / 2 - 125, this.game.config.height / 2 + 150, "Login", () => {
            title.destroy();
            login[0].destroy();
            login[1].destroy();
            register[0].destroy();
            register[1].destroy();
            this.loginPanel();
        });
    }

    fadeSprite(sprite, x, y, callback) {
        this.fade(sprite, x, y, 0, 1, () => {
            this.fade(sprite, x, y, 1, 0, callback, 1500);
        });
    }

    fade(sprite, x, y, start, end, callback, delay = 0) {
        this.add.tween({
            targets: [sprite],
            ease: 'Sine.easeInOut',
            duration: 1000,
            delay: delay,
            x: {
                getStart: () => x,
                getEnd: () => x,
            },
            y: {
                getStart: () => y,
                getEnd: () => y,
            },
            alpha: {
                getStart: () => start,
                getEnd: () => end
            },
            onComplete: () => {
                callback();
            }
        });
    }

    showPanel(w, h) {
        this.panel.setDisplaySize(w, h);
        this.panel.alpha = 1;
    }
}