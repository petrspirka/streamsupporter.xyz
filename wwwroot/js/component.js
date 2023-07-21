export class SignalComponent {
    #connection;
    #trigger = (obj) => {};
    #reload = () => location.reload();

    constructor(id, uid, type) {
        this.#connection = new signalR.HubConnectionBuilder()
            .withAutomaticReconnect()
            .withUrl(`/Api/Dispatcher?oid=${id}&uid=${uid}&type=${type}`);
    }

    start() {
        this.#connection = this.#connection.build();

        this.#connection.on("Reload", this.#reload);
        this.#connection.on("Trigger", (arg) => {
            this.#trigger(JSON.parse(arg));
        });

        this.#connection.start();
    }

    setReload(func) {
        this.#reload = func;
    }

    setTrigger(func) {
        this.#trigger = func;
    }
}