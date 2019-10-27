import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    connected: false,
    streaming: false,
    layout: [],
  },
  mutations: {
    setConnected(s, v) {
      s.connected = v;
    },
    setStreaming(s, v) {
      s.streaming = v;
    },
    setLayout(s, l) {
      s.layout = l;
    }
  },
  getters: {
    isConnected: s => s.connected,
    isStreaming: s => s.streaming,
    layout: s => s.layout,
  },
  actions: {
  },
  modules: {
  },
});
