class MainMenu extends Phaser.Scene {

    constructor() {
        super({key: "main_menu"});
    }

    init(data) {
    }

    preload() {
        LocUtils.addDefaultUIAssets(this.load);
    }

    create() {
        console.log("Eingeloggt: " + LocUtils.getHandlers().session.session.accessToken);
    }
}