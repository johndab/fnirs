import Vue from 'vue';
import App from './App.vue';
import router from './router';
import store from './store';
import BootstrapVue from 'bootstrap-vue'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'

import demoIpc from './demoIpc';

// tslint:disable-next-line
const { ipcRenderer } = require("electron");

Vue.use(BootstrapVue)
Vue.config.productionTip = false;

Vue.prototype.$ipc = ipcRenderer;
// Vue.prototype.$ipc = demoIpc;

new Vue({
  router,
  store,
  render: (h) => h(App),
}).$mount('#app');
