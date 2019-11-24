
export default class SocketIpc {
  
  constructor() {
    this.registered = {};
    this.connection = null;
  }

  updateConnection(conn) {
    this.connection = conn;
    this.refresh();
  }

  clearConnection() {
    this.connection = null;
  }

  refresh() {
    if(!this.connection) return;

    Object.keys(this.registered).forEach((chn) => {
      this.registered[chn].forEach((fn) => {
        this.connection.on(chn, fn);
      });
    });
  }

  on(channel, fun) {
    if(!this.registered[channel]) {
      this.registered[channel] = [fun];
    } else {
      this.registered[channel].push(fun);
    }

    if(this.connection) {
      this.connection.on(channel, fun);
    }
  }

  send(channel, x) {
    if(this.connection) {
      if(x === undefined) {
        return this.connection.invoke(channel);
      } 
      return this.connection.invoke(channel, x);
    }
    throw "Connection not established"
  }

  removeListener(channel, fun) {
    if(this.registered[channel]) {
      this.registered[channel] = this.registered[channel].filter(x => x !== fun);
    }
    if(this.connection) {
      this.connection.off(channel, fun);
    }
  }
}