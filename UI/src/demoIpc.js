
const registered = { 
}

export default {
  on(channel, fun) {
    console.debug(`on ${channel}`);
    if(!registered[channel]) {
      registered[channel] = [fun];
    } else {
      registered[channel].push(fun);
    }
    console.debug(registered);
  },
  send(channel, x) {
    if(channel === 'getConfig') {
      const config = localStorage.getItem('config');
      registered[channel].forEach((fun) => {
        fun({}, config);
      });
    } else if(channel === 'saveConfig') {
      localStorage.setItem('config', x);
      setTimeout(() => {
        registered['getConfig'].forEach((fun) => {
          fun({}, x);
        });
      }, 1000);
    }
    console.debug(`send ${channel}`);
  },
  removeListener(channel, fun) {
    console.debug(`remove ${channel}`);
    registered[channel] = registered[channel].filter(x => x !== fun);
    console.debug(registered);
  }
}