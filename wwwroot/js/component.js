export class SignalComponent {
    #connection;
    #trigger = (obj) => {};
    #reload = () => location.reload();

    constructor(id, uid, type) {
        this.#connection = new signalR.HubConnectionBuilder()
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: retryContext => {
                    if (retryContext.elapsedMilliseconds < 60000) {
                        return 10000;
                    }
                    return 30000 + Math.random() * 30;
            })
            .withUrl(`/Api/Dispatcher?oid=${id}&uid=${uid}&type=${type}`);
    }

    async start() {
        this.#connection = this.#connection.build();

        this.#connection.on("Reload", this.#reload);
        this.#connection.on("Trigger", (arg) => {
            console.log("Received Trigger with data: " + arg);
            this.#trigger(JSON.parse(arg));
        });
        console.log("Starting connection");
        await this.#connection.start();
        console.log("Connected with id: " + this.#connection.connectionId);
    }

    setReload(func) {
        this.#reload = func;
    }

    setTrigger(func) {
        this.#trigger = func;
    }
}