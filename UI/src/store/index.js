import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    connected: false,
    streaming: false,
    collecting: false,
    layout: [],
    layoutPending: false,
    cyclesNum: 8,
    switchMode: 8,
  },
  mutations: {
    setConnected(s, v) {
      s.connected = v;
    },
    setStreaming(s, v) {
      s.streaming = v;
    },
    setCollecting(s, v) {
      s.collecting = v;
    },
    setCyclesNum(s, v) {
      s.cyclesNum = v;
    },
    setSwitch(s, v) {
      s.switchMode = v;
    },
    setLayout(s, l) {
      s.layout = l;
      s.layoutPending = false;
    },
    setLayoutPending(s, p) {
      s.layoutPending = p;
    }
  },
  getters: {
    cyclesNum: s => s.cyclesNum,
    isConnected: s => s.connected,
    isStreaming: s => s.streaming,
    isCollecting: s => s.collecting,
    layout: s => s.layout,
    layoutPending: s => s.layoutPending,
  },
  actions: {
  },
  modules: {
  },
});
