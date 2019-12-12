<template>
  <div id="app">
    <RouterView v-if="!pending" />
    <BSpinner
      v-else
      style="margin-top: 50px"
    />
  </div>
</template>

<script>
// import Vue from 'vue';
import * as signalR from "@microsoft/signalr";
const url = process.env.VUE_APP_BASEURL

export default {
  data: () => ({
    pending: true,
  }),
  created() {
    this.connect()
  },
  methods: {
    connect() {
      const conn = new signalR.HubConnectionBuilder()
          .withUrl(`${url}fnirs`)
          .configureLogging(signalR.LogLevel.Information)
          .build();

      conn.start()
        .then(()  => {
          conn.onclose(() => {
            this.$ipc.clearConnection();
            this.reconnect();
          });
          this.$ipc.updateConnection(conn);
          this.pending = false;
        })
        .catch((err) => {
          this.reconnect();
        });
    },
    reconnect() {
      this.pending = true;
      if(this.timeout) {
        clearTimeout(this.timeout);
      }

      this.timeout = setTimeout(() => {
        this.connect();
      }, 2000);
    },
  },
}
</script>

<style lang="scss">
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

$ionicons-font-path: '~ionicons/dist/fonts';
@import "~ionicons/dist/scss/ionicons";
</style>
